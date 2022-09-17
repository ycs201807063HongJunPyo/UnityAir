using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerAir : MonoBehaviourPunCallbacks, IPunObservable
{
    

    public float maxSpeed;
    public float currentSpeed;
    public bool isWallTop;
    public bool isWallRight;
    public bool isWallLeft;
    public bool isWallBottom;

    public PhotonView photonV;
    public Text nickname;

    public GameObject bulletObjA;
    public GameObject bulletObjB;


    public int bulletCount;
    public int hp;
    public Text hpText;

    public float maxBulletDelay;
    public float curBulletDelay;
    public float maxShotDelay;
    public float curShotDelay;
    // Update is called once per frame

    void Awake() {
        nickname.text = photonV.IsMine ? PhotonNetwork.NickName : photonV.Owner.NickName;
       
    }
    void Start() {
        hp = 5;
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
            Vector3 nextPos = new Vector3(h, v, 0) * currentSpeed * Time.deltaTime;

            transform.position = currentPos + nextPos;
        }
    }

    public void Fire() {
        if (Input.GetButton("Fire1")&& bulletCount < 20 && curBulletDelay > maxBulletDelay && photonV.IsMine) {
            
            PhotonNetwork.Instantiate("PlayerBullet A", transform.position, transform.rotation).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, 1);
            
            bulletCount++;
            curBulletDelay = 0;
        }
        
    }
    public void Reload() {
        if (photonV.IsMine) {
            curBulletDelay += Time.deltaTime;
            if (bulletCount >= 20) {
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
            hpText.text = (string)stream.ReceiveNext();
        }
    }
}
