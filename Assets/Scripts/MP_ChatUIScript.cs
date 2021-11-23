using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MP_ChatUIScript : NetworkBehaviour
{
    public Text chatText = null;
    public InputField chatInput = null;

    NetworkVariableString messages = new NetworkVariableString("Temp");
    public NetworkList<MPPlayerInfo> chatPlayers;
    private String playerName = "N/A";

    public GameObject scoreCardPanel;
    public Text scorePlayerName;
    public Text scoreKills;
    public Text scoreDeaths;
    private bool showScore = true;

    // Start is called before the first frame update
    void Start()
    {
        messages.OnValueChanged += updateUIClientRpc;
        foreach(MPPlayerInfo player in chatPlayers)
        {
            if (NetworkManager.LocalClientId == player.networkClientId)
            {
                playerName = player.networkPlayerName;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            showScore = true;
            //scoreCardPanel.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            showScore = false;
            //scoreCardPanel.SetActive(false);
        }

        if (showScore)
        {
            scoreCardPanel.SetActive(showScore);
            if (IsOwner)
            {
                updateUIScoreServerRpc();
            }
        }
        else
        {
            scoreCardPanel.SetActive(showScore);
        }
    }

    // Update is called once per frame
    public void handleSend()
    {

        if (!IsServer)
        {
            sendMessageServerRpc(chatInput.text);
        }
        else
        {
            messages.Value += "\n" + playerName + " says: " + chatInput.text;
        }

    }

    [ClientRpc]
    private void updateUIClientRpc(string previousValue, string newValue)
    {
        chatText.text += newValue.Substring(previousValue.Length, newValue.Length - previousValue.Length);
    }

    [ServerRpc]
    private void sendMessageServerRpc(string text, ServerRpcParams svrParam = default)
    {
        foreach (MPPlayerInfo player in chatPlayers)
        {
            if (svrParam.Receive.SenderClientId == player.networkClientId)
            {
                playerName = player.networkPlayerName;
            }
        }
        messages.Value += "\n" + playerName + " says: " + text;
    }
    
    [ServerRpc]
    private void updateUIScoreServerRpc(ServerRpcParams svrParams = default)
    {
        //clear old scores
        clearUIScoreClientRpc();
        //get each player's info
        GameObject[] currentPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObj in currentPlayers)
        {
            foreach (MPPlayerInfo playerInfo in chatPlayers)
            {
                if (playerObj.GetComponent<NetworkObject>().OwnerClientId == playerInfo.networkClientId)
                {
                    updateUIScoreClientRpc(playerInfo.networkPlayerName, playerObj.GetComponent<MPPlayerAttributes>().kills.Value, playerObj.GetComponent<MPPlayerAttributes>().deaths.Value);
                }
            }
        }

    }

    [ClientRpc]
    private void updateUIScoreClientRpc(string networkPlayerName, int kills, int deaths)
    {
        if (IsOwner)
        {
            scorePlayerName.text += networkPlayerName + "\n";
            scoreKills.text += kills + "\n";
            scoreDeaths.text += deaths + "\n";
        }
    }

    [ClientRpc]
    private void clearUIScoreClientRpc()
    {
        if (IsOwner)
        {
            scorePlayerName.text = "";
            scoreKills.text = "";
            scoreDeaths.text = "";
        }
    }
    
}
