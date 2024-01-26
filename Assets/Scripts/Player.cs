using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer spr;
    public Sprite[] runSprites;
    private int spriteIndex;
    public Sprite[] climbSprites;
    private Vector2 dir;
    private Rigidbody2D rb;
    private Collider2D collide;
    private Collider2D[] results;
    public float moveSpeed = 1f;
    public float jumpStr = 1f;
    //private bool jumping;
    public LayerMask groundLayer;
    public LayerMask ladderLayer;
    public LayerMask barrelLayer;
    //private GameManagerScript gm;

    public MAIro network;
    private float[] input = new float[9];
    private bool gotHit = false;
    private int fell = 0;
    private bool climbed = false;
    private float highestPoint;
    private float wentDown;
    private int nbJump = 0;
    private int dodge;
    private List<GameObject> laddersClimbed;
    private bool win;
    private int coins;

    private void Awake(){
        //gm = FindObjectOfType<GameManagerScript>();
        spr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        collide = GetComponent<Collider2D>();
        results = new Collider2D[3]; 
        Physics2D.IgnoreLayerCollision(0, 0);
        highestPoint = transform.position.y;
       laddersClimbed = new List<GameObject>();
    }

    private void OnEnable(){
        InvokeRepeating(nameof(AnimationSprites), 1f/10f, 1f/10f);
    }

    private void OnDisable(){
        CancelInvoke();
    }

    private bool IsClimbing(){

        float x1 = transform.position.x;
        float y1 = transform.position.y - 0.47f;
        float z1 =transform.position.z; 
        Vector3 climbingRay;

        climbingRay.x = x1;
        climbingRay.y = y1;
        climbingRay.z = z1;

        Debug.DrawRay(climbingRay, transform.TransformDirection(Vector2.right) * 0.1f, Color.green);
        RaycastHit2D hitC = Physics2D.Raycast(climbingRay, transform.TransformDirection(Vector2.right), 0.1f, ladderLayer);

        if(hitC.collider != null){
            if(!laddersClimbed.Contains(hitC.collider.GameObject())){
                laddersClimbed.Add(hitC.collider.GameObject());
            }
            
            return true;
        }
        return false;

    }

    private void IsHitOnLadder(){

        float x1 = transform.position.x;
        float y1 = transform.position.y;
        float z1 =transform.position.z; 
        Vector3 rayLeft;

        rayLeft.x = x1;
        rayLeft.y = y1;
        rayLeft.z = z1;

        float x2 = transform.position.x;
        float y2 = transform.position.y;
        float z2 =transform.position.z; 
        Vector3 rayRight;

        rayRight.x = x1;
        rayRight.y = y1;
        rayRight.z = z1;

        Debug.DrawRay(rayRight, transform.TransformDirection(Vector2.right) * 0.3f, Color.green);
        Debug.DrawRay(rayLeft, transform.TransformDirection(Vector2.left) * 0.3f, Color.green);
        RaycastHit2D hitBR = Physics2D.Raycast(rayRight, transform.TransformDirection(Vector2.right), 0.3f, barrelLayer);
        RaycastHit2D hitBL = Physics2D.Raycast(rayLeft, transform.TransformDirection(Vector2.left), 0.3f, barrelLayer);

        if(hitBR.collider != null || hitBL.collider != null){  
            enabled = false;
            gameObject.SetActive(false);
        }
    }
    private bool IsGrounded(){

        float x1 = transform.position.x + 0.3f;
        float y1 = transform.position.y - 0.45f;
        float z1 =transform.position.z; 
        Vector3 ray1Position;

        ray1Position.x = x1;
        ray1Position.y = y1;
        ray1Position.z = z1;

        float x2 = transform.position.x - 0.3f;
        float y2 = transform.position.y - 0.45f;
        float z2 =transform.position.z; 
        Vector3 ray2Position;

        ray2Position.x = x2;
        ray2Position.y = y2;
        ray2Position.z = z2;

        float x3 = transform.position.x;
        float y3 = transform.position.y - 0.45f ;
        float z3 =transform.position.z; 
        Vector3 ray3Position;

        ray3Position.x = x3;
        ray3Position.y = y3;
        ray3Position.z = z3;

        Debug.DrawRay(ray1Position, transform.TransformDirection(Vector2.down) * 0.2f, Color.green);
        Debug.DrawRay(ray2Position, transform.TransformDirection(Vector2.down) * 0.2f, Color.green);
        Debug.DrawRay(ray3Position, transform.TransformDirection(Vector2.right) * 0.35f, Color.green);

        RaycastHit2D hitR = Physics2D.Raycast(ray1Position, transform.TransformDirection(Vector2.down), 0.2f, groundLayer);
        RaycastHit2D hitL = Physics2D.Raycast(ray2Position, transform.TransformDirection(Vector2.down), 0.2f, groundLayer);
        RaycastHit2D hitS = Physics2D.Raycast(ray3Position, transform.TransformDirection(Vector2.right), 0.35f, groundLayer);

        if(hitS.collider != null && !IsClimbing()){
            dir.x = moveSpeed;
            dir.y = 0.05f;
            
        }

        if(hitR.collider != null || hitL.collider != null){
            //jumping = false;
            return true;
        }
        return false;
    }
    

    private void Jump(){
        if (!IsGrounded()){
            dir += Physics2D.gravity * Time.deltaTime;
            return;
        } else{
            if(Input.GetButtonDown("Jump")){
                dir = Vector2.up * jumpStr;
            }else{
                dir += Physics2D.gravity * Time.deltaTime;
            }
        }
    }

    private void JumpBot(float output){
        if (!IsGrounded()){
            dir += Physics2D.gravity * Time.deltaTime;
            return;
        } else{
            if(output >= 0.80f){
                dir = Vector2.up * jumpStr;
                nbJump++;

            }else{
                dir += Physics2D.gravity * Time.deltaTime;
            }
        }
    }

    /*private void Update() {

        input[0] = this.transform.position.x;
        input[1] = this.transform.position.y;
        input[2] = IsGrounded() ? 1 : 0;
        input[3] = IsClimbing() ? 1 : 0;
        input[4] = NearestLadder().transform.position.x;
        input[5] = NearestBarrel().transform.position.x;
        input[6] = NearestBarrel().transform.position.y;
        input[7] = NearestBarrel().GetComponent<Barrel>().GetSpeedX();
        input[8] = NearestBarrel().GetComponent<Barrel>().GetSpeedY();
        //float marioPosInit = collide.transform.position.y;
        if(IsClimbing() && (Input.GetAxis("Vertical") > 0 || !IsGrounded() /*|| marioPosInit < collide.transform.position.y*//*)){
            IsHitOnLadder();
            collide.isTrigger=true;
            dir.y = Input.GetAxis("Vertical") * moveSpeed / 2;;
           /* marioPosInit = collide.transform.position.y;
            dir.y = Input.GetAxis("Vertical") * moveSpeed / 2;;
            int amount = Physics2D.OverlapBoxNonAlloc(transform.position, collide.bounds.size, 0f, results);
            for (int i = 0; i < amount; i++){
                if(results[i].transform.position.y > collide.transform.position.y){
                    Physics2D.IgnoreCollision(collide, results[i], isClimbing());
                } else if(!isClimbing()){
                    Physics2D.IgnoreCollision(collide, results[i], false);
                }
            }
*//*
        } else {
            collide.isTrigger=false;
            Jump();
        }

        dir.x = Input.GetAxis("Horizontal");
        dir.y = Mathf.Max(dir.y, -1f);

        

        if(dir.x > 0f ){
            transform.eulerAngles = Vector3.zero;
        }else if(dir.x < 0f){
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }*/

    private void FixedUpdate(){
        input[0] = this.transform.position.x;
        input[1] = this.transform.position.y;
        input[2] = IsGrounded() ? 1 : 0;
        input[3] = IsClimbing() ? 1 : 0;
        

    if (NearestLadder() != null && NearestBarrel() != null)
    {
        input[4] = NearestLadder().transform.position.x;
        input[5] = NearestBarrel().transform.position.x;
        input[6] = NearestBarrel().transform.position.y;
        input[7] = NearestBarrel().GetComponent<Barrel>().GetSpeedX();
        input[8] = NearestBarrel().GetComponent<Barrel>().GetSpeedY();
    }
    else
    {
        input[4] = 0f;
        input[5] = 0f;
        input[6] = 0f;
        input[7] = 0f;
        input[8] = 0f;
        return;
    }
        float[] output = network.FeedForward(input);

        if(IsClimbing() && (output[1] > 0 || !IsGrounded() /*|| marioPosInit < collide.transform.position.y*/)){
            if(transform.position.y >= highestPoint){
                highestPoint = transform.position.y;
            }else if (wentDown > transform.position.y) {
                wentDown = transform.position.y;
            }
            IsHitOnLadder();
            climbed = true;
            collide.isTrigger=true;
            dir.y = output[1] * moveSpeed / 2;;
            } else {
                if(transform.position.y >= highestPoint){
                    highestPoint = transform.position.y;
                }else if (wentDown > transform.position.y){
                    wentDown = highestPoint - transform.position.y;
                }
            collide.isTrigger=false;
            JumpBot(output[2]);
        }
        dir.x = output[0];
        dir.y = Mathf.Max(dir.y, -1f);

        if(dir.x > 0f ){
            transform.eulerAngles = Vector3.zero;
        }else if(dir.x < 0f){
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }

        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        
    }

    private void AnimationSprites() {
        int sprites = runSprites.Length; 
        int spritesC = climbSprites.Length;
        if (IsClimbing() && (Input.GetAxis("Vertical") > 0 || !IsGrounded())){
            //Debug.Log(spriteIndex);
            //Debug.Log(spritesC);
            spriteIndex ++;
            if (spriteIndex >= spritesC){
                        spriteIndex = 0;
                    }
            if(dir.y != 0){
                spr.sprite = climbSprites[spriteIndex];
            }
            
        } else {
            if(dir.x != 0f){
                spriteIndex++;
                    
                    if (spriteIndex >= sprites){
                        spriteIndex = 0;
                    }
                    if(!IsGrounded()) {
                        spr.sprite = runSprites[2];
                        spriteIndex = 0;
                    } else {
                        spr.sprite = runSprites[spriteIndex];
                    }
                }  else {
                    spr.sprite = runSprites[0];
                    spriteIndex = 0;
                }
                
            }
        }

        private void OnCollisionEnter2D(Collision2D collision){
            if(collision.gameObject.CompareTag("Barrel")){
                gotHit = true;
                enabled = false;
                gameObject.SetActive(false);
            }else if(collision.gameObject.CompareTag("Princess")){
                enabled = false;
                //gm.LevelComplete();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision){
            if(collision.gameObject.CompareTag("Hole") && !IsGrounded()){
                fell++;
            }
            if(collision.gameObject.CompareTag("Coin")){
                coins++;
                collision.gameObject.SetActive(false);
            }
            if(collision.gameObject.CompareTag("Barrel")){
                dodge++;
            }
            if(collision.gameObject.CompareTag("Respawn")){
                fell += 10;
                dodge = 0;
            }
            if(collision.gameObject.CompareTag("Princess")){
                win = true;
                enabled = false;
                gameObject.SetActive(false);
            }
        }

        private GameObject NearestBarrel(){
            GameObject[] barrels;
            barrels = GameObject.FindGameObjectsWithTag("Barrel");
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach(GameObject potentialTarget in barrels){
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if(dSqrToTarget < distance){
                    closest = potentialTarget;
                    distance = dSqrToTarget;
                }
            }
            //Vector3 dir = (closest.transform.position - transform.position).normalized;
            //Debug.DrawRay(transform.position, dir , Color.yellow);
            return closest;
        }

        private GameObject NearestLadder(){
            GameObject[] ladders;
            ladders = GameObject.FindGameObjectsWithTag("Ladder");
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach(GameObject potentialTarget in ladders){
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if(potentialTarget.transform.position.y < transform.position.y +2 && potentialTarget.transform.position.y > transform.position.y-1f && !potentialTarget.Equals(closest)){
                    if(dSqrToTarget < distance){
                    closest = potentialTarget;
                    distance = dSqrToTarget;
                    }
                }
            }
            Vector3 direc = (closest.transform.position - transform.position).normalized;
            Debug.DrawRay(transform.position, direc, Color.red);
            return closest;
        }

        public void UpdateFitness()
        {
            network.fitness = highestPoint * 10;//updates fitness of network for sorting
            if(wentDown != 0){
                if(wentDown < 0){
                    wentDown *= -1;
                }
                network.fitness -= wentDown;
            }
            if(gotHit && climbed){
                network.fitness -= 5 + (laddersClimbed.Count * 5);
            } else if(gotHit && !climbed){
                network.fitness -= 30;
            }
            if(fell > 0){
                network.fitness -= 5 * fell;
            }
            if(climbed){
                network.fitness += 10 * laddersClimbed.Count;
            }
            if(nbJump > 15){
                network.fitness -= nbJump;
            }
            if(dodge > 0){
                network.fitness += dodge * 0.5f;
            }
            if(coins > 0){
                network.fitness += coins * 10f;
            }
            if(win){
                network.fitness += 10000;
            }
            //Debug.Log("fitness"+network.fitness+" d :"+dodge+" c:"+climbed+" y :"+highestPoint+" fell :"+fell+" gothit :"+gotHit+" lost :"+wentDown);
        }

    }



