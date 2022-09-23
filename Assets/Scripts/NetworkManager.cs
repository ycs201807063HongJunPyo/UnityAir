﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager nInstance;

    public InputField NickNameInput;
    public InputField RoomInput;
    RoomOptions roomOp;
    public GameObject airSelectPanel;

    private void Awake() {
        nInstance = this;
        Screen.SetResolution(1080, 1920, false);
        PhotonNetwork.SendRate = 120;
        PhotonNetwork.SerializationRate = 60;
    }

    public void isRoomStart() {
        if(roomOp.IsOpen != false) {
            Connect();
        }
        else { 
        }
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() {
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        roomOp = new RoomOptions();
        roomOp.MaxPlayers = 2;
        roomOp.IsOpen = true;
        PhotonNetwork.JoinOrCreateRoom(RoomInput.text, roomOp, null);
    }

    public override void OnJoinedRoom() {
        //IDPanel.SetActive(false);
        StartCoroutine("DestoryBullet");
        RoomData.rInstance.roomName = RoomInput.text;

        //RoomData.rInstance.UpdateInfo();
        //Spawn();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) {
            PhotonNetwork.Disconnect();
        }    
    }

    IEnumerator DestoryBullet() {
        yield return new WaitForSeconds(0.2f);
        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("Bullet")) {
            bullet.GetComponent<PhotonView>().RPC("DestoryRPC", RpcTarget.All);
        }
        
    }

    public void Spawn() {
        roomOp.IsOpen = false;
        PhotonNetwork.Instantiate("Prototype_Fighter_03", Vector3.zero, Quaternion.identity);
        airSelectPanel.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause) {
        //IDPanel.SetActive(true);
    }
}
