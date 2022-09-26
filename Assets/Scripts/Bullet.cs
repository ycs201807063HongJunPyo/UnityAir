using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Bullet : MonoBehaviourPunCallbacks {

    public PhotonView photonV;
    int dir;
    public int damage;
    public int bulletHitCount;
    public GameObject bulletType;

    void Update() {
        transform.Translate(Vector3.up * 20 * Time.deltaTime * dir);
    }

    [PunRPC]
    void DirRPC(int dir) => this.dir = dir;

    void OnTriggerEnter2D(Collider2D other) {
        
        if (other.gameObject.tag == "WallBullet") {
            photonV.RPC("DestroyRPC", RpcTarget.All);
        }

        if (!photonV.IsMine && other.tag == "Player" && other.GetComponent<PhotonView>().IsMine) {
            other.GetComponent<PlayerAir>().Hit();
            photonV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        if(other.tag == "Enemy") {
            bulletHitCount--;
        }
        
        if (bulletHitCount <= 0) {
            photonV.RPC("DestroyRPC", RpcTarget.All);
        }
    }
    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

}
