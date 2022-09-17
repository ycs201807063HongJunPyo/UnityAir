using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Bullet : MonoBehaviourPunCallbacks {

    public PhotonView photonV;
    int dir;

    void Update() {
        transform.Translate(Vector3.up * 20 * Time.deltaTime * dir);
    }

    [PunRPC]
    void DirRPC(int dir) => this.dir = dir;

    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "WallBullet") {
            Destroy(gameObject);
        }

        if (!photonV.IsMine && other.tag == "Player" && other.GetComponent<PhotonView>().IsMine) {
            other.GetComponent<PlayerAir>().Hit();
            photonV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        
    }
    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

}
