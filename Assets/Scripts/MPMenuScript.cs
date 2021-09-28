using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class MPMenuScript : MonoBehaviour
{

    public GameObject startMenu;
    public void OnHostButtonClicked()
    {
        NetworkManager.Singleton.StartHost();
        startMenu.SetActive(false);
    }

    public void OnClientButtonClicked()
    {
        NetworkManager.Singleton.StartClient();
        startMenu.SetActive(false);
    }
}
