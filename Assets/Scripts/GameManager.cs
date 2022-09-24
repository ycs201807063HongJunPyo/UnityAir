using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviourPun, IPunObservable {
    public static GameManager gmInstance;

    [SerializeField]
    private Text gameWaitText;
    private int waitCount = 15;

    public GameObject[] enemyObjs;
    public Transform[] spawnPoints;

    public bool gameStart;
    public bool gameWait;
    public bool switchBool = false;

    public float maxDelay;
    public float curDelay;

    public int gameStage;
    public int gameMaxUnit;


    void Awake() {
        gmInstance = this;
        gameMaxUnit = 5;
    }
    void Start() {
        gameStage = 1;
    }

    void Update() {
        if (gameStart == true && gameMaxUnit > 0) {
            if (!PhotonNetwork.IsMasterClient) {
                return;
            }
            curDelay += Time.deltaTime;
            if (curDelay > maxDelay) {
                SpawnEnemy();
                maxDelay = Random.Range(0.5f, 1.5f);
                curDelay = 0;
            }
        }
        else if(gameStart == true && gameMaxUnit <= 0) {
            
            if (switchBool == false) {
                if (PhotonNetwork.IsMasterClient) {
                    PhotonNetwork.Instantiate("StatPoint", Vector3.zero, Quaternion.identity);
                }
                gameStage++;
                WaitTime();
                switchBool = true;
            }
        }
        
    }
    //statSelectPanel.SetActive(true);
    //PlayerStat.psInstance.OpenStat();

    void SpawnEnemy() {
        int ranEnemy = Random.Range(0, 3);
        int ranPoint = Random.Range(0, 5);
        PhotonNetwork.Instantiate(enemyObjs[ranEnemy].name, spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
        gameMaxUnit--;
    }
    void WaitTime() {
        StartCoroutine("GameWaitTimer", 1);
    }

    IEnumerator GameWaitTimer(float delayTime) {
        gameWaitText.text = string.Format("{0}초 후 게임이 시작됩니다.\n{1}스테이지 준비중", waitCount, gameStage);
        yield return new WaitForSeconds(delayTime);
        waitCount--;
        if (waitCount >= 0) {
            StartCoroutine("GameWaitTimer", 1);
        }
        else {
            waitCount = 15;
            gameMaxUnit =5;
            
            gameWaitText.text = "";
            switchBool = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(gameStage);
            stream.SendNext(gameWaitText.text);
        }
        else {
            this.gameStage = (int)stream.ReceiveNext();
            this.gameWaitText.text = (string)stream.ReceiveNext();
        }
    }
}
