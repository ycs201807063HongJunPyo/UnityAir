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

    // Update is called once per frame
    void Update()
    {
        Move();
        
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
