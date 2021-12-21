// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;

// public class Player : MonoBehaviour
// {
//     //Start() variables
//     Rigidbody2D rigid;
//     Animator anim;
//     Collider2D collider;
//     SpriteRenderer renderer;

//     //PSM
//     enum State {idle, running, jumping, falling, hunt, defend1, defend2, defend3, attack1, attack2, attack3, JAttack1, JAttack2, JAttack3, kick, holdIdle, holdMove, holdKick, push, wallHold, wallJump};
//     State state = State.idle;

//     //Ladder
//     // [HideInInspector] public bool canClimb = false;
//     // [HideInInspector] public bool bottomLadder = false;
//     // [HideInInspector] public bool topLadder = false;
//     // public TLadder ladder;
//     // float naturalGravity;
//     // [SerializeField] float climbSpeed = 3f;

//     //Inspector variables
//     [SerializeField] LayerMask ground;
//     [SerializeField] float hDirection;

//     [SerializeField] float wallDirection;
//     public float speed = 3f;//몬스터에 닿으면 이동속도가 느려짐
//     [SerializeField] float jumpForce = 15f;
//     [SerializeField] float huntForce = 8f;
//     [SerializeField] AudioSource cherry;
//     [SerializeField] AudioSource footstep;
//     [SerializeField] bool defending = false;
//     [SerializeField] bool attack = false;
//     public bool jumping = false;
//     [SerializeField] bool kicking = false;
//     public bool holding = false;
//     [SerializeField] bool downDefend = false;
//     [SerializeField] bool downAttack = false;

//     [SerializeField] bool isWall = false;
//     [SerializeField] float slidingSpeed;
//     [SerializeField] float wallJumpPower;
//     [SerializeField] bool isWallJump;

//     // [SerializeField] bool push = false;

//     public Transform attackPos;
//     public Vector2 attackSize;
//     public Transform defendPos;
//     public Vector2 defendSize;

//     public Transform wallChk;
//     public float wallchkDistance;
//     public LayerMask w_Layer;

//     //발차기 영역은 공격오브젝를 활용함
    
//     public Hold holder;

//     void Start()
//     {
//         rigid = GetComponent<Rigidbody2D>();
//         anim = GetComponent<Animator>();
//         collider = GetComponent<Collider2D>();
//         renderer = GetComponent<SpriteRenderer>();
//         rigid.drag = 0.2f;
//         rigid.gravityScale = 4;

//         collider.offset = new Vector2(0f, -0.19f);
//         CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//         temp.size = new Vector2(0.6f, 1.75f);
//         //TPermanentUI.perm.healthAmount.text = TPermanentUI.perm.health.ToString();
//         //naturalGravity = rigid.gravityScale;
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         // if(state == State.climb){
//         //     Climb();
//         // }
//         Debug.DrawRay(wallChk.position, Vector2.right * transform.localScale.x * wallchkDistance, new Color(0, 1, 0));
//         isWall = Physics2D.Raycast(wallChk.position, Vector2.right * transform.localScale.x, wallchkDistance, w_Layer);
//         if(isWall){
//             state = State.wallHold;
//         }

//         //방향전환
//         hDirection = Input.GetAxis("Horizontal");
//         //공격도중에 방향전환 금지
//         if(!attack){
//             if(hDirection < 0){
//                 transform.localScale = new Vector2(-1 ,1);
//             }
//             else if(hDirection > 0){
//                 transform.localScale = new Vector2(1 ,1);
//             }
//         } 

//         if(state != State.hunt){            
//             Defend();
//             Attack();
//             Kick();

//             if(!defending && !attack && !kicking){//방어중에는 이동 불가능
//                 Movement();
//             }
//         }
//         if(rigid.velocity.y < -0.1f && !holding){
//             JumpHitLay();
//         }
//         else if(rigid.velocity.y == 0 && !holding){
//             rigid.gravityScale = 4;
//         }

//         anim.SetInteger("state",(int)state);
//     }

//     void FixedUpdate()
//     {
//         if(isWall){
//             isWallJump = false;
//             rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * slidingSpeed);

