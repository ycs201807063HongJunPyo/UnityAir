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


    public float vSpeed; //상하
    public float hSpeed; //좌우

    public bool isWallTop;
    public bool isWallRight;
    public bool isWallLeft;
    public bool isWallBottom;

    public bool statCheck;
    public bool isReload;

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
    public int skillTimer;
    public int maxSkillTimer;
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
        }
        skillTimer = maxSkillTimer;
        statCheck = false;
        isReload = false;
        isSkill = false;
        skillText = GameObject.Find("Canvas").transform.Find("Main Image").transform.Find("SkillText").GetComponent<Text>();
        StartCoroutine("SkillTimer", 1);
        isLife = true;
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
        if (photonV.IsMine && isReload == false && isLife)
            GameObject.Find("Canvas").transform.Find("Main Image").transform.Find("GameUI").transform.Find("ReloadButton").GetComponent<Button>().onClick.AddListener(() => this.OnClickButtonReload());
        if (photonV.IsMine && isSkill == true && isLife)
            GameObject.Find("Canvas").transform.Find("Main Image").transform.Find("GameUI").transform.Find("SkillButton").GetComponent<Button>().onClick.AddListener(() => this.OnClickButtonSkill());
    }
    void Move() {
        if (photonV.IsMine && isLife) {
            float h = Input.GetAxisRaw("Horizontal");
            if ((h == 1 && isWallRight) || (h == -1 && isWallLeft)) {
                h = 0;
            }
            float v = Input.GetAxisRaw("Vertical");
            if ((v == 1 && isWallTop) || (v == -1 && isWallBottom)) {
                v = 0;
            }
            Vector3 currentPos = transform.position;
            Vector3 nextPos = new Vector3(h * hSpeed, v * vSpeed, 0) * Time.deltaTime;

            transform.position = currentPos + nextPos;
        }
    }

    public void Fire() {
        if (Input.GetButton("Fire1") && bulletCount < maxBulletCount && curBulletDelay > maxBulletDelay && photonV.IsMine && isLife) {
            if (airType == EAirType.LightFighter) {
                PhotonNetwork.Instantiate("PlayerBullet A", transform.position + Vector3.right * 0.2f, transform.rotation).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
                PhotonNetwork.Instantiate("PlayerBullet A", transform.position + Vector3.left * 0.2f, transform.rotation).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
            }
            else if (airType == EAirType.JetFighter) {
                PhotonNetwork.Instantiate("PlayerBullet B", transform.position + Vector3.right * 0.25f, transform.rotation).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
                PhotonNetwork.Instantiate("PlayerBullet B", transform.position + Vector3.left * 0.25f, transform.rotation).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
            }
            bulletCount++;
            curBulletDelay = 0;
        }

    }
    public void Reload() {
        if (photonV.IsMine && isLife) {
            if (skillING && airType == EAirType.JetFighter) {
                curBulletDelay += Time.deltaTime;
            }
            curBulletDelay += Time.deltaTime;
            if (bulletCount >= maxBulletCount) {
                isReload = true;
                if (skillING && airType == EAirType.JetFighter) {
                    curShotDelay += Time.deltaTime;
                }
                curShotDelay += Time.deltaTime;
                if (maxShotDelay < curShotDelay) {
                    isReload = false;
                    curShotDelay = 0;
                    bulletCount = 0;
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
            if (maxHp > hp)
                hp++;
            hpText.text = hp.ToString();
            Destroy(other.gameObject);
            photonV.RPC("StatRPC", RpcTarget.All);
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
    [PunRPC]
    public void StatRPC() {
        statCheck = false;
    }
    IEnumerator SkillTimer(float delayTime) {
        if (photonV.IsMine) {
            yield return new WaitForSeconds(delayTime);
            skillTimer--;
            if (skillTimer >= 0) {
                skillText.text = skillTimer.ToString();
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