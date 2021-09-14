using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class BasicInputs : NetworkBehaviour
{
    public float movementSpeed = 5f;
    public Transform camT;
    CharacterController mpCharController;

    void Start()
    {
        mpCharController = GetComponent<CharacterController>();

        //Color change check
        if (IsLocalPlayer)
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.blue;
        }

        //disable other cameras
        if(!IsLocalPlayer)
        {
            camT.GetComponent<Camera>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            MPMovePlayer();
        }
        
    }

    void MPMovePlayer()
    {

        Vector3 moveVect = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        mpCharController.SimpleMove(moveVect * movementSpeed);

    }
}
