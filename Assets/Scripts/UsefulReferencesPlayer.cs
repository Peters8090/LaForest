using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created due to problem, which appeared sometimes for no reason; eq couldn't be found by transform.Find...
/// </summary>
public class UsefulReferencesPlayer : MonoBehaviour
{
    public GameObject eq;
    public GameObject mainCamera;
    public GameObject mainCameraPosRot;
    public Rigidbody[] ybotRagdollRigidbodies;

    void Start()
    {
        
    }
    
    void Update()
    {

    }
}
