using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public enum EAirType {
    LightFighter,
    JetFighter
}

public class PlayerAir : MonoBehaviourPunCallbacks, IPunObservable {
    public EAirType airType;
    //조이스틱
    public VariableJoystick joy;

    public float vSpeed; //상하
    public float hSpeed; //좌우

    public bool isWallTop;
    public bool isWallRight;
    public bool isWallLeft;
    public bool isWallBottom;

    public bool statCheck;
    public bool isReload;

    //각인 관련
    public bool engraveCheck;
    public bool isPlusHp;  //부품 회수
    public bool isGunPack; //건팩

    public PhotonView photonV;
    public Text nickname;

    public GameObject bulletObjA;
    public GameObject bulletObjB;

    //체력 관련
    public int maxHp;
    public int hp;
    public Text hpText;
    public bool isLife;

    //스킬 관련
    public float skillTimer;
    public float maxSkillTimer;
    public Text skillText;
    public bool isSkill;//스킬 버튼 눌렀는지 확인
    public bool skillING;//스킬 사용중

    //총알 관련
    public int bulletCount;
    public int maxBulletCount;
    public float maxBulletDelay;//차탄 장전 시간
    public float curBulletDelay;//차탄 장전 속도
    public float maxShotDelay;//재장전 시간
    public float curShotDelay;//재장전 속도
    public int fireCount;//버튼 클릭때 몇번 사격하는지
    public Text bulletText;
    public bool isFire;//사격 진행 버튼
    public bool isFireDelay;//사격 진행 버튼

    // Update is called once per frame

    void Awake() {
        nickname.text = photonV.IsMine ? PhotonNetwork.NickName : photonV.Owner.NickName;

    }
    void Start() {
        if (RoomData.airforceCount == 0) {
            airType = EAirType.LightFighter;
            maxHp = 7;
            hp = maxHp;
            vSpeed = 3f;
            hSpeed = 3f;
            maxBulletCount = 250;
            maxBulletDelay = 0.15f;
            maxShotDelay = 3f;
            maxSkillTimer = 40;
            fireCount = 3;
        }
        else if (RoomData.airforceCount == 1) {
            airType = EAirType.JetFighter;
            maxHp = 5;
            hp = maxHp;
            vSpeed = 5f;
            hSpeed = 2f;
            maxBulletCount = 20;
            maxBulletDelay = 0.6f;
            maxShotDelay = 7f;
            maxSkillTimer = 60;
            fireCount = 1;
        }
        skillTimer = maxSkillTimer;
        statCheck = false;
        engraveCheck = false;
        isReload = false;
        isSkill = false;
        skillText = GameObject.Find("Canvas").transform.Find("Main Image").transform.Find("SkillText").GetComponent<Text>();
        bulletText = GameObject.Find("Canvas").transform.Find("Main Image").transform.Find("BulletText").GetComponent<Text>();
        bulletText.text = "탄약 : " + (maxBulletCount - bulletCount);
        StartCoroutine("SkillTimer", 1);
        isLife = true;
        isFire = false;
        isFireDelay = false;
        joy = GameObject.Find("Canvas").transform.Find("Main Image").transform.Find("GameUI").transform.Find("Variable Joystick 1").GetComponent<VariableJoystick>();
        hpText.text = hp.ToString();
    }

    void Update() {
        Move();
        Fire();
        Reload();
        if (photonV.IsMine && statCheck == false && isLife) {
            GameObject.Find("Canvas").transform.Find("Unit Stat Panel").transform.Find("FirstStatButton").GetComponent<Button>().onClick.AddListener(() => this.OnClickButtonFirst(statCheck));
            GameObject.Find("Canvas").transform.Find("Unit Stat Panel").transform.Find("SecondStatButton").GetComponent<Button>().onClick.AddListener(() => this.OnClickButtonSecond(statCheck));
        }
        if (photonV.IsMine && engraveCheck == false && isLife) {
            GameObject.Find("Canvas").transform.Find("Unit Engrave Panel").transform.Find("EngraveButton").GetComponent<Button>().onClick.AddListener(() => this.OnClickButtonEngrave(engraveCheck));
        }
        if (photonV.IsMine && isLife && isFireDelay == false) {
            GameObject.Find("Canvas").transform.Find("Main Image").transform.Find("GameUI").transform.Find("AttackButton").GetComponent<Button>().onClick.AddListener(() => this.OnClickButtonAttack());
            isFireDelay = true;
        }
        if (photonV.IsMine && isReload == false && isLife) {
            GameObject.Find("Canvas").transform.Find("Main Image").transform.Find("GameUI").transform.Find("ReloadButton").GetComponent<Button>().onClick.AddListener(() => this.OnClickButtonReload());
        }
        if (photonV.IsMine && isSkill == true && isLife) {
            GameObject.Find("Canvas").transform.Find("Main Image").transform.Find("GameUI").transform.Find("SkillButton").GetComponent<Button>().onClick.AddListener(() => this.OnClickButtonSkill());
        }
    }
    void Move() {
        if (photonV.IsMine && isLife) {
            float h = (joy.Horizontal * -1);
            Debug.Log(h);
            if ((h > 0 && isWallRight) || (h < 0 && isWallLeft)) {
                h = 0;
            }
            float v = (joy.Vertical * -1);
            if ((v > 0 && isWallTop) || (v < 0 && isWallBottom)) {
                v = 0;
            }
            Vector3 currentPos = transform.position;
            Vector3 nextPos = new Vector3(h * hSpeed, v * vSpeed, 0) * Time.deltaTime;

            transform.position = currentPos + nextPos;
        }
    }

