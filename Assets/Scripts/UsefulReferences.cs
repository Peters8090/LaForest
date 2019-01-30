using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsefulReferences
{
    public static GameObject player;
    public static GameObject mainCamera;
    public static GameObject eq;
    public static PlayerMovement playerMovement;
    public static PlayerWeapons playerWeapons;
    public static Animator playerAnimator;
    public static CharacterController playerCharacterController;
    public static GameObject ui;
    public static bool initialized = false;

    public static void Initialize(GameObject myPlayer)
    {
        player = myPlayer;
        mainCamera = player.transform.Find("Main Camera").gameObject;
        eq = player.transform.Find("ybot/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/Equipment").gameObject;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerWeapons = player.GetComponent<PlayerWeapons>();
        playerAnimator = player.transform.Find("ybot").gameObject.GetComponent<Animator>();
        playerCharacterController = player.GetComponent<CharacterController>();
        ui = GameObject.Find("UI");
        initialized = true;
    }
}