//             if(Input.GetAxis("Jump") != 0){
//                 isWallJump = true;
//                 Invoke("FreezeX", 0.3f);
//                 rigid.velocity = new Vector2(-wallJumpPower * transform.localScale.x, 0.9f * wallJumpPower);
//                 transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
//             }
//         }
//     }

//     // void OnTriggerStay2D(Collider2D collision)
//     // {   
//     //     if(collision.gameObject.layer == LayerMask.NameToLayer("Wall")){
            
//     //         if(wallDirection == 1 && Input.GetKey(KeyCode.LeftArrow)){
//     //             state = State.running;
//     //             push = false;
//     //             return;
//     //         }
//     //         else if(wallDirection == -1 && Input.GetKey(KeyCode.RightArrow)){
//     //             state = State.running;
//     //             push = false;
//     //             return;
//     //         }


//     //         if(!jumping){
//     //             push = true;
//     //             if(Input.GetButton("Horizontal")){
//     //                 state = State.push;
//     //             }
//     //             else{
//     //                 state = State.idle;
//     //             }
//     //         }
//     //     }

//     // }

//     // void OnTriggerEnter2D(Collider2D collision)
//     // {
//     //     if(collision.gameObject.layer == LayerMask.NameToLayer("Wall")){
//     //         wallDirection = transform.localScale.x;
//     //     }

//     //     // if(collision.tag == "Coin"){ //체리 
//     //     //     cherry.Play();
//     //     //     Destroy(collision.gameObject);
//     //     //     //TPermanentUI.perm.cherries += 1;
//     //     //     //TPermanentUI.perm.cherryText.text = TPermanentUI.perm.cherries.ToString();
//     //     // }
//     //     // else if(collision.tag == "Powerup"){ 
//     //     //     Destroy(collision.gameObject);
//     //     //     jumpForce = 25;//점프 파워를 상승시킴
//     //     //     GetComponent<SpriteRenderer>().color = Color.yellow;
            
//     //     //     StartCoroutine(ResetPower());//10초뒤에 원래대로 돌아옴
//     //     // }
//     // }

//     // void OnTriggerExit2D(Collider2D collision)
//     // {
//     //     if(collision.gameObject.layer == LayerMask.NameToLayer("Wall")){
//     //         state = State.running;
//     //         push = false;
//     //     }
//     // }


//     void OnCollisionEnter2D(Collision2D other)
//     {   
//         if(downDefend || downAttack){
//             return;
//         }
//         // //적과 충돌
//         // if(other.gameObject.tag == "Enemy"){

//         //     //TEnemy enemy = other.gameObject.GetComponent<TEnemy>();

//         //     //공격을 당함
//         //     state = State.hunt;
//         //     StartCoroutine(HuntOff());
//         //     rigid.drag = 1f;//바닥에 미끄러지는거 방지
//         //     renderer.color = new Color(1, 1, 1, 0.5f);
//         //     this.gameObject.layer = 10;

//         //     if(other.gameObject.transform.position.x > transform.position.x){
//         //         rigid.velocity = new Vector2(-huntForce, huntForce);
//         //         transform.localScale = new Vector2(1 ,1);
//         //     }
//         //     else{
//         //         rigid.velocity = new Vector2(huntForce, huntForce);
//         //         transform.localScale = new Vector2(-1 ,1);
//         //     }
//         //     StartCoroutine(Invincibility());
//         // }
//         if(other.gameObject.tag == "Trap"){ //함정에 맞음
//             state = State.hunt;
//             rigid.drag = 1f;//바닥에 미끄러지는거 방지
//             renderer.color = new Color(1, 1, 1, 0.5f);
//             this.gameObject.layer = 10;
//             //HandleHealth();//체력 표시 업데이트 및 체력이 없을경우 다시시작
//             if(other.transform.position.x > transform.position.x){
//                 rigid.velocity = new Vector2(-huntForce, huntForce);
//                 transform.localScale = new Vector2(1 ,1);
//             }
//             else{
//                 rigid.velocity = new Vector2(huntForce, huntForce);
//                 transform.localScale = new Vector2(-1 ,1);
//             }
//             other.gameObject.GetComponent<EnemyBullet>().Stop();//화살을 멈춤

