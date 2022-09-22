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
            Destroy(gameObject);
        }
    }

    void ReturnSprite() {
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "WallBullet") {
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Bullet") {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.damage);
        }
        else if (collision.tag == "Player") {
            collision.GetComponent<PlayerAir>().Hit();
            photonV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);
}
