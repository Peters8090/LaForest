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
                        heart.transform.parent = healthUI.transform;
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
    public void TakeDamage(float damage)
    {
        //Debug.LogError("Damage: " + damage);
        health -= damage;
    }

    public void Regenerate()
    {
        health = maxHealth;
    }
}
