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

    public PhotonView photonV;
    public Text nickname;

    public GameObject bulletObjA;
    public GameObject bulletObjB;


    public int bulletCount;
    public int maxBulletCount;
    public int hp;
    public Text hpText;

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
            hp = 7;
            vSpeed = 3f;
            hSpeed = 3f;
            maxBulletCount = 250;
            maxBulletDelay = 0.15f;
            maxShotDelay = 3f;
        }
        else if (RoomData.airforceCount == 1) {
            airType = EAirType.JetFighter;
            hp = 5;
            vSpeed = 5f;
            hSpeed = 2f;
            maxBulletCount = 20;
            maxBulletDelay = 0.6f;
            maxShotDelay = 7f;
        }

        hpText.text = hp.ToString();
    }

    void Update()
    {
        Move();
        Fire();
        Reload();
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
                PhotonNetwork.Instantiate("PlayerBullet A", transform.position+Vector3.right*0.2f, transform.rotation).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(hpText.text);
        }
        else {
            this.hpText.text = (string)stream.ReceiveNext();
        }
    }
}
