using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsefulReferences
{
    //gameObjects
    public static GameObject playerResources;
    public static GameObject player;
    public static GameObject mainCamera;
    public static GameObject ybot;
    public static GameObject eq;
    public static GameObject ui;
    public static GameObject environment;
    public static GameObject multiplayerGameControlObject;
    public static GameObject localGameControlObject;
    public static GameObject healthUI;
    public static GameObject deathUI;
    public static GameObject crosshairUI;

    //my scripts
    public static PlayerMovement playerMovement;
    public static PlayerWeapons playerWeapons;
    public static CharacterController playerCharacterController;
    public static PlayerDeath playerDeath;
    public static PlayerHealth playerHealth;
    public static PlayerRagdoll playerRagdoll;
    public static PlayerSounds playerSounds;
    
    //built-in scripts
    public static Animator playerAnimator;
    public static RawImage activeWeaponImg;
    public static AudioSource playerAudioSource;
    
    //useful constant values
    public const float terrainSize = 2500f;

    public static bool initialized = false;

    public static void Initialize(GameObject myPlayer)
    {
        if(myPlayer != null)
        {
            player = myPlayer;
            mainCamera = player.GetComponent<UsefulReferencesPlayer>().mainCamera;
            ybot = player.transform.Find("ybot").gameObject;
            eq = player.GetComponent<UsefulReferencesPlayer>().eq;
            playerMovement = player.GetComponent<PlayerMovement>();
            playerWeapons = player.GetComponent<PlayerWeapons>();
            playerAnimator = player.transform.Find("ybot").gameObject.GetComponent<Animator>();
            playerCharacterController = player.GetComponent<CharacterController>();
            playerDeath = player.GetComponent<PlayerDeath>();
            playerRagdoll = player.GetComponent<PlayerRagdoll>();
            playerHealth = player.GetComponent<PlayerHealth>();
            playerAudioSource = player.GetComponent<AudioSource>();
            playerSounds = player.GetComponent<PlayerSounds>();
            initialized = true;
        }

        playerResources = (GameObject) Resources.Load("Player");
        ui = GameObject.Find("UI");
        activeWeaponImg = GameObject.Find("UI").transform.Find("Weapons/ActiveWeapon").gameObject.GetComponent<RawImage>();
        healthUI = ui.transform.Find("Health").gameObject;
        deathUI = ui.transform.Find("Death").gameObject;
        environment = GameObject.Find("Environment");
        multiplayerGameControlObject = GameObject.Find("MultiplayerGameControlObject");
        localGameControlObject = GameObject.Find("LocalGameControlObject");
        crosshairUI = ui.transform.Find("Crosshair").gameObject;
    }
}
