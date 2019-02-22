using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsefulReferences
{
    public static GameObject player;
    public static GameObject mainCamera;
    public static GameObject eq;
    public static PlayerMovement playerMovement;
    public static PlayerWeapons playerWeapons;
    public static Animator playerAnimator;
    public static CharacterController playerCharacterController;
    public static PlayerDeath playerDeath;
    public static PlayerHealth playerHealth;
    public static GameObject ui;
    public static GameObject environment;
    public static GameObject mainMenuCamera;
    public static GameObject multiplayerGameControlObject;
    public static RawImage activeWeaponImg;
    public static GameObject healthUI;
    public static GameObject deathUI;
    public static AudioSource playerAudioSource;
    public static bool initialized = false;

    public static void Initialize(GameObject myPlayer)
    {
        if(myPlayer != null)
        {
            player = myPlayer;
            mainCamera = player.transform.Find("Main Camera").gameObject;
            eq = player.GetComponent<UsefulReferencesPlayer>().eq;
            //eq = player.transform.Find("ybot/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/Equipment").gameObject;
            playerMovement = player.GetComponent<PlayerMovement>();
            playerWeapons = player.GetComponent<PlayerWeapons>();
            playerAnimator = player.transform.Find("ybot").gameObject.GetComponent<Animator>();
            playerCharacterController = player.GetComponent<CharacterController>();
            playerDeath = player.GetComponent<PlayerDeath>();
            playerHealth = player.GetComponent<PlayerHealth>();
            playerAudioSource = player.GetComponent<AudioSource>();
            initialized = true;
        }
        
        ui = GameObject.Find("UI");
        activeWeaponImg = GameObject.Find("UI").transform.Find("Weapons/ActiveWeapon").gameObject.GetComponent<RawImage>();
        healthUI = ui.transform.Find("Health").gameObject;
        deathUI = ui.transform.Find("Death").gameObject;
        environment = GameObject.Find("Environment");
        multiplayerGameControlObject = GameObject.Find("MultiplayerGameControlObject");
    }
}
