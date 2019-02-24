using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNickTextMesh : MonoBehaviour
{
    void FixedUpdate()
    {
        if (UsefulReferences.initialized)
        {
            if (transform.parent != UsefulReferences.player)
            {
                transform.LookAt(UsefulReferences.player.transform);
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

            }
        }
    }
}
