using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField NickNameInput;
    public GameObject IDPanel;

    private void Awake() {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 120;
        PhotonNetwork.SerializationRate = 60;
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() {
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom() {
        IDPanel.SetActive(false);
        Spawn();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) {
            PhotonNetwork.Disconnect();
        }    
    }

    public void Spawn() {
        PhotonNetwork.Instantiate("Prototype_Fighter_03", Vector3.zero, Quaternion.identity);

    }

    public override void OnDisconnected(DisconnectCause cause) {
        IDPanel.SetActive(true);
    }
}