//             StartCoroutine(Invincibility());
//         }
//     }

//     // void HandleHealth(){ //애니메이션에서 함수를 실행함
        
//     //     TPermanentUI.perm.health -= 1;//체력 감소
//     //     TPermanentUI.perm.healthAmount.text = TPermanentUI.perm.health.ToString();

//     //     if(TPermanentUI.perm.health <= 0){
//     //         TPermanentUI.perm.health = 5;//체력 감소
//     //         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//     //     }
//     // }

//     void Movement()
//     {   

//         // //사다리 오르기
//         // if(canClimb && Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f){
//         //     state = State.climb;
//         //     rigid.constraints = RigidbodyConstraints2D.FreezePositionX | 
//         //     RigidbodyConstraints2D.FreezeRotation;

//         //     transform.position = new Vector3(ladder.transform.position.x,rigid.position.y);
//         //     rigid.gravityScale = 0f;
//         // }

//         //기둥 타기
//         if(holding){
//             collider.offset = new Vector2(0f, -0.2f);
//             CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//             temp.size = new Vector2(0.9f, 2.3f);

//             rigid.constraints = RigidbodyConstraints2D.FreezePositionY | //기둥에 메달리는게 설정함
//             RigidbodyConstraints2D.FreezeRotation;
//             rigid.drag = 30f;
//             transform.position = new Vector2(transform.position.x, holder.transform.position.y -1f);

//             hDirection = Input.GetAxis("Horizontal");

//             if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetButtonDown("Jump")){ // 기둥에서 내려옴
//                 collider.offset = new Vector2(0f, -0.19f);
//                 temp.size = new Vector2(0.6f, 1.75f);

//                 rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
//                 rigid.drag = 0.2f;
//                 holding = false;
//             }
//             else if(Mathf.Abs(hDirection) > 0.5f){//기둥에서 좌우로 이동함
//                 state = State.holdMove;
//                 rigid.velocity = new Vector2((speed * hDirection) / 3, rigid.velocity.y);
//             }
//             else{
//                 state = State.holdIdle;//기둥에서 움직임 없을때
//             }

//             if(!holding){
//                 rigid.drag = 0.2f;
//             }
//         }
//         else{
//             rigid.constraints = RigidbodyConstraints2D.FreezeRotation;//기둥에 메달리는거 해제함

//             //달리기
//             hDirection = Input.GetAxis("Horizontal");
//             if(!isWallJump){
//                 rigid.velocity = new Vector2(speed * hDirection, rigid.velocity.y);
//             }
    
//             //점프하기
//             if(Input.GetButtonDown("Jump") && (collider.IsTouchingLayers(ground))){
//                 if(!defending && !attack){
//                     jumping = true;
//                     state = State.jumping;
//                 }
//             }
//             //떨어짐
//             else if(rigid.velocity.y < -0.1f && !isWall){
//                 state = State.falling;
//             }

//             else if(!defending && !attack && !jumping && !kicking){          
                
//                 collider.offset = new Vector2(0f, -0.19f);
//                 CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//                 temp.size = new Vector2(0.6f, 1.75f);
                
//                 if(Mathf.Abs(rigid.velocity.x) > 0.2f){//달리기
//                     Debug.Log("test");
//                     state = State.running;
//                 }
//                 else{
//                     state = State.idle;
//                 }
//             }
//         }  
//     }
//     void Jump()
//     { 
//         rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
//     }

//     void Defend()
//     {
//         //방패 위치 변경
//         if(Input.GetKey(KeyCode.D)){

//             if(Input.GetKey(KeyCode.UpArrow)){//위 방어
//                 defendPos.localPosition = new Vector2(0.1f, 0.58f);
//                 defendSize = new Vector2(1.7f, 0.8f);

//                 collider.offset = new Vector2(0f, -0.3f);
//                 CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//                 temp.size = new Vector2(0.8f, 1.5f);

//                 state = State.defend2;
//             }
//             else if(Input.GetKey(KeyCode.DownArrow)){//아래 방어
//                 defendPos.localPosition = new Vector2(0f, -1.1f);
//                 defendSize = new Vector2(1.7f, 0.8f);
            