    public void OnClickButtonAttack() {
        if (isFireDelay) {
            if (isFire == true) {
                isFire = false;
            }
            else if (isFire == false) {
                isFire = true;
            }
        }
        isFireDelay = false;
    }

    public void Fire() {
        if (isFire) {
            if (bulletCount < maxBulletCount && curBulletDelay > maxBulletDelay && photonV.IsMine && isLife && isFire) {
                if (airType == EAirType.LightFighter) {
                    PhotonNetwork.Instantiate("PlayerBullet A", transform.position + Vector3.right * 0.2f, transform.rotation).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
                    PhotonNetwork.Instantiate("PlayerBullet A", transform.position + Vector3.left * 0.2f, transform.rotation).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
                    if (isGunPack) {
                        PhotonNetwork.Instantiate("PlayerBullet A", transform.position + Vector3.right * 0.1f, transform.rotation).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
                        PhotonNetwork.Instantiate("PlayerBullet A", transform.position + Vector3.left * 0.1f, transform.rotation).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
                    }
                }
                else if (airType == EAirType.JetFighter) {
                    PhotonNetwork.Instantiate("PlayerBullet B", transform.position + Vector3.right * 0.25f, transform.rotation).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
                    PhotonNetwork.Instantiate("PlayerBullet B", transform.position + Vector3.left * 0.25f, transform.rotation).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
                    if (isGunPack) {
                        PhotonNetwork.Instantiate("PlayerBullet B", transform.position, transform.rotation).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
                    }
                }
                bulletCount++;
                bulletText.text = "탄약 : " + (maxBulletCount - bulletCount);
                curBulletDelay = 0;
            }
        }
    }

    public void Reload() {
        if (photonV.IsMine && isLife) {
            if (skillING && airType == EAirType.JetFighter) {
                curBulletDelay += Time.deltaTime;
            }
            curBulletDelay += Time.deltaTime;
            if (bulletCount >= maxBulletCount) {
                bulletText.text = "장전중...";
                isReload = true;
                if (skillING && airType == EAirType.JetFighter) {
                    curShotDelay += Time.deltaTime;
                }
                curShotDelay += Time.deltaTime;
                if (maxShotDelay < curShotDelay) {
                    isReload = false;
                    curShotDelay = 0;
                    bulletCount = 0;
                    bulletText.text = "탄약 : " + (maxBulletCount - bulletCount);
                }
            }
        }
    }

    public void Hit() {
        if (isLife) {
            hp--;
            hpText.text = hp.ToString();
            if (hp <= 0) {
                tag = "DeadPlayer";
                isLife = false;
            }
        }
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Wall") {
            switch (other.gameObject.name) {
                case "top":
                    isWallTop = true;
                    break;
                case "right":
                    isWallRight = true;
                    break;
                case "left":
                    isWallLeft = true;
                    break;
                case "bottom":
                    isWallBottom = true;
                    break;
            }
        }
        if (other.gameObject.tag == "StatPoint") {
            GameObject.Find("Canvas").transform.Find("Unit Stat Panel").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("Unit Stat Panel").gameObject.GetComponent<PlayerStat>().OpenStat();
            isLife = true;
            tag = "Player";
            if (maxHp > hp) {
                hp++;
                if(maxHp > hp && isPlusHp) {
                    hp++;
                }
            }
            hpText.text = hp.ToString();
            Destroy(other.gameObject);
            photonV.RPC("StatRPC", RpcTarget.All);
        }
        
