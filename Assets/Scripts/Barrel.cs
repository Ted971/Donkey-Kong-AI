using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    private Rigidbody2D body;
    private Collider2D circle;
    private Collider2D[] results;
    private Vector2 dir;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public LayerMask ladderLayer;
    private float speed = 3.5f;
    private bool down;
    private SpriteRenderer spR;
    public Sprite[] rollSprites;
    private int spriteIndex;

    private void Awake(){
        spR = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        circle = GetComponent<Collider2D>();
        results = new Collider2D[3];
        down = false;
    }

    private void OnEnable(){
        InvokeRepeating(nameof(AnimationSprites), 1f/8f, 1f/8f);
    }

    private void OnDisable(){
        CancelInvoke();
    }

    private bool IsGrounded(){

        float x1 = transform.position.x;
        float y1 = transform.position.y;
        float z1 =transform.position.z; 
        Vector3 ray1Position;

        ray1Position.x = x1;
        ray1Position.y = y1;
        ray1Position.z = z1;

        /*float x2 = transform.position.x;
        float y2 = transform.position.y - 0.35f;
        float z2 =transform.position.z; 
        Vector3 ray2Position;

        ray2Position.x = x2;
        ray2Position.y = y2;
        ray2Position.z = z2;*/


        //Debug.DrawRay(ray1Position, transform.TransformDirection(Vector2.down) * 0.4f, Color.green);
        //Debug.DrawRay(ray2Position, transform.TransformDirection(Vector2.down) * 0.3f, Color.green);

        RaycastHit2D hitR = Physics2D.Raycast(ray1Position, transform.TransformDirection(Vector2.down), 0.4f, groundLayer);
        //RaycastHit2D hitL = Physics2D.Raycast(ray2Position, transform.TransformDirection(Vector2.down), 0.3f, ladderLayer);

        /*if(hitL.collider != null){
            ClimbDown();
        }*/

        if(hitR.collider != null /*|| hitL.collider != null*/){
            //Debug.Log("hit : "+ hitR.collider.name + "et : "+ hitL.collider.name);
            return true;
        }
        return false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("LadderTop"))
        {
            ClimbDown();
            
        } else if(collision.gameObject.CompareTag("LadderBottom")) {
            if(down){
                circle.isTrigger = false;
                down = false;
                speed *= -1;
            }
        }
      } 

    // Update is called once per frame
    void Update()
    {
        if(body.IsTouchingLayers(wallLayer)){
            speed *= -1;
        }
        
            if(IsGrounded() ){
                if(down){
                    circle.isTrigger = true;
                    dir.x = -0f;
                    dir.y = -2f;
                    //Debug.Log(down);
                } else {
                dir.x = speed;
                dir.y = 0f;
                }
            } else if(down){
                    circle.isTrigger = true;
                    dir.x = -0f;
                    dir.y = -2f;
                    //Debug.Log(down);
                } else {
                dir.x = speed*0.5f;
                dir.y = -2f;
            } 

        if(transform.position.y < -4){
            CancelInvoke();
            Destroy(gameObject);
        }
            

    }

    private bool ClimbDown(){
        Random.InitState(System.DateTime.Now.Millisecond);
        float tirage = Random.Range(0f,0.5f);
        tirage = float.Parse(tirage.ToString("0.#"));
        float tir = Random.value;
        tir = float.Parse(tir.ToString("0.#"));
        //Debug.Log(tirage + " tir :" + tir);
        if(tirage == tir){
            //Debug.Log("test");
            down = true;
            return down;
        }
        return down;
    }

    public void Restart(){
        CancelInvoke();
        Destroy(gameObject);
    }

    private void FixedUpdate(){
        body.MovePosition(body.position + dir * Time.fixedDeltaTime);
    }

    private void AnimationSprites() {
        int sprites = rollSprites.Length; 
            if(dir.x != 0f){
                spriteIndex++; 
                    if (spriteIndex >= sprites){
                        spriteIndex = 0;
                    }
                        spR.sprite = rollSprites[spriteIndex];
                }  else {
                    
                }
                
    }

    public float GetSpeedX(){
        return this.dir.x;
    }

    public float GetSpeedY(){
        return this.dir.y;
    }
}
