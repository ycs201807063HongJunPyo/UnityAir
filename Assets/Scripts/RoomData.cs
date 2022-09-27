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

    public bool switchBool = false;

    private int readyCount = 3;
    public static int airforceCount = 0;

    [SerializeField]
    private Text roomDataText;
    [SerializeField]
    private Text readyCountText;
    [SerializeField]
    private Text airForceStatText;
    [SerializeField]
    private Text airForceText;

    void Awake() {
        airforceCount = 0;
        rInstance = this;
        //roomDataText = GetComponentInChildren<Text>();
        airForceStatText.text = string.Format("무기 : 기관총x2(250발)\n체력 : 7\n이동속도 : 3/3");
        airForceText.text = string.Format("기본적인 전투기로 모든 상황에 적절히 대응합니다.");
    }
    public void UpdateInfo() {
        roomDataText.text = string.Format("Room Name : {0} [{1} / {2}]", roomName, PhotonNetwork.PlayerList.Length, maxCount);
    }
     
    public void NextButton() {
        airforceCount++;
        if(airforceCount > 1) {
            airforceCount = 0;
        }
        if(airforceCount == 0) {
            airForceStatText.text = string.Format("무기 : 기관총x2(250발)\n체력 : 7\n이동속도 : 3/3");
            airForceText.text = string.Format("기본적인 전투기로 모든 상황에 적절히 대응합니다.");
        }
        else if(airforceCount == 1) {
            airForceStatText.text = string.Format("무기 : 기관포x2(20발)\n체력 : 5\n이동속도 : 5/2");
            airForceText.text = string.Format("강한 화력과 높은 이동능력을 가졌지만 적은 탄창과 낮은 체력, 느린 좌우 이동을 커버할 컨트롤이 필요합니다.\n\n관통탄 : 적을 1회 관통합니다.");
        }
    }

    void Update(){
        UpdateInfo();
        if (PhotonNetwork.PlayerList.Length == maxCount && switchBool == false) {
            StartCoroutine("GameReadyTimer", 1);
            switchBool = true;
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
            GameManager.gmInstance.gameStart = true;
            readyCountText.text = "";
        }
    }
}
