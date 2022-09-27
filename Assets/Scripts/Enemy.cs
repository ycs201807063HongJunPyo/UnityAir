using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Enemy : MonoBehaviourPunCallbacks {
    public float speed;
    public int enemyHp;

    public PhotonView photonV;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.down * speed;
    }

    void OnHit(int damage) {
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.8f);
        Invoke("ReturnSprite", 0.1f);
        enemyHp -= damage;
        if(enemyHp <= 0) {
            photonV.RPC("DestroyRPC", RpcTarget.AllBuffered);
            GameManager.gmInstance.gameDeadUnit++;
        }
        /*  적절한 위치 지정해주기
        if(GameManager.gmInstance.gameStage % 2 == 0) {
            enemyHp *= (int)1.2;
            speed *= 1.1f;
        }
        */
    }

    void ReturnSprite() {
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "WallBullet") {
            photonV.RPC("DestroyRPC", RpcTarget.AllBuffered);
            GameManager.gmInstance.gameDeadUnit++;
        }
        else if (collision.gameObject.tag == "Bullet" ) {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            int hitDamage = bullet.damage + (bullet.damage / 2);
            OnHit(Random.Range(bullet.damage, hitDamage));
        }
        else if (collision.tag == "Player") {
            collision.GetComponent<PlayerAir>().Hit();
            photonV.RPC("DestroyRPC", RpcTarget.AllBuffered);
            GameManager.gmInstance.gameDeadUnit++;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(enemyHp);
            stream.SendNext(speed);
        }
        else {
            this.enemyHp = (int)stream.ReceiveNext();
            this.speed = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);
}
