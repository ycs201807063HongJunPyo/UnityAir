using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAir : MonoBehaviour
{
    public float maxSpeed;
    public float currentSpeed;
    public bool isWallTop;
    public bool isWallRight;
    public bool isWallLeft;
    public bool isWallBottom;

    public GameObject bulletObjA;
    public GameObject bulletObjB;


    public int bulletCount;
    public float maxBulletDelay;
    public float curBulletDelay;
    public float maxShotDelay;
    public float curShotDelay;
    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
        Reload();
    }
    public void Move() {
        float h = Input.GetAxisRaw("Horizontal");
        if((h==1 && isWallRight) || (h==-1 && isWallLeft)) {
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
    public void Fire() {
        if (Input.GetButton("Fire1")&& bulletCount < 20 && curBulletDelay > maxBulletDelay) {
            GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);
            Rigidbody2D rig = bullet.GetComponent<Rigidbody2D>();
            rig.AddForce(Vector2.up * 20, ForceMode2D.Impulse);
            bulletCount++;
            curBulletDelay = 0;
        }
        
    }
    public void Reload() {
        curBulletDelay += Time.deltaTime;
        if (bulletCount >= 20) {
            curShotDelay += Time.deltaTime;
            if (maxShotDelay < curShotDelay) {
                curShotDelay = 0;
                bulletCount = 0;
            }
        }
    }
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
}
