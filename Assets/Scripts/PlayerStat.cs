using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
/*
->추가 장갑 : 최대 체력이 1,2,3,4,6증가합니다.
->화력 유지 : 차탄 장전까지 5,10,15,20,30% 빨라집니다.
->퀵드로우 : 재장전 속도가 10,15,20,25,35% 빨라집니다.
->플립 조절 : 좌우 이동속도가 20,30,40,50,70% 빨라집니다.
->신형 엔진 : 상하 이동속도가 5,15,25,35,45% 빨라집니다.
->관통탄 : 공격력이 1감소하지만 탄이 적을 1,2,3,4,6회 관통합니다.
 */


public class PlayerStat : MonoBehaviourPunCallbacks {
    public static PlayerStat psInstance;
    public int statRandom;
    public GameObject statSelectPanel;


    [SerializeField]
    private Text firstText;
    //[SerializeField]
    //private Image firstImage;

    [SerializeField]
    private Text secondText;
    //[SerializeField]
    //private Image secondImage;


    void Awake() {
        statRandom = 0;
        psInstance = this;

    }
    public void OpenStat() {
        //statSelectPanel.SetActive(true);
        statRandom = Random.Range(0, 6);

        if(statRandom == 0) {
            firstText.text = "추가 장갑 : 최대 체력이 1,2,3,4,6증가합니다.";
            secondText.text = "화력 유지 : 차탄 장전까지 5,10,15,20,30% 빨라집니다.";
        }
        else if (statRandom == 1) {
            firstText.text = "퀵드로우 : 재장전 속도가 10,15,20,25,35% 빨라집니다.";
            secondText.text = "플립 조절 : 좌우 이동속도가 20,30,40,50,70% 빨라집니다.";
        }
        else if (statRandom == 2) {
            firstText.text = "부스터 팩 : 상하 이동속도가 5,15,25,35,45% 빨라집니다.";
            secondText.text = "관통탄 : 공격력이 1감소하지만 탄이 적을 1,2,3,4,6회 관통합니다.";
        }
        else if (statRandom == 3) {
            firstText.text = "추가 장갑 : 최대 체력이 1,2,3,4,6증가합니다.";
            secondText.text = "관통탄 : 공격력이 1감소하지만 탄이 적을 1,2,3,4,6회 관통합니다.";
        }
        else if (statRandom == 4) {
            firstText.text = "부스터 팩 : 상하 이동속도가 5,15,25,35,45% 빨라집니다.";
            secondText.text = "플립 조절 : 좌우 이동속도가 20,30,40,50,70% 빨라집니다.";
        }
        else if (statRandom == 5) {
            firstText.text = "화력 유지 : 차탄 장전까지 5,10,15,20,30% 빨라집니다.";
            secondText.text = "퀵드로우 : 재장전 속도가 10,15,20,25,35% 빨라집니다.";
        }
    }
    public void FisrtCloseStat() {
        statSelectPanel.SetActive(false);
    }
    public void SecondCloseStat() {
        statSelectPanel.SetActive(false);
    }
}