//                 collider.offset = new Vector2(0f, -0.43f);
//                 CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//                 temp.size = new Vector2(0.8f, 1.24f);

//                 state = State.defend3;
                
//                 if(jumping){

//                     rigid.gravityScale = 8;
//                     this.gameObject.layer = 10;                    
//                     downDefend = true;//아래 방패공격
//                 }
//             }
//             else{//좌우 방어
//                 defendPos.localPosition = new Vector2(0.7f, -0.1f);
//                 defendSize = new Vector2(0.8f, 2f);

//                 collider.offset = new Vector2(0.1f, -0.24f);
//                 CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//                 temp.size = new Vector2(0.8f, 1.65f);

//                 state = State.defend1;
//             }
//             defending = true;//방어 도중에 이동금지

//             Collider2D[] colliderDefend = Physics2D.OverlapBoxAll(defendPos.position, defendSize, 0);        
//             foreach (Collider2D collider in colliderDefend)
//             {   
//                 if(collider.tag == "Enemy"){
//                     Debug.Log("방어함");
                    
//                     if(collider.GetComponent<Enemy>().die == true){
//                         return;
//                     }

//                     //뒤로 밀리는 기능
//                     if(state == State.defend1){
//                         if(collider.transform.position.x > transform.position.x){
//                             rigid.velocity = new Vector2(-huntForce / 2, rigid.velocity.y);
//                         }
//                         else{
//                             rigid.velocity = new Vector2(huntForce / 2, rigid.velocity.y);
//                         }
//                     }
//                     else if(state == State.defend2){
//                         if(collider.transform.position.y > transform.position.y){
//                             rigid.velocity = new Vector2(rigid.velocity.x, -huntForce);
//                         }
//                     }
//                     else if(state == State.defend3){
//                         if(collider.transform.position.y < transform.position.y){
//                             rigid.velocity = new Vector2(rigid.velocity.x, huntForce);
                            
//                             if(jumping){
//                                 collider.GetComponent<Enemy>().TakeDamage(2);
//                             }
//                         }
//                     }
//                 }
//                 else if(collider.tag == "Trap"){
//                     collider.gameObject.GetComponent<EnemyBullet>().Stop();//화살을 멈춤
//                 }
//                 // else if(collider.tag == "Trap"){
//                 //     collider.GetComponent<EnemyBullet>().Stop();
//                 // }
//             }
//         }
//         //방패 해제
//         else if(state == State.defend3 && Input.GetButton("Horizontal") && rigid.velocity.y == 0){
//             collider.offset = new Vector2(0f, -0.19f);
//             CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//             temp.size = new Vector2(0.6f, 1.75f);
//             state = State.running;

//             attack = false;
//         }
//         else{
//             collider.offset = new Vector2(0f, -0.19f);
//             CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//             temp.size = new Vector2(0.6f, 1.75f);
            
//             defending = false;
//         }
//     }

//     void Attack()
//     {
//         if(kicking){
//             return;
//         }

//         //공격 위치 변경
//         if(Input.GetKey(KeyCode.A)){
//             attack = true;

//             if(Input.GetKey(KeyCode.UpArrow)){//위로 공격
//                 attackPos.localPosition = new Vector2(0.38f, 1.8f);
//                 attackSize = new Vector2(0.5f, 0.6f);
                
//                 if(jumping){                    
//                     collider.offset = new Vector2(0f, -0.19f);
//                     CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//                     temp.size = new Vector2(0.6f, 1.73f);

//                     state = State.JAttack2;
//                 }
//                 else{
//                     collider.offset = new Vector2(0f, -0.3f);
//                     CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//                     temp.size = new Vector2(0.6f, 1.5f);

//                     state = State.attack2;
//                 }
//             }
//             else if(Input.GetKey(KeyCode.DownArrow)){//아래로 공격

//                 if(jumping){
//                     Debug.Log("아래공격");
//                     attackPos.localPosition = new Vector2(0.12f, -1.25f);
//                     attackSize = new Vector2(0.5f, 0.6f);

