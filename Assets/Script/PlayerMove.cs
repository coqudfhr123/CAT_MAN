using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    Collider2D collider;
    SpriteRenderer renderer;

    public float speed;
    public float runSpeed;
    float defaultSpeed;
    

    public bool isJump;
    public bool isStartJump;
    public bool isShortJump;
    public float jumpForce;
    public LayerMask ground;
    public Transform JUMP;
    public float jumpCurTime;//키를 짧게 누르면 낮은 점프 길게 누르면 높은 점프
    public float jumpMaxTime;

    public bool isAttack;
    public bool kickAttack;//참이면 kick 아니면 bow
    public float arrowPower;
    public Transform attackPos;
    public Vector2 attackSize;
    public GameObject playerArrow;
    public Transform arrowPos;
    // public ObjectManager objectManager;

    public Transform wallChk;
    public LayerMask w_Layer;
    public bool isWall;
    public bool isWallJump;
    public float wallchkDistance;
    public float slidingSpeed;
    public float wallJumpPower;
    public float wallCurTime;
    public float wallMaxTime;

    public bool isPush;
    public bool isWallPush;
    public Transform pushChk;
    public LayerMask p_Layer;
    public float pushchkDistance;

    public float huntForce;
    public bool isHunt;
    public bool isDie;
    public int hp;

    public bool isHold;
    public float hold_y;//매달리는 물체의 높이값을 전달받음

    public bool isLadder;
    public bool bottomLadder;
    public bool topLadder;
    public float naturalGravity;
    public float climbSpeed = 3f;
    public Ladder ladder;

    public bool isDown;
    public bool isStartDown;
    public bool isAnimOff;


    enum State {idle, run, jump1, jump2, fail, hunt, bow, kick, ladder, push, wall, die, shortJump, holdIdle, holdMove, holdKick, down, up};
    State state = State.idle;

    // Start is called before the first frame update
    void Start()
    {
        defaultSpeed = speed;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        renderer = GetComponent<SpriteRenderer>();

        // collider.offset = new Vector2(0.125f, -0.175f);
        // CapsuleCollider2D temp = (CapsuleCollider2D)collider;
        // temp.direction = CapsuleCollider2D.Direction.Vertical;
        // temp.size = new Vector2(0.85f, 1.9f);
    }

    void Update()
    {
        if(isDie){
            return;
        }
        Jump();
        Ladder();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        anim.SetInteger("state",(int)state);
        Die();
        if(isDie){
            return;
        }
        Move();
        Attack();
        Fall();
        Wall();
        Push();
        WallPush();
        Hold();
        LadderClimb();
        Down();
        Up();
    }

    void Move()
    {   
        if(isStartJump || isAttack || isWallJump || isWall || isWallPush || isHunt || isHold || isLadder || isDown){
            return;
        }

        float hDirection = Input.GetAxis("Horizontal");

        if(!Input.GetKey(KeyCode.A)){//공격한 사이클이 끝나도 이동은 금지함
            rigid.velocity = new Vector2(hDirection * defaultSpeed, rigid.velocity.y);
        }
        
        
        if(hDirection < 0){//공격 한사이클이 끝나고는 방향 회전은 가능하게 함
            transform.localScale = new Vector2(-1 ,1);
        }
        else if(hDirection > 0){
            transform.localScale = new Vector2(1 ,1);
        }

        if(!isJump){
            if(Mathf.Abs(rigid.velocity.x) < 0.05f){
                state = State.idle;
            }
            else{
                state = State.run;
            }
        }
    }

    void Jump()
    {
        if((rigid.velocity.y < -0.1 && isWallJump) || isHunt || isLadder || isDown || isAttack){
            return;
        }
        else if(Input.GetKey(KeyCode.S) && (collider.IsTouchingLayers(ground))){    
            jumpCurTime += Time.deltaTime;
        }

        if(!isJump){ //중복 점프 방지            
            if(isHold){
                isStartJump = false;
            }
            else if(jumpCurTime < jumpMaxTime && Input.GetKeyUp(KeyCode.S)){//짧게 누르면 낮은 점프
                isJump = true;
                state = State.shortJump;
                rigid.velocity = new Vector2(rigid.velocity.x, jumpForce / 2);
                jumpCurTime = 0;
            }
            else if(jumpCurTime > jumpMaxTime){
                isJump = true;
                isStartJump = true;//큰 점프 준비중 이동금지
                state = State.jump1;
                jumpCurTime = 0;
            }
        }

        if(rigid.velocity.y > 0.1f && !isShortJump && Input.GetButton("Horizontal") && isLadder){//공중으로 날아오를때 좌우로 이동하면 동작이 변함
            state = State.jump2;
        }
    }
    public void JumpPower()
    {
        isStartJump = false;
        rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
    }

    void Fall()
    {

        if(rigid.velocity.y < -0.3f){//떨어지는 상태
            
            if(!isLadder){
                state = State.fail;
            }
            isJump = true;//벽에 매달리는 동작을 위함임
            Debug.DrawRay(JUMP.position, (Vector2.right * transform.localScale.x) * 0.6f, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(JUMP.position, Vector2.right * transform.localScale.x, 0.6f, ground);
            if(rayHit.collider != null){
                isJump = false;
                isShortJump = false;
                isWall = false;
                isWallJump = false;
                jumpCurTime = 0;
                if(isDown){
                    state = State.down;
                }
                else{
                    state = State.idle;
                } 
            }
        }
    }

    void Attack()
    {   
        if(isHunt || isLadder || isDown || isJump){
            isAttack = false;
            return;
        }

        Collider2D[] colliderAttack = Physics2D.OverlapBoxAll(attackPos.position, attackSize, 0);//발차기 여부를 감지함
        foreach (Collider2D collider in colliderAttack)
        {   
            if(!isAttack){
                if(isHold){
                    kickAttack = true;//매달린 상태에서는 무조건 발차기 공격만 함
                }
                else{
                    if(collider.tag == "Enemy" || collider.tag == "Box"){//발차기 공격
                        kickAttack = true;
                        goto attack;
                    }
                    else{//활 공격
                        kickAttack = false;
                    }
                }
            }
        }
        attack:

        if(Input.GetKey(KeyCode.A)){
            isAttack = true;
            if(kickAttack){//발차기
                if(isHold){
                    state = State.holdKick;
                }
                else{
                    state = State.kick;
                }
            }
            else{//활 쏘기
                state = State.bow;
            }
        }
        else{
            isAttack = false;
        }
    }
    public void KickOn()//애니메이터에서 실행함
    {
        Collider2D[] colliderKick = Physics2D.OverlapBoxAll(attackPos.position, attackSize, 0);//발차기를 할때 적이 있으면 적이 날라감
        foreach (Collider2D collider in colliderKick)
        {   
            if(collider.tag == "Box"){//발차기 공격
                Rigidbody2D boxRigid = collider.GetComponent<Rigidbody2D>();
                if(transform.localScale.x == 1){//오른쪽으로 바라봄
                    boxRigid.velocity = new Vector2(huntForce / 2,boxRigid.velocity.y);
                }
                else{
                    boxRigid.velocity = new Vector2(-huntForce / 2,boxRigid.velocity.y);
                }
                
            }
            else if(collider.tag == "Enemy"){
                collider.GetComponent<Enemy>().KickDamage();
                
            }
        }

        // Collider2D[] colliderAttack = Physics2D.OverlapBoxAll(attackPos.position, attackSize, 0);        
        // foreach (Collider2D collider in colliderAttack)
        // {   

        //     //부셔지는 오브젝트에 적용됨
        //     else if(collider.tag == "Destruction"){
        //         collider.GetComponent<Destruction>().ObjectDamaged();
        //     }
        // }
    }
    public void KickOff()//애니메이터에서 실행함
    {
        isAttack = false;
    }
    public void BowOn()//애니메이터에서 실행함
    {
        playerArrow.transform.localScale = new Vector2(transform.localScale.x, 1);//화살 방향 설정
        Instantiate(playerArrow, arrowPos.position, Quaternion.identity); 
        // GameObject playerArrow = objectManager.MakeObj("playerArrow");//화살 생성
        // Rigidbody2D arrowRigid = playerArrow.GetComponent<Rigidbody2D>();//화살 물리 설정
        // playerArrow.transform.position = new Vector2(arrowPos.position.x, arrowPos.position.y);//화살 위치 설정
        // playerArrow.transform.localScale = new Vector2(transform.localScale.x, 1);//화살 방향 설정
        // arrowRigid.velocity = new Vector2(transform.localScale.x * arrowPower, rigid.velocity.y);//화살 날라가는 힘 설정
        isAttack = false;
    }

    void Wall()//벽 매달리기 및 지그재그로 오르기
    {
        if(isHunt || isHold || isLadder || isDown){//빠르게 떨어지면 벽에 매달리지 못함
            return;
        }

        isWall = Physics2D.Raycast(wallChk.position, Vector2.right * transform.localScale.x, wallchkDistance, w_Layer);
        
        if(isWall && isJump){
            wallCurTime += Time.deltaTime;

            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * slidingSpeed);

            if(wallCurTime > wallMaxTime){//시간에 따라 동작이 변함
                state = State.wall;
            }
            else if(wallCurTime <= wallMaxTime){
                state = State.jump2;
            }

            if(Input.GetKeyDown(KeyCode.S) && !isWallJump){//점프키를 누르면 지그재그로 벽을 탐
                isWallJump = true;
                rigid.velocity = new Vector2(-transform.localScale.x * wallJumpPower, 0.9f * wallJumpPower);
                transform.localScale = new Vector2(-transform.localScale.x, 1);//방향전환
                Invoke("WallJumpTime",0.2f);//지그재그 벽타기 지연속도
                state = State.jump2;
            }
            else if(Input.GetButtonDown("Horizontal")){//벽에서 좌우 방향키를 눌러 강제로 내림
                state = State.fail;
            }

            float hDirection = Input.GetAxis("Horizontal");//좌우 방향키로 강제로 내리는 기능
            if(transform.localScale.x == -1 && Input.GetKey(KeyCode.RightArrow)){
                rigid.velocity = new Vector2(hDirection * defaultSpeed, rigid.velocity.y);
            }
            else if(transform.localScale.x == 1 && Input.GetKey(KeyCode.LeftArrow)){
                rigid.velocity = new Vector2(hDirection * defaultSpeed, rigid.velocity.y);
            }
        }
        else if(!isWall){
            wallCurTime = 0;
        }
    }
    void WallJumpTime(){ //Invoke에 의해 동작함
        isWallJump = false;
    }

    void Push()//물체를 밀어서 움직임
    {
        if(isJump || isHunt || !Input.GetButton("Horizontal") || isHold || isLadder || isDown){
            isPush = false;
            return;
        }

        isPush = Physics2D.Raycast(pushChk.position, Vector2.right * transform.localScale.x, pushchkDistance, p_Layer);

        if(isPush && Input.GetButton("Horizontal")){
            state = State.push;

            float hDirection = Input.GetAxis("Horizontal");//방향전환을 하여 동작을 취소할수 있음
            if(hDirection < 0){
                transform.localScale = new Vector2(-1 ,1);
            }
            else if(hDirection > 0){
                transform.localScale = new Vector2(1 ,1);
            }
        }
    }
    void WallPush()//벽은 밀어도 움직이지 않음
    {
        if(isJump || isHunt || !Input.GetButton("Horizontal") || isHold || isLadder || isDown){
            isWallPush = false;
            return;
        }

        isWallPush = Physics2D.Raycast(pushChk.position, Vector2.right * transform.localScale.x, pushchkDistance, ground);

        if(isWallPush && Input.GetButton("Horizontal")){
            state = State.push;

            float hDirection = Input.GetAxis("Horizontal");//방향전환을 하여 동작을 취소할수 있음
            if(hDirection < 0){
                transform.localScale = new Vector2(-1 ,1);
            }
            else if(hDirection > 0){
                transform.localScale = new Vector2(1 ,1);
            }
        }
    }

    public void Hunt()//적 스크립트에서 실행시킴
    {   
        hp -= 1;
        if(hp <= 0){
            return;
        }
        else if(isDown){//맞으면 엎드리는 동작을 취소함
            isDown = false;
            //플레이어 충돌 영역을 변경함
        }
        isHunt = true;
        isStartJump = false;
        state = State.hunt;
        renderer.color = new Color(0.5f, 0, 0, 1);
        this.gameObject.tag = "PlayerDamaged";
        Invoke("HuntOff", 0.3f);
    }
    void HuntOff()//Invoke에서 실행함
    {
        renderer.color = new Color(1, 1, 1, 0.5f);
        isHunt = false;//해제 하지 않으면 플레이어가 움직이지 못함
        Invoke("InvincibilityOff", 5f);//3초간 무적시간
    }
    void InvincibilityOff()//Invoke에서 움직임
    {
        this.gameObject.tag = "Player";
        renderer.color = new Color(1, 1, 1, 1f);
        
    }
    void Die()//어떤 상태든 체력이 0이하면 무조건 실행함
    {
        if(hp > 0){
            return;
        }

        isDie = true;
        state = State.die;
    }


    void Hold()
    {
        if(!isHold || isHunt || isLadder || isDown){
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigid.drag = 0.2f;
            return;
        }
        else if(isAttack){
            rigid.drag = 30f;
            return;
        }

        rigid.constraints = RigidbodyConstraints2D.FreezePositionY | //기둥에 메달리는게 설정함
        RigidbodyConstraints2D.FreezeRotation;
        rigid.drag = 30f;
        transform.position = new Vector2(transform.position.x, hold_y - 0.75f);

        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.DownArrow)){ // 기둥에서 내려옴
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigid.drag = 0.2f;
            isHold = false;
        }
        else if(Input.GetButton("Horizontal")){//기둥에서 좌우로 이동함
            float hDirection = Input.GetAxis("Horizontal");
            rigid.velocity = new Vector2((defaultSpeed * hDirection) / 3, rigid.velocity.y);
            state = State.holdMove;

            if(hDirection < 0){
                transform.localScale = new Vector2(-1 ,1);
            }
            else if(hDirection > 0){
                transform.localScale = new Vector2(1 ,1);
            }
        }
        else{
            state = State.holdIdle;//기둥에서 움직임 없을때
        }
    }

    void Ladder()//사다리 탑승
    {
        if(isLadder){
            state = State.ladder;
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX | 
            RigidbodyConstraints2D.FreezeRotation;
            isJump = false;

            transform.position = new Vector3(ladder.transform.position.x,rigid.position.y);
            rigid.gravityScale = 0f;
        }
    }
    void LadderClimb()
    {   
        if(!isLadder || isHunt || isDown){
            anim.speed = 1f;
            return;
        }

        if(Input.GetKey(KeyCode.S)){ // 사다리에서 점프해서 내리는 기능
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigid.gravityScale = naturalGravity;
            anim.speed = 1f;
            float hDirection = Input.GetAxis("Horizontal");
            rigid.velocity = new Vector2(hDirection * jumpForce, jumpForce/2);
            isLadder = false;
            return;
        }
        float vDirection = Input.GetAxis("Vertical");
        //up
        if(vDirection > 0.1f && !topLadder){
            rigid.velocity = new Vector2(0f, vDirection * climbSpeed);
            anim.speed = 1f;
        }
        //down
        else if(vDirection < -0.1f && !bottomLadder){
            rigid.velocity = new Vector2(0f, vDirection * climbSpeed);
            anim.speed = 1f;
        }
        //still
        else{
            anim.speed = 0f;
            rigid.velocity = Vector2.zero;
        }
    }

    //적의 공격을 피하기 위해서 엎드리는동작을 하거나 취소하는 동작을 나타냄
    void Down()
    {
        if(isLadder || isJump || !collider.IsTouchingLayers(ground)){//사다리 탈때나 점프중에는 엎드리기 금지
            return;
        }
        
        if(!isDown && Input.GetKey(KeyCode.DownArrow)){
            isDown = true;
            state = State.down;
        }
    }
    public void DownComplete()//애니메이션이 끝나야 동작이 완료됨
    {
        //플레이어 충돌 영역을 변경함
    }
    void Up()
    {
        if(isDown && !Input.GetKey(KeyCode.DownArrow)){
            state = State.up;
        }
    }
    public void UpComplete()
    {
        isDown = false;
        //플레이어 충돌 영역을 변경함
    }



    void OnDrawGizmos()//공격 범위 표시
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos.position, attackSize);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(wallChk.position, Vector2.right * transform.localScale.x * wallchkDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(pushChk.position, Vector2.right * transform.localScale.x * pushchkDistance);
    }
}
