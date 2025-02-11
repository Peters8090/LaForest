﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    bool running = false;
    bool jumped = false;
    public bool movementLocked = true;
    public bool mouseLookLocked = false;
    public bool moving = false;
    public bool slowDown = false;
    float movementX;
    float movementY;
    float playerY = 0f;
    float movingSpeed = 10f;
    float runningSpeed = 15f;
    float curMaxSpeed = 0f;
    float jumpHeight = 5f;
    float mouseSensitivity = 3f;
    float mouseSensitivityDefaultValue;
    float mouseX = 0f;
    float mouseY = 0f;
    CharacterController cc;
    Animator animator;
    GameObject mainCamera;
    [HideInInspector]
    public GameObject mainCameraPosRot;
    AudioSource audioSource;
    AudioClip jumpingSound;
    AudioClip landingSound;
    //variables for playing footstep sounds
    AudioClip[] footstepSounds;
    float m_StepCycle = 0;
    float m_NextStep = 0;
    float m_RunstepLenghten = 1;
    float m_StepInterval = 6;
    public bool isGrounded;
    bool prevIsGrounded;
    float delayBetweenGroundCheck = 0.2f;
      

    void Start()
    {
        jumpingSound = (AudioClip)Resources.Load("Jump");
        landingSound = (AudioClip)Resources.Load("Land");
        footstepSounds = new AudioClip[2] { (AudioClip)Resources.Load("Footstep01"), (AudioClip)Resources.Load("Footstep02") };
        cc = UsefulReferences.playerCharacterController;
        animator = UsefulReferences.playerAnimator;
        mainCamera = UsefulReferences.mainCamera;
        audioSource = GetComponent<AudioSource>();
        mainCameraPosRot = GetComponent<UsefulReferencesPlayer>().mainCameraPosRot;
        mouseSensitivityDefaultValue = mouseSensitivity;
        InvokeRepeating("GroundCheck", 0f, delayBetweenGroundCheck);
    }
    

    void Update()
    {
        if (mouseLookLocked)
            mouseSensitivity = 0f;
        else
            mouseSensitivity = mouseSensitivityDefaultValue;
        
        SetMainCamerasPosRot();
        
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, -90, 90);

        //joysticks compatibility
        if (Input.GetAxis("Joystick 4th axis") > 0.22f)
        {
            mouseX = Input.GetAxis("Joystick 4th axis") * mouseSensitivity;
        }
        else if (Input.GetAxis("Joystick 4th axis") < -0.22f)
        {
            mouseX = Input.GetAxis("Joystick 4th axis") * mouseSensitivity;
        }

        if (Input.GetAxis("Joystick 5th axis") > 0.22f)
        {
            mouseY -= Input.GetAxis("Joystick 5th axis") * mouseSensitivity;
        }
        else if (Input.GetAxis("Joystick 5th axis") < -0.22f)
        {
            mouseY -= Input.GetAxis("Joystick 5th axis") * mouseSensitivity;
        }
        
        transform.Rotate(0, mouseX, 0);
        
        mainCamera.transform.localRotation = Quaternion.Euler(mouseY, 0, 0);
        
        running = Input.GetButton("Run");
        
        //make the player move slower, or faster if needed
        if (running)
        {
            if (slowDown)
                curMaxSpeed = runningSpeed / 2;
            else
                curMaxSpeed = runningSpeed;
        }
        else
        {
            if (slowDown)
                curMaxSpeed = movingSpeed / 2;
            else
                curMaxSpeed = movingSpeed;
        }
        
        //if the movement is locked, for example the pause menu is active, we set the player animation to idle (by resetting VelX and VelY)
        if (!movementLocked)
        {
            animator.SetFloat("VelX", Input.GetAxis("Horizontal"));
            animator.SetFloat("VelY", Input.GetAxis("Vertical"));
            movementY = Input.GetAxis("Vertical") * curMaxSpeed;
            movementX = Input.GetAxis("Horizontal") * curMaxSpeed;
        }
        else
        {
            animator.SetFloat("VelX", 0);
            animator.SetFloat("VelY", 0);
            movementX = 0f;
            movementY = 0f;
        }
        
        if (isGrounded)
        {
            //player landed
            if (!prevIsGrounded)
            {
                UsefulReferences.playerSounds.PlaySound(landingSound);
                jumped = false;
            }

            //player jumps
            if (Input.GetButton("Jump") && !movementLocked && !jumped)
            {
                playerY = jumpHeight;
                animator.Play("Jump");
                audioSource.Stop();
                UsefulReferences.playerSounds.PlaySound(jumpingSound);
                jumped = true;
            }
        }
        else
        {
            playerY += Physics.gravity.y * Time.deltaTime;
            animator.SetFloat("VelX", 0);
            animator.SetFloat("VelY", 0);
        }

        //moving = animator.GetCurrentAnimatorStateInfo(0).IsName("Movement") && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Idle";
        moving = cc.velocity.magnitude > 1f;


        if(!moving)
        {
            animator.SetFloat("VelX", 0);
            animator.SetFloat("VelY", 0);
        }
        
        //move the player
        Vector3 move = new Vector3(movementX, playerY, movementY);
        move = transform.rotation * move;
        
        cc.Move(move * Time.deltaTime);
        
        //to detect if we landed in the next frame
        prevIsGrounded = isGrounded;
    }

    void FixedUpdate()
    {
        if(!movementLocked)
            ProgressStepCycle();
    }

    /// <summary>
    /// Method from FirstPersonController script from Unity Standard Assets
    /// </summary>
    void ProgressStepCycle()
    {
        if (cc.velocity.sqrMagnitude > 0 && (movementX != 0 || movementY != 0))
        {
            m_StepCycle += (cc.velocity.magnitude + (curMaxSpeed * (!running ? 1f : m_RunstepLenghten))) * Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep) || !isGrounded)
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        if(moving)
            PlayFootStepSounds();
    }

    /// <summary>
    /// Set the main camera's pos and rot equal to mainCameraPosRot's (without setting its parent to head in hips)
    /// </summary>
    public void SetMainCamerasPosRot()
    {
        mainCamera.transform.position = new Vector3(mainCameraPosRot.transform.position.x, mainCameraPosRot.transform.position.y, mainCameraPosRot.transform.position.z);
        mainCamera.transform.rotation = mainCameraPosRot.transform.rotation;
    }

    //for PlayFootstepSounds method to check which once it is executed
    int i = 0;
    /// <summary>
    /// Every footstepSounds.Length times footstepsSounds array is randomly ordered with 0% possibility that one sound would play two times in row
    /// </summary>
    void PlayFootStepSounds()
    {
        if (i%footstepSounds.Length == 0 || i==0)
        {
            ShuffleFootStepSounds();
        }
        //audioSource.PlayOneShot(footstepSounds[0]);
        UsefulReferences.playerSounds.PlaySound(footstepSounds[0]);

        //To make one sound never play two times in row
        void ShuffleFootStepSounds()
        {
            AudioClip prevFirstClip;
            prevFirstClip = footstepSounds[0];
            footstepSounds = ShuffleArray(footstepSounds);
            if (prevFirstClip == footstepSounds[0])
                ShuffleFootStepSounds();
        }
    }


    /// <summary>
    /// isGrounded variable is updated 1/delayBetweenGroundCheck times per second to prevent problems while player would have been on an irregular surface
    /// </summary>
    void GroundCheck()
    {
        isGrounded = cc.isGrounded;
    }

    AudioClip[] ShuffleArray(AudioClip[] sounds)
    {
        for (int t = 0; t < sounds.Length; t++)
        {
            AudioClip tmp = sounds[t];
            int r = Random.Range(t, sounds.Length);
            sounds[t] = sounds[r];
            sounds[r] = tmp;
        }
        return sounds;
    }
}
