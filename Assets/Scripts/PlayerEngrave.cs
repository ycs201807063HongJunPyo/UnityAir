﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

//0 : 부품 회수
//1 : 시전 향상
//2 : 개량 탄창
public class PlayerEngrave : MonoBehaviourPunCallbacks {
    public static PlayerEngrave peInstance;
    public int engraveRandom;
    public GameObject engravePanel;
    [SerializeField]
    private Text engraveText;

    void Start() {
        engraveRandom = 0;
    }
    public void OnEngrave() {
        engraveRandom = Random.Range(0, 2);

        if (engraveRandom == 0) {
            engraveText.text = "라운드 시작시 체력이 추가로 회복됩니다.";
        }
        else if (engraveRandom == 1) {
            engraveText.text = "스킬 재사용 시간이 20% 감소합니다.";
        }
        else if (engraveRandom == 2) {
            engraveText.text = "차탄 장전시간이 15% 감소합니다.";
        }
    }
}
