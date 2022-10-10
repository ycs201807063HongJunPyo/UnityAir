using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Cloud : MonoBehaviourPunCallbacks {
    public float speed;
    public PhotonView photonV;
    Rigidbody2D rigid;

    void Awake() {

        rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.down * speed;
    }


    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "WallBullet") {
            photonV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
       
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(speed);
        }
        else {
            this.speed = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);
}
