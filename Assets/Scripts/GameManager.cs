using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    public static GameManager gmInstance;

    public GameObject[] enemyObjs;
    public Transform[] spawnPoints;

    public bool gameStart;
    public float maxDelay;
    public float curDelay;

    void Update() {
        if (gameStart == true) {
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
    }

    void SpawnEnemy() {
        int ranEnemy = Random.Range(0, 3);
        int ranPoint = Random.Range(0, 5);
        PhotonNetwork.Instantiate(enemyObjs[ranEnemy].name, spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
    }
}