        else if (other.gameObject.tag == "EngravePoint") {
            GameObject.Find("Canvas").transform.Find("Unit Engrave Panel").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("Unit Engrave Panel").gameObject.GetComponent<PlayerEngrave>().OpenEngrave();
            isLife = true;
            tag = "Player";
            hp = maxHp;
            hpText.text = hp.ToString();
            Destroy(other.gameObject);
            photonV.RPC("EngraveRPC", RpcTarget.All);
        }
        
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Wall") {
            switch (other.gameObject.name) {
                case "top":
                    isWallTop = false;
                    break;
                case "right":
                    isWallRight = false;
                    break;
                case "left":
                    isWallLeft = false;
                    break;
                case "bottom":
                    isWallBottom = false;
                    break;
            }
        }
    }

    public void OnClickButtonFirst(bool isCheck) {
        if (isCheck == false) {
            statCheck = true;
            isCheck = true;
            int select = PlayerStat.psInstance.statRandom;
            if (select == 0) {
                maxHp += 1;
                hp += 1;
                Debug.Log("선택값 " + hp);
            }
            else if (select == 1) {
                maxShotDelay *= 0.9f;
                Debug.Log("선택값 " + maxShotDelay);
            }
            else if (select == 2) {
                vSpeed *= 1.1f;
                Debug.Log("선택값 " + vSpeed);
            }
            else if (select == 3) {
                maxHp += 1;
                hp += 1;
                Debug.Log("선택값 " + hp);
            }
            else if (select == 4) {
                vSpeed *= 1.1f;
                Debug.Log("선택값 " + vSpeed);
            }
            else if (select == 5) {
                maxBulletDelay *= 0.85f;
                Debug.Log("선택값 " + maxBulletDelay);
            }
            hpText.text = hp.ToString();
            GameObject.Find("Canvas").transform.Find("Unit Stat Panel").gameObject.SetActive(false);
        }
    }

    public void OnClickButtonSecond(bool isCheck) {
        if (isCheck == false) {
            statCheck = true;
            isCheck = true;
            int select = PlayerStat.psInstance.statRandom;
            if (select == 0) {
                maxBulletDelay *= 0.85f;
                Debug.Log("선택값 " + maxBulletDelay);
            }
            else if (select == 1) {
                hSpeed *= 1.15f;
                Debug.Log("선택값 " + hSpeed);
            }
            else if (select == 2) {
                if (airType == EAirType.LightFighter) {
                    maxBulletCount += 15;
                }
                else if(airType == EAirType.JetFighter) {
                    maxBulletCount += 2;
                }
            }
            else if (select == 3) {
                if (airType == EAirType.LightFighter) {
                    maxBulletCount += 15;
                }
                else if (airType == EAirType.JetFighter) {
                    maxBulletCount += 2;
                }
            }
            else if (select == 4) {
                hSpeed *= 1.15f;
                Debug.Log("선택값 " + hSpeed);
            }
            else if (select == 5) {
                maxShotDelay *= 0.9f;
                Debug.Log("선택값 " + maxShotDelay);
            }
            hpText.text = hp.ToString();
            GameObject.Find("Canvas").transform.Find("Unit Stat Panel").gameObject.SetActive(false);
        }
    }
    public void OnClickButtonReload() {
        if (isReload == false) {
            isReload = true;
            bulletCount = maxBulletCount;
        }
    }
    public void OnClickButtonSkill() {
        if (isSkill == true) {
            isSkill = false;
            skillTimer = maxSkillTimer;
            if(airType == EAirType.LightFighter) {
                if (photonV.IsMine) {
                    hp += 2;
                    if(maxHp < hp) {
                        hp = maxHp;
                    }
                }
                hpText.text = hp.ToString();
            }
            else {
                skillING = true;
                StartCoroutine("SkillINGTimer", 10);
            }
            StartCoroutine("SkillTimer", 1);
        }
    }
    
    public void OnClickButtonEngrave(bool isCheck) {
        if (isCheck == false) {
            Debug.Log("들어왓다");
            engraveCheck = true;
            isCheck = true;
            int select = PlayerEngrave.peInstance.engraveRandom;
            if (select == 0) {
                isPlusHp = true;
            }
            else if (select == 1) {
                maxSkillTimer *= 0.8f;
                skillTimer = maxSkillTimer;
            }
            else if (select == 2) {
                isGunPack = true;
            }
            hpText.text = hp.ToString();
            GameObject.Find("Canvas").transform.Find("Unit Engrave Panel").gameObject.SetActive(false);
        }
    }
    
    [PunRPC]
    public void StatRPC() {
        statCheck = false;
    }

    [PunRPC]
    public void EngraveRPC() {
        engraveCheck = false;
    }
    IEnumerator SkillTimer(float delayTime) {
        if (photonV.IsMine) {
            yield return new WaitForSeconds(delayTime);
            skillTimer--;
            if (skillTimer >= 0) {
                skillText.text = "스킬 쿨타임 : " + skillTimer.ToString();
                StartCoroutine("SkillTimer", 1);
            }
            else {
                skillText.text = "스킬 사용 가능";
                isSkill = true;
            }
        }
    }

    IEnumerator SkillINGTimer(float delayTime) {
        if (photonV.IsMine) {
            yield return new WaitForSeconds(delayTime);
            skillING = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(hpText.text);
            stream.SendNext(tag);
        }
        else {
            this.hpText.text = (string)stream.ReceiveNext();
            tag = (string)stream.ReceiveNext();
        }
    }
}