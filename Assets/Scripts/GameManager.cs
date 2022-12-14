using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviourPun, IPunObservable {
    public static GameManager gmInstance;

    [SerializeField]
    private Text gameWaitText;
    private int waitCount = 9;

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
    }

    void Update() {
        
        if (gameStart == true && gameCurUnit > 0) {
            if (!PhotonNetwork.IsMasterClient) {
                return;
            }
            curDelay += Time.deltaTime;
            if (curDelay > maxDelay) {
                SpawnEnemy();
                maxDelay = Random.Range(0.3f, 0.8f);
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
        if (gameStage <= 4) {
            ranEnemy = Random.Range(0, 1);
        }
        else if (gameStage <= 10) {
            ranEnemy = Random.Range(0, 2);
        }
        else {
            ranEnemy = Random.Range(0, 3);
        }
        int ranPoint = Random.Range(0, 5);
        int cloudPoint = Random.Range(0, 8);
        if (cloudPoint <= 6) {
            int ranCloud;
            ranCloud = Random.Range(0, 2);
            PhotonNetwork.Instantiate(cloudObjs[ranCloud].name, spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
        }
        PhotonNetwork.Instantiate(enemyObjs[ranEnemy].name, spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
        //2배
        if (gameStage >= 10) {
            int ranPointTwo = ranPoint + 1;
            if(ranPointTwo == 5) {
                ranPointTwo = 0;
            }
            PhotonNetwork.Instantiate(enemyObjs[ranEnemy].name, spawnPoints[ranPointTwo].position, spawnPoints[ranPointTwo].rotation);
        }
        gameCurUnit--;
        gameStageText.text = "현재 스테이지 : " + gameStage.ToString() + "\n 남은 적 공세 : " + gameCurUnit;
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
            waitCount = 9;
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
        gameStageText.text = "현재 스테이지 : " + gameStage.ToString();
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

    //게임 종료
    public void GameQuit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
