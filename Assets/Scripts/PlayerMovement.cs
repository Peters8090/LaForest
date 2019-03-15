using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    bool running = false;
    bool jumped = false;
    public bool movementLocked = false;
    public bool mouseLookLocked = false;
    public bool moving = false;
    public bool slowDown = false;
    float movementX;
    float movementY;
    float playerY = 0f;
    float movingSpeed = 10f;
    float runningSpeed = 15f;
    float accMaxSpeed = 0f;
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
    }
    

    void Update()
    {
        if (mouseLookLocked)
            mouseSensitivity = 0f;
        else
            mouseSensitivity = mouseSensitivityDefaultValue;
        
        SetMainCameraPosRot();
        
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, -90, 90);

        //Joysticks compatibility
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

        if(mouseLookLocked && UsefulReferences.playerDeath.died)
        {
            SetMainCameraPosRot();
        }

        //set the main camera's pos and rot equal to obj's (without setting its parent to head in hips)
        void SetMainCameraPosRot()
        {
            mainCamera.transform.position = new Vector3(mainCameraPosRot.transform.position.x, mainCameraPosRot.transform.position.y, mainCameraPosRot.transform.position.z);
            mainCamera.transform.rotation = mainCameraPosRot.transform.rotation;
        }

        running = Input.GetButton("Run");

        if (cc.isGrounded)
        {
            if (jumped)
            {
                //audioSource.PlayOneShot(landingSound);
                UsefulReferences.playerSounds.PlaySound(landingSound);
                jumped = false;
            }
            if (!movementLocked)
            {
                animator.SetFloat("VelX", Input.GetAxis("Horizontal"));
                animator.SetFloat("VelY", Input.GetAxis("Vertical"));
            }
            else
            {
                animator.SetFloat("VelX", 0);
                animator.SetFloat("VelY", 0);
            }
        } else
        {
            //animator.SetFloat("VelX", 0);
            //animator.SetFloat("VelY", 0);
        }

        if (running)
        {
            if(slowDown)
                accMaxSpeed = runningSpeed / 2;
            else
                accMaxSpeed = runningSpeed;
        }
        else
        {
            if (slowDown)
                accMaxSpeed = movingSpeed / 2;
            else
                accMaxSpeed = movingSpeed;
        }

        if (!movementLocked)
        {
            movementY = Input.GetAxis("Vertical") * accMaxSpeed;
            movementX = Input.GetAxis("Horizontal") * accMaxSpeed;
        }
        else
        {
            movementX = 0f;
            movementY = 0f;
        }
        
        if (cc.isGrounded && Input.GetButton("Jump") && !movementLocked)
        {
            playerY = jumpHeight;
            animator.Play("Jump");
            audioSource.Stop();
            //audioSource.PlayOneShot(jumpingSound);
            UsefulReferences.playerSounds.PlaySound(jumpingSound);
            jumped = true;

        }
        else if (!cc.isGrounded)
        {
            playerY += Physics.gravity.y * Time.deltaTime;
        }
        
        moving = animator.GetCurrentAnimatorStateInfo(0).IsName("Movement") && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Idle";

        //move the player
        Vector3 move = new Vector3(movementX, playerY, movementY);
        move = transform.rotation * move;

        cc.Move(move * Time.deltaTime);
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
            m_StepCycle += (cc.velocity.magnitude + (accMaxSpeed * (!running ? 1f : m_RunstepLenghten))) * Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep) || !cc.isGrounded)
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        PlayFootStepSounds();
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
