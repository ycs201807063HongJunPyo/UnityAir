using System.Collections;
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
    public RoomOptions roomOp;
    public GameObject airSelectPanel;
    public GameObject mainGamePanel;
    public GameObject stageText;
    public GameObject winPanel;

    private void Awake() {
        nInstance = this;
        Screen.SetResolution(1080, 1920, false);
        PhotonNetwork.SendRate = 120;
        PhotonNetwork.SerializationRate = 60;
        roomOp = new RoomOptions();
    }
    /*
    public void isRoomStart() {
        if(roomOp.IsOpen != false) {
            Connect();
        }
        else { 
        }
    }
    */
    
    public void Connect() { 
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        roomOp.MaxPlayers = 2;
        roomOp.IsOpen = true;
    }

    public void SoloConnect() {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        roomOp.MaxPlayers = 1;
        roomOp.IsOpen = false;
    }

    public override void OnConnectedToMaster() {
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
            bullet.GetComponent<PhotonView>().RPC("DestroyRPC", RpcTarget.All);
        }
        
    }

    public void Spawn() {
        stageText.SetActive(true);
        roomOp.IsOpen = false;
        PhotonNetwork.Instantiate("Prototype_Fighter_03", Vector3.zero, Quaternion.identity);
        airSelectPanel.SetActive(false);
        mainGamePanel.SetActive(true);
    }

    public void Win() {
        foreach (GameObject palyer in GameObject.FindGameObjectsWithTag("Player")) {
            palyer.GetComponent<PhotonView>().RPC("DestroyRPC", RpcTarget.All);
        }
        winPanel.SetActive(true);
        Invoke("Disconnect", 2f);
    }
    public void Disconnect() {
        PhotonNetwork.Disconnect();
        Application.Quit();
    }
}
