using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomData : MonoBehaviourPunCallbacks {

    public static RoomData rInstance;

    public string roomName = "";
    public int playerCount = 0;
    public int maxCount = 2;

    public bool test = false;

    private int readyCount = 5;

    [SerializeField]
    private Text roomDataText;
    [SerializeField]
    private Text readyCountText;


    void Awake() {
        rInstance = this;
        roomDataText = GetComponentInChildren<Text>();
    }
    public void UpdateInfo() {
        
        roomDataText.text = string.Format("Room Name : {0} [{1} / {2}]", roomName, PhotonNetwork.PlayerList.Length, maxCount);
    }
     
    void Update(){
        UpdateInfo();
        if (PhotonNetwork.PlayerList.Length == maxCount && test == false) {
            StartCoroutine("GameReadyTimer", 1);
            test = true;
        }
    }

    IEnumerator GameReadyTimer(float delayTime) {
        readyCountText.text = string.Format("{0}초 후 게임이 시작됩니다.", readyCount);
        yield return new WaitForSeconds(delayTime);
        readyCount--;
        if (readyCount >= 0) {
            StartCoroutine("GameReadyTimer", 1);
        }
        else {
            roomDataText.text = "";
            NetworkManager.nInstance.Spawn();
            readyCountText.text = string.Format("게임 시작");
        }
    }
}
