using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    [SerializeField]
    readonly float minHealth = 0f;
    [SerializeField]
    readonly float maxHealth = 100f;
    int healthRounded = 10;
    GameObject healthUI;

    void Start()
    {
        health = SaveLoad.sd.health;
        healthUI = UsefulReferences.healthUI;
        healthUI.SetActive(true);
    }

    void Update()
    {
        if (health < minHealth)
            health = minHealth;
        if (health > maxHealth)
            health = maxHealth;
        if (health == minHealth && !UsefulReferences.playerDeath.died)
            UsefulReferences.playerDeath.died = true;

        healthRounded = Mathf.RoundToInt(health / 10);
        if(!(healthRounded == healthUI.transform.childCount))
        {
            if(healthUI.transform.childCount < healthRounded)
            {
                for(int i = 0; i < (maxHealth - minHealth) / 10 - 1; i++)
                {
                    if (healthUI.transform.childCount != healthRounded)
                    {
                        GameObject heart = Instantiate((GameObject)Resources.Load("Heart"));
                        heart.transform.SetParent(healthUI.transform);
                        heart.transform.localScale = Vector3.one; //sometimes for no reason its scale sets to ~ 1.5 and hearts look a bit strange
                    } else
                    {
                        break;
                    }
                }
            }
            else if (healthUI.transform.childCount > healthRounded)
            {
                //I could use while loop, I used for to prevent stack overflow exception
                for (int j = 0; j < (maxHealth - minHealth) / 10 - 1; j++)
                {
                    if (healthUI.transform.childCount != healthRounded)
                    {
                        Destroy(healthUI.transform.GetChild(0).gameObject);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    [PunRPC]
    public void TakeDamage(float damage, Vector3 ?dir2Fall)
    {
        if(health > minHealth)
            health -= damage;
        if (health <= minHealth && !UsefulReferences.playerDeath.died)
        {
            UsefulReferences.playerRagdoll.dir2Fall = dir2Fall;
        }
    }

    public void Regenerate()
    {
        health = maxHealth;
    }
}