//                     collider.offset = new Vector2(0f, -0.12f);
//                     CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//                     temp.size = new Vector2(0.6f, 1.9f);

//                     state = State.JAttack3;
                    
//                     rigid.gravityScale = 8;
//                     this.gameObject.layer = 10;
//                     downAttack = true;//공격도중에 이동 금지

//                 }
//                 else {
//                     attackPos.localPosition = new Vector2(0.12f, -0.8f);
//                     attackSize = new Vector2(0.5f, 0.6f);

//                     collider.offset = new Vector2(0f, -0.25f);
//                     CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//                     temp.size = new Vector2(0.6f, 1.6f);

//                     state = State.attack3;
//                 }
//             }
//             else{//좌우 공격
//                 attackPos.localPosition = new Vector2(1.8f, -0.13f);
//                 attackSize = new Vector2(0.6f, 0.5f);

//                 collider.offset = new Vector2(0.15f, -0.25f);
//                 CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//                 temp.size = new Vector2(0.9f, 1.6f);

//                 if(jumping){
//                     state = State.JAttack1;
//                 }
//                 else{
//                     state = State.attack1;
//                 }
//             }    
//         }
//         else{
//             if(state == State.JAttack3 && Input.GetButton("Horizontal") && rigid.velocity.y == 0){
//                 collider.offset = new Vector2(0f, -0.19f);
//                 CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//                 temp.size = new Vector2(0.6f, 1.75f);
//                 state = State.running;

//                 attack = false;
//             }
//             //공격 해제
//             else{
//                 collider.offset = new Vector2(0f, -0.19f);
//                 CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//                 temp.size = new Vector2(0.6f, 1.75f);
                    
//                 attack = false;
//             }
//         }
//     }

//     void Kick()
//     {
//         if(attack){
//             return;
//         }

//         if(Input.GetKey(KeyCode.S)){
//             state = State.kick;
//             attackPos.localPosition = new Vector2(0.9f, -0.25f);
//             attackSize = new Vector2(0.5f, 1.5f);

//             collider.offset = new Vector2(0.2f, 0f);
//             CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//             temp.size = new Vector2(0.8f, 2f);
//             kicking = true;
//         }
//         else{
//             collider.offset = new Vector2(0f, -0.19f);
//             CapsuleCollider2D temp = (CapsuleCollider2D)collider;
//             temp.size = new Vector2(0.6f, 1.75f);
//             kicking = false; 
//         }
//     }

//     //찌를때 몬스터가 데미지를 입음
//     public void EnemyDamaged()
//     {
//         Collider2D[] colliderAttack = Physics2D.OverlapBoxAll(attackPos.position, attackSize, 0);        
//         foreach (Collider2D collider in colliderAttack)
//         {   
//             if(collider.tag == "Enemy"){
//                 if(state == State.JAttack3){//창으로 낙하공격
//                     collider.GetComponent<Enemy>().TakeDamage(1);
//                 }
//                 else if(state == State.attack1 || state == State.attack2 || state == State.attack3 || state == State.JAttack1 || state == State.JAttack2){
//                     collider.GetComponent<Enemy>().TakeDamage(0);// 적 체력 감소
//                 }
//                 else if(state == State.kick){
//                     collider.GetComponent<Enemy>().TakeDamage(3);// 적 체력 감소
//                 }
//             }
//             //부셔지는 오브젝트에 적용됨
//             else if(collider.tag == "Destruction"){
//                 collider.GetComponent<Destruction>().ObjectDamaged();
//             }
//         }
//     }

//     public void PlayerDamaged(int direction)//플레이어가 적한테 공격을 당함
//     {
//         //공격을 당함
//         state = State.hunt;
//         StartCoroutine(HuntOff());
//         rigid.drag = 1f;//바닥에 미끄러지는거 방지
//         renderer.color = new Color(1, 1, 1, 0.5f);
//         this.gameObject.layer = 10;

//         rigid.velocity = new Vector2(huntForce * direction, huntForce);
//         transform.localScale = new Vector2(1 *(-direction) ,1);

//         StartCoroutine(Invincibility());
//     }

