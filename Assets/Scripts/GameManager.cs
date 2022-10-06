using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviourPun, IPunObservable {
    public static GameManager gmInstance;

    [SerializeField]
    private Text gameWaitText;
    private int waitCount = 7;

    public GameObject[] enemyObjs;
    public GameObject[] cloudObjs;
    public Transform[] spawnPoints;

    public bool gameStart;
    public bool gameWait;
    public bool switchBool;

    public float maxDelay;
    public float curDelay;

    public int gameStage;
    public Text gameStageText;
    public int gameCurUnit;
    public int gameMaxUnit;

    void Awake() {
        gmInstance = this;
    }
    void Start() {
        gameMaxUnit = 5;
        gameCurUnit = gameMaxUnit;
        switchBool = false;
        gameStage = 1;
        gameStageText.text = gameStage.ToString();
    }

    void Update() {
        
        if (gameStart == true && gameCurUnit > 0) {
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
        else if(gameStart == true && gameCurUnit <= 0) {
            if (switchBool == false) {
                StartCoroutine("StageWaitTimer");
                switchBool = true;
            }
        }
        
    }
    //statSelectPanel.SetActive(true);
    //PlayerStat.psInstance.OpenStat();

    void SpawnEnemy() {
        int ranEnemy;
        if (gameStage <= 3) {

            ranEnemy = Random.Range(0, 1);
        }
        else if (gameStage <= 12) {
            ranEnemy = Random.Range(0, 2);
        }
        else {
            ranEnemy = Random.Range(0, 3);
        }
        int ranPoint = Random.Range(0, 5);
        if (ranPoint <= 1) {
            int ranCloud;
            ranCloud = Random.Range(0, 2);
            PhotonNetwork.Instantiate(cloudObjs[ranCloud].name, spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
        }
        PhotonNetwork.Instantiate(enemyObjs[ranEnemy].name, spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
        gameCurUnit--;
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
            waitCount = 7;
            gameMaxUnit += 3;
            gameCurUnit = gameMaxUnit;
            gameWaitText.text = "";
            switchBool = false;
        }
    }

    
    IEnumerator StageWaitTimer() {
        yield return new WaitForSeconds(4.0f);
        if (PhotonNetwork.IsMasterClient) {
            if (gameStage != 9) {
                PhotonNetwork.Instantiate("StatPoint", Vector3.zero, Quaternion.identity);
            }
            else {
                PhotonNetwork.Instantiate("EngravePoint", Vector3.zero, Quaternion.identity);
            }
        }
        gameStage++;
        gameStageText.text = gameStage.ToString();
        if (gameStage >= 21) {
            StartCoroutine("WinGameTimer");
        }
        else {
            WaitTime();
        }
    }

    IEnumerator WinGameTimer() {
        yield return new WaitForSeconds(3.0f);
        NetworkManager.nInstance.Win();
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(gameStage);
            stream.SendNext(gameCurUnit);
            stream.SendNext(gameWaitText.text);
            stream.SendNext(gameStageText.text);
        }
        else {
            this.gameStage = (int)stream.ReceiveNext();
            this.gameCurUnit = (int)stream.ReceiveNext();
            this.gameWaitText.text = (string)stream.ReceiveNext();
            this.gameStageText.text = (string)stream.ReceiveNext();
        }
    }
}
