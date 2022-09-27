using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
/*
->추가 장갑 : 최대 체력이 1증가합니다.
->화력 유지 : 차탄 장전까지 15% 빨라집니다.
->퀵드로우 : 재장전 속도가 10% 빨라집니다.
->플립 조절 : 좌우 이동속도가 15% 빨라집니다.
->신형 엔진 : 상하 이동속도가 10% 빨라집니다.
->추가 탄창 : 탄약이 2발(기관포),15발(기관총) 추가 지급됩니다.
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

        if (statRandom == 0) {
            firstText.text = "추가 장갑 : 최대 체력이 1증가합니다.";
            secondText.text = "화력 유지 : 차탄 장전까지 15% 빨라집니다.";
        }
        else if (statRandom == 1) {
            firstText.text = "퀵드로우 : 재장전 속도가 10% 빨라집니다.";
            secondText.text = "플립 조절 : 좌우 이동속도가 15% 빨라집니다.";
        }
        else if (statRandom == 2) {
            firstText.text = "부스터 팩 : 상하 이동속도가 10% 빨라집니다.";
            secondText.text = "추가 탄창 : 탄약이 2발(기관포),15발(기관총) 추가 지급됩니다.";
        }
        else if (statRandom == 3) {
            firstText.text = "추가 장갑 : 최대 체력이 1증가합니다.";
            secondText.text = "추가 탄창 : 탄약이 2발(기관포),15발(기관총) 추가 지급됩니다.";
        }
        else if (statRandom == 4) {
            firstText.text = "부스터 팩 : 상하 이동속도가 10% 빨라집니다.";
            secondText.text = "플립 조절 : 좌우 이동속도가 15% 빨라집니다.";
        }
        else if (statRandom == 5) {
            firstText.text = "화력 유지 : 차탄 장전까지 15% 빨라집니다.";
            secondText.text = "퀵드로우 : 재장전 속도가 10% 빨라집니다.";
        }
    }
}