//     // void Hold()
//     // {   
//     //     if(Input.GetButtonDown("Jump")){ // 사다리에서 점프해서 내리는 기능
//     //         rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
//     //         canClimb = false;
//     //         rigid.gravityScale = naturalGravity;
//     //         anim.speed = 1f;
//     //         Jump();
//     //         return;
//     //     }

//     //     //방향키 좌우로 움직는 기능
//     //     float vDirection = Input.GetAxis("Horizontal");
//     //     else if(vDirection > 0.1f){
//     //         rigid.velocity = new Vector2(0f, (vDirection * speed)/3);
//     //         state = State.holdMove;
//     //     }
//     //     else if(vDirection < -0.1f){
//     //         rigid.velocity = new Vector2(0f, (vDirection * speed)/3);
//     //         state = State.holdMove;
//     //     }
//     //     //기둥에 매달리는거 유지
//     //     else{
//     //         // anim.speed = 0f;

//     //         state = State.holdIdle;
//     //         rigid.velocity = Vector2.zero;
//     //     }
//     // }

//     // void Footstep()
//     // {
//     //     footstep.Play();
//     // }

//     // void Climb()
//     // {   
//     //     if(Input.GetButtonDown("Jump") && !Input.GetKey(KeyCode.UpArrow)){ // 사다리에서 점프해서 내리는 기능
//     //         rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
//     //         canClimb = false;
//     //         rigid.gravityScale = naturalGravity;
//     //         anim.speed = 1f;
//     //         Jump();
//     //         return;
//     //     }

//     //     float vDirection = Input.GetAxis("Vertical");
//     //     //up
//     //     if(vDirection > 0.1f && !topLadder){
//     //         rigid.velocity = new Vector2(0f, vDirection * climbSpeed);
//     //         anim.speed = 1f;
//     //     }
//     //     //down
//     //     else if(vDirection < -0.1f && !bottomLadder){
//     //         rigid.velocity = new Vector2(0f, vDirection * climbSpeed);
//     //         anim.speed = 1f;
//     //     }
//     //     //still
//     //     else{
//     //         anim.speed = 0f;
//     //         rigid.velocity = Vector2.zero;
//     //     }
//     // }

//     void JumpHitLay()
//     {
//         //땅에 착지함
//         Vector2 frontVec = new Vector2(rigid.position.x + transform.localScale.x * 0.25f, rigid.position.y);

//         Debug.DrawRay(frontVec, Vector3.down * 2f, new Color(0, 1, 0));
//         RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down , 2f , LayerMask.GetMask("Ground"));//Platform만 닿았는지 검사
//         if(rayHit.collider != null){//빔에 맞음              
//             //빔에 절반이 닿으면 닿는것으로 표시됨
//             if(rayHit.distance > 0.5f){
//                 state = State.idle;
//                 jumping = false;
//                 rigid.gravityScale = 4;
//                 rigid.drag = 0.2f;

//                 if(downAttack || downDefend){
//                     StartCoroutine(DownAttack());
//                 }
//             } 
//         }
//     }

//     void OnDrawGizmos()//공격 범위 표시
//     {
//         // if(attack || kicking){
//         //     Gizmos.color = Color.red;
//         //     Gizmos.DrawWireCube(attackPos.position, attackSize);
//         // }
//         if(defending){
//             Gizmos.color = Color.blue;
//             Gizmos.DrawWireCube(defendPos.position, defendSize);
//         }
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireCube(attackPos.position, attackSize);
//     }

//     IEnumerator Invincibility()//무적시간
//     {
//         yield return new WaitForSeconds(4.5f);
//         renderer.color = new Color(1, 1, 1, 1);
//         this.gameObject.layer = 9;
//     }
//     IEnumerator HuntOff()//맞은 효과
//     {
//         yield return new WaitForSeconds(1.5f);
//         state = State.idle;
//         rigid.drag = 0.2f;
//     }
//     IEnumerator DownAttack()//아래로 가속 공격(창,방패)
//     {
//         yield return new WaitForSeconds(1f);
//         this.gameObject.layer = 9;
//         downAttack = false;
//         downDefend = false;
//     }

//     void FreezeX()
//     {
//         isWallJump = false;
//     }
// }
