using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class MPBulletSpawner : NetworkBehaviour
{

    public Rigidbody bullet;
    public Transform bulletSpawnerPos;
    private float bulletSpeed = 10f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && IsOwner)
        {
            Debug.Log("Fired Weapon! 2");
            FireServerRpc();
        }
    }
    [ServerRpc]
    private void FireServerRpc()
    {
        Debug.Log("Fired Weapon!");
        Rigidbody bulletClone = Instantiate(bullet, bulletSpawnerPos.position, transform.rotation);
        bulletClone.velocity = transform.forward * bulletSpeed;
        bulletClone.gameObject.GetComponent<NetworkObject>().Spawn();
        Destroy(bulletClone.gameObject, 3);
    }
}
