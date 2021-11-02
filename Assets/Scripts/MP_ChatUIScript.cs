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
}
