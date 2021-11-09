using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MPPlayerAttributes : NetworkBehaviour
{

    public Slider hpBar;

    private float maxHp = 100f;
    private float damageValue = 20f;
    private float healValue = 20f;
    private float damageMultiplier = 2f;

    private NetworkVariableFloat currentHp = new NetworkVariableFloat(100f); 

    // Update is called once per frame
    void Update()
    {
        hpBar.value = currentHp.Value / maxHp;
    }

    //On collision
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet") && IsOwner)
        {
            Debug.Log("Hit!");
            TakeDamageServerRpc(damageValue);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Medkit") && IsOwner)
        {
            Debug.Log("Healed!");
            HealDamageServerRpc(healValue);
        }
        else if (collision.gameObject.CompareTag("PowerUp") && IsOwner)
        {
            Debug.Log("I do more damage now!");
            PowerUpServerRpc(damageMultiplier);
        }
    }

    [ServerRpc]
    private void TakeDamageServerRpc(float damage)
    {
        currentHp.Value -= damage;
        if(currentHp.Value <= 0)
        {
            Debug.Log("Dead!");
            Destroy(this.gameObject);
        }
    }

    [ServerRpc]
    private void HealDamageServerRpc(float heal)
    {
        currentHp.Value += heal;

        //makes sure health does not go over the max
        if (currentHp.Value > maxHp)
        {
            currentHp.Value = maxHp;
        }
    }

    [ServerRpc]
    private void PowerUpServerRpc(float damageBuff)
    {
        damageValue /= damageBuff;
    }
}
