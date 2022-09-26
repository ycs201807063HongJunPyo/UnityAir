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

public class PlayerAir : MonoBehaviourPunCallbacks, IPunObservable
{
    public EAirType airType;


    public float vSpeed; //상하
    public float hSpeed; //좌우

    public bool isWallTop;
    public bool isWallRight;
    public bool isWallLeft;
    public bool isWallBottom;

    private bool statCheck;

    public PhotonView photonV;
    public Text nickname;

    public GameObject bulletObjA;
    public GameObject bulletObjB;


    public int bulletCount;
    public int maxBulletCount;
    public int maxHp;
    public int hp;
    public Text hpText;
    public int apAmmo;

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
        }
        apAmmo = 0;
        statCheck = false;
        hpText.text = hp.ToString();
    }

    void Update()
    {
        Move();
        Fire();
        Reload();
        if (photonV.IsMine && statCheck == false) {
            GameObject.Find("Canvas").transform.Find("Unit Stat Panel").transform.Find("FirstStatButton").GetComponent<Button>().onClick.AddListener(() => this.OnClickButtonFirst(statCheck));
            GameObject.Find("Canvas").transform.Find("Unit Stat Panel").transform.Find("SecondStatButton").GetComponent<Button>().onClick.AddListener(() => this.OnClickButtonSecond(statCheck));
        }
    }
    void Move() {
        if (photonV.IsMine) {
            float h = Input.GetAxisRaw("Horizontal");
            if ((h == 1 && isWallRight) || (h == -1 && isWallLeft)) {
                h = 0;
            }
            float v = Input.GetAxisRaw("Vertical");
            if ((v == 1 && isWallTop) || (v == -1 && isWallBottom)) {
                v = 0;
            }
            Vector3 currentPos = transform.position;
            Vector3 nextPos = new Vector3(h*hSpeed, v*vSpeed, 0) * Time.deltaTime;

            transform.position = currentPos + nextPos;
        }
    }

    public void Fire() {
        if (Input.GetButton("Fire1")&& bulletCount < maxBulletCount && curBulletDelay > maxBulletDelay && photonV.IsMine) {
            if (airType == EAirType.LightFighter) {
                bulletObjA = PhotonNetwork.Instantiate("PlayerBullet A", transform.position + Vector3.right * 0.2f, transform.rotation);
                bulletObjA.GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
                if(apAmmo >= 1) {
                    bulletObjA.GetComponent<Bullet>().damage--;
                    bulletObjA.GetComponent<Bullet>().bulletHitCount += apAmmo;
                }
                bulletObjA = PhotonNetwork.Instantiate("PlayerBullet A", transform.position + Vector3.left * 0.2f, transform.rotation);
                bulletObjA.GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
                if (apAmmo >= 1) {
                    bulletObjA.GetComponent<Bullet>().damage--;
                    bulletObjA.GetComponent<Bullet>().bulletHitCount += apAmmo;
                    Debug.Log(bulletObjA.GetComponent<Bullet>().bulletHitCount);
                }
            }
            else if (airType == EAirType.JetFighter) {
                bulletObjB = PhotonNetwork.Instantiate("PlayerBullet B", transform.position + Vector3.right * 0.25f, transform.rotation);
                bulletObjB.GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
                if (apAmmo >= 1) {
                    bulletObjB.GetComponent<Bullet>().damage--;
                    bulletObjB.GetComponent<Bullet>().bulletHitCount += apAmmo;
                }
                bulletObjB = PhotonNetwork.Instantiate("PlayerBullet B", transform.position + Vector3.left * 0.25f, transform.rotation);
                bulletObjB.GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
                if (apAmmo >= 1) {
                    bulletObjB.GetComponent<Bullet>().damage--;
                    bulletObjB.GetComponent<Bullet>().bulletHitCount += apAmmo;
                    Debug.Log(bulletObjA.GetComponent<Bullet>().bulletHitCount);
                }
            }
            bulletCount++;
            curBulletDelay = 0;
        }
        
    }
    public void Reload() {
        if (photonV.IsMine) {
            curBulletDelay += Time.deltaTime;
            if (bulletCount >= maxBulletCount) {
                curShotDelay += Time.deltaTime;
                if (maxShotDelay < curShotDelay) {
                    curShotDelay = 0;
                    bulletCount = 0;
                }
            }
        }
    }

    public void Hit() {
        hp--;
        hpText.text = hp.ToString();
        if(hp <= 0) {
            photonV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Wall") {
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
        if(other.gameObject.tag == "StatPoint") {
            GameObject.Find("Canvas").transform.Find("Unit Stat Panel").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("Unit Stat Panel").gameObject.GetComponent<PlayerStat>().OpenStat();
            Destroy(other.gameObject);
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
                maxHp += 5;
                hp += 5;
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
                maxHp += 5;
                hp += 5;
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
                apAmmo++;
            }
            else if (select == 3) {
                apAmmo++;
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

    public void OnSelectButton() {
        
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(hpText.text);
        }
        else {
            this.hpText.text = (string)stream.ReceiveNext();
        }
    }
}
