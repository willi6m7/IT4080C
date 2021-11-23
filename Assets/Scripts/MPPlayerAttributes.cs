using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MPPlayerAttributes : NetworkBehaviour
{

    public Slider hpBar;

    private float maxHp = 100f;
    private float damageValue = 15f;
    private float healValue = 20f;
    private float damageMultiplier = 2f;

    private NetworkVariableFloat currentHp = new NetworkVariableFloat(100f);

    public NetworkVariableInt kills = new NetworkVariableInt(0);
    public NetworkVariableInt deaths = new NetworkVariableInt(0);

    // Update is called once per frame
    void Update()
    {
        hpBar.value = currentHp.Value / maxHp;
        if (currentHp.Value <= 0)
        {
            RespawnPlayerServerRpc();
            ResetPlayerClientRpc();
            if (IsOwner)
            {
                Debug.Log("Dead!");
            }

        }
    }

    //On collision
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet") && IsOwner)
        {
            if (collision.gameObject.GetComponent<MP_BulletScript>().spawnPlayerId != NetworkManager.Singleton.LocalClientId)
            {
                Debug.Log("Hit!");

                if (currentHp.Value - damageValue < 0)
                {
                    IncreaseKillCountServerRpc(collision.gameObject.GetComponent<MP_BulletScript>().spawnPlayerId);
                }

                TakeDamageServerRpc(damageValue);
                Destroy(collision.gameObject);
            }

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
    private void TakeDamageServerRpc(float damage, ServerRpcParams svrParams = default)
    {
        currentHp.Value -= damage;
        if (currentHp.Value < 0 && OwnerClientId == svrParams.Receive.SenderClientId)
        {
            Debug.Log("Dead!");
            deaths.Value++;
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

    [ServerRpc]
    private void RespawnPlayerServerRpc()
    {
        //sent health to maximum
        currentHp.Value = maxHp;
    }

    [ClientRpc]
    private void ResetPlayerClientRpc()
    {
        //reset player position to spawn point
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        int index = UnityEngine.Random.Range(0, spawnPoints.Length);
        GameObject currentPoint = spawnPoints[index];

        GetComponent<CharacterController>().enabled = false;
        transform.position = spawnPoints[index].transform.position;
        GetComponent<CharacterController>().enabled = true;
    }

    [ServerRpc]
    private void IncreaseKillCountServerRpc(ulong spawnPlayerId)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObj in players)
        {
            if (playerObj.GetComponent<NetworkObject>().OwnerClientId == spawnPlayerId)
            {
                playerObj.GetComponent<MPPlayerAttributes>().kills.Value++;
            }
        }
    }
}
