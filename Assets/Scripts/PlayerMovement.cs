using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    bool running = false;
    bool jumped = false;
    [HideInInspector]
    public bool movementLocked = false;
    public bool mouseLookLocked = false;
    public bool moving = false;
    public bool slowDown = false;
    float movementX;
    float movementY;
    float playerY = 0f;
    float movingSpeed = 10f;
    float runningSpeed = 15f;
    float jumpHeight = 5f;
    float mouseSensitivity = 3f;
    float mouseSensitivityDefaultValue;
    float mouseX = 0f;
    float mouseY = 0f;
    CharacterController cc;
    Animator animator;
    GameObject mainCamera;
    GameObject obj;
    AudioSource audioSource;
    [SerializeField]
    AudioClip jumpingSound;
    [SerializeField]
    AudioClip landingSound;
    [SerializeField]
    AudioClip[] footstepSounds;


    void Start()
    {
        cc = UsefulReferences.playerCharacterController;
        animator = UsefulReferences.playerAnimator;
        mainCamera = UsefulReferences.mainCamera;
        audioSource = GetComponent<AudioSource>();
        obj = transform.Find("ybot/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head/MainCameraPosRot").gameObject;
        mouseSensitivityDefaultValue = mouseSensitivity;
    }
    
    void Update()
    {
        if (mouseLookLocked)
            mouseSensitivity = 0f;
        else
            mouseSensitivity = mouseSensitivityDefaultValue;

        //set the main camera's pos and rot equal to obj's (without setting its parent to head in hips)
        mainCamera.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
        mainCamera.transform.rotation = obj.transform.rotation;
        
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

        running = Input.GetButton("Run");

        if (cc.isGrounded)
        {
            if (jumped)
            {
                audioSource.PlayOneShot(landingSound);
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
        }
        if (!movementLocked)
        {
            if (slowDown)
            {
                if (running)
                {
                    movementY = Input.GetAxis("Vertical") * runningSpeed / 2;
                    movementX = Input.GetAxis("Horizontal") * runningSpeed / 2;
                }
                else
                {
                    movementY = Input.GetAxis("Vertical") * movingSpeed / 2;
                    movementX = Input.GetAxis("Horizontal") * movingSpeed / 2;
                }
            }
            else
            {
                if (running)
                {
                    movementY = Input.GetAxis("Vertical") * runningSpeed;
                    movementX = Input.GetAxis("Horizontal") * runningSpeed;
                }
                else
                {
                    movementY = Input.GetAxis("Vertical") * movingSpeed;
                    movementX = Input.GetAxis("Horizontal") * movingSpeed;
                }
            }
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
            audioSource.PlayOneShot(jumpingSound);
            jumped = true;

        }
        else if (!cc.isGrounded)
        {
            playerY += Physics.gravity.y * Time.deltaTime;
        }

        moving = animator.GetCurrentAnimatorStateInfo(0).IsName("Movement") && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Idle";

        /*if (!audioSource.isPlaying)
        {
            if (moving)
            {
                audioSource.PlayOneShot(walkingSounds[Random.Range(0, walkingSounds.Length)]);
            }
            else
            {
                audioSource.Stop();
            }
        }*/

        //move the player
        Vector3 move = new Vector3(movementX, playerY, movementY);
        move = transform.rotation * move;

        cc.Move(move * Time.deltaTime);
    }

    void Wsad()
    {

    }

    void MouseLook()
    {
        
    }
}
