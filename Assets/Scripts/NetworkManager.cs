using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField NickNameInput;
    public InputField RoomInput;
    //public GameObject Panel;

    private void Awake() {
        Screen.SetResolution(800, 600, false);
        PhotonNetwork.SendRate = 120;
        PhotonNetwork.SerializationRate = 60;
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() {
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        PhotonNetwork.JoinOrCreateRoom(RoomInput.text, new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom() {
        //IDPanel.SetActive(false);
        StartCoroutine("DestoryBullet");
        RoomData.rInstance.roomName = RoomInput.text;
        //RoomData.rInstance.UpdateInfo();
        Spawn();
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
        PhotonNetwork.Instantiate("Prototype_Fighter_03", Vector3.zero, Quaternion.identity);

    }

    public override void OnDisconnected(DisconnectCause cause) {
        //IDPanel.SetActive(true);
    }
}
