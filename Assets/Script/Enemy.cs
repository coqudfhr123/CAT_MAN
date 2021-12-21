using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer renderer;
    Animator anim;
    Collider2D collider;

    //hunt는 플레이어가 발차기를 할떄 발동됨
    //hunt2는 창,방패 공격을 할때 발동됨
    enum State {idle, run, attack, hunt, hunt2, die, back, reload, defend, recoil};
    State state = State.idle;

    public string EnemyType;//캐릭터에 따른 동작을 하게 해줌
    public ObjectManager objectManager;

    public int hp;
    public int nextMove;
    public int hurtForce;

    public bool isArrowHunt;
    public bool isKickHunt;

    public bool isAttack;
    public bool isDie;

    public bool isBack;//적이 뒤로 물러나는 기능

    public bool isReload;

    public bool isDefend;

    public bool isRecoil;
    public bool isRecoilMove;

    public Transform attackPos;
    public Vector2 attackSize;
    public Transform damagePos;
    public Vector2 damageSize;

    public Transform backPos;
    public Vector2 backSize;

    public Transform wall;
    public Transform falling;

    public Transform bulletView;//레이를 시작하는 위치
    public Transform bulletPos;
    public int bulletPower;

    public Transform defendPos;
    public Vector2 defendSize;

    public Transform recoilView;//레이를 시작하는 위치
    public Transform recoilAttackPos;
    public Vector2 recoilAttackSize;

    public GameObject enemyArrow;
    public GameObject enemyBullet;

    public GameObject playerGameobject;
    public Transform playerTransform;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        Invoke("Think", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if(isDie){
            return;
        }
        Attack();
    }

    void FixedUpdate()
    {
        Die();
        anim.SetInteger("state",(int)state);
        if(isDie){
            return;
        }
        BulletAttack();//Dog Bow만 적용됨
        Move();
        HuntMove();
        Back();//Dog Spear만 적용됨
        Reload();//Dog Gun에만 적용됨
        Defend();//Dog Warload에만 적용됨
        Recoil();//Dog Warlord에만 적용됨
    }

    void Move()//절벽이나 벽에서 회전함
    {
        if(isDie || isKickHunt || isArrowHunt || isAttack || isBack || isReload || isRecoil || isDefend){
            return;
        }

        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);//움직임 속도

        if(Mathf.Abs(rigid.velocity.x) > 0.2f){
            state = State.run;
        }
        else{
            state = State.idle;
        }

        //절벽에 떨어지지 않도록 함
        RaycastHit2D fallRayHit = Physics2D.Raycast(falling.position, Vector3.down , 0.5f, LayerMask.GetMask("Ground"));
        if(fallRayHit.collider == null){
            Turn();
        }
        //벽과 충돌시 방향전환
        RaycastHit2D wallRayHit = Physics2D.Raycast(wall.position, Vector3.right * nextMove , 0.5f, LayerMask.GetMask("Ground"));
        if(wallRayHit.collider != null){
            Turn();
        }
    }
    void Turn()
    {
        nextMove *= -1;
        if(nextMove != 0){
        transform.localScale = new Vector2(nextMove ,1);//멈추는것 없이 좌우로 계속 이동함
        }

        CancelInvoke();
        Invoke("Think", 5f);
    }
    void Think()
    {
        nextMove = Random.Range(0,2);
        //애니메이션
        //anim.SetInteger("WalkSpeed", nextMove);

        //스프라이트 뒤집기
        if(nextMove != 0){
            transform.localScale = new Vector2(nextMove ,1);//멈추는것 없이 좌우로 계속 이동함
        }
        //recursive
        float nextThinkTime = Random.Range(2f, 5f);
        if(hp <= 0){
            CancelInvoke();//몬스터가 죽으면 방향전환을 삭제함
            return;
        }
        Invoke("Think", nextThinkTime);
    }

    //몬스터가 플레이어를 공격함
    void Attack()//원거리 근거리 몬스터가 사용함
    {
        if(isKickHunt || isArrowHunt || isDie || isBack || isAttack || isReload || isRecoil || isDefend){
            return;
        }

        Collider2D[] colliderAttack = Physics2D.OverlapBoxAll(attackPos.position, attackSize, 0);        
        foreach (Collider2D collider in colliderAttack)
        {   
            if(collider.tag == "Player" || collider.tag == "PlayerDamaged"){
                isAttack = true;
                state = State.attack;
                CancelInvoke();

                if(transform.position.x < playerTransform.position.x){//플레이어가 몬스터한테 닿으면 몬스터가 플레이어 방향으로 바라봄
                    transform.localScale = new Vector2(1, 1);
                    nextMove = 1; 
                }
                else{
                    transform.localScale = new Vector2(-1, 1);
                    nextMove = -1; 
                }
            }
        }
    }
    public void PlayerDamaged()//몬스터가 공격을 하고 원래 동작으로 돌아감
    {
        Collider2D[] colliderDamage = Physics2D.OverlapBoxAll(damagePos.position, damageSize, 0);        
        foreach (Collider2D collider in colliderDamage)
        {   
            if(collider.tag == "Player"){
                Rigidbody2D playerRigid = collider.GetComponent<Rigidbody2D>();
                collider.GetComponent<PlayerMove>().isHunt = true;//플레이어가 공격 당함을 전달함

                if(transform.position.x < playerTransform.position.x){
                    playerRigid.velocity = new Vector2(hurtForce / 3, hurtForce / 2);
                    playerTransform.localScale = new Vector2(-1, 1); 
                }
                else{
                    playerRigid.velocity = new Vector2(-hurtForce / 3, hurtForce / 2);
                    playerTransform.localScale = new Vector2(1, 1); 
                }
                collider.GetComponent<PlayerMove>().Hunt();
            }

        }
    }
    public void AttackOff()//공격이 끝나면 isAttack을 꺼줌
    {
        isAttack = false;
    }

    void BulletAttack()//원거리 공격 몬스터가 사용함
    {
        if(isKickHunt || isArrowHunt || isReload || isAttack || isRecoil || isDefend){
            return;
        }

        if(EnemyType == "DogBow" || EnemyType == "DogGun"){
            RaycastHit2D bulletRayHit = Physics2D.Raycast(bulletView.position, Vector2.right * transform.localScale.x, 5f, LayerMask.GetMask("Player"));
            if(bulletRayHit.collider != null){
                isAttack = true;
                state = State.attack;
                CancelInvoke();
            }
        }
        else if(EnemyType == "DogWarlord"){
            // Vector2 arrowPos = transform.position;
            // arrowPos.x += 5f * transform.localScale.x;
            // Debug.DrawRay(bulletView.position ,Vector2.right * transform.localScale * 5f , new Color(1, 0, 0));
            RaycastHit2D bulletRayHit = Physics2D.Raycast(bulletView.position, Vector2.right * transform.localScale.x, 5f, LayerMask.GetMask("Player"));
            if(bulletRayHit.collider != null){
                anim.SetBool("isBow",true);
                isAttack = true;
                state = State.attack;
                CancelInvoke();
            }
            else{
                anim.SetBool("isBow",false);
            }
        }
    }
    public void Bullet()//화살을 발사하는 함수
    {
        if(EnemyType == "DogBow" || EnemyType == "DogWarlord"){

            GameObject enemyArrow = objectManager.MakeObj("enemyArrow");//화살 생성
            Rigidbody2D arrowRigid = enemyArrow.GetComponent<Rigidbody2D>();//화살 물리 설정
            enemyArrow.transform.position = new Vector2(bulletPos.position.x, bulletPos.position.y);//화살 위치 설정
            enemyArrow.transform.localScale = new Vector2(transform.localScale.x, 1);//화살 방향 설정
            arrowRigid.velocity = new Vector2(transform.localScale.x * bulletPower / 2, rigid.velocity.y);//화살 날라가는 힘 설정

        }
        else if(EnemyType == "DogGun"){

            isReload = true;

            GameObject enemyBullet = objectManager.MakeObj("enemyBullet");
            Rigidbody2D bulletRigid = enemyBullet.GetComponent<Rigidbody2D>();
            enemyBullet.transform.position = new Vector2(bulletPos.position.x, bulletPos.position.y);
            enemyBullet.transform.localScale = new Vector2(transform.localScale.x, 1);
            bulletRigid.velocity = new Vector2(transform.localScale.x * bulletPower / 2, rigid.velocity.y);
        }
    }

    void Reload()//발사 동작이 끝나야 하며 공격 당하지 않아야 Reload를 할수 있음
    {
        if(!isReload){
            return;
        }

        if(!isAttack && EnemyType == "DogGun" && !isKickHunt && !isArrowHunt){
            state = State.reload;
        }
    }
    public void ReloadOff()//DogGun일때만 실행
    {
        isReload = false;
    }

    //플레이어가 몬스터를 공격함
    public void KickDamage()//발차기 공격을 맞을시 발동
    {   
        if(isDie){ //적이 죽었을때 발차기 공격을 받음
            rigid.velocity = new Vector2((hurtForce * playerTransform.localScale.x) / 2, hurtForce / 4);
            return;
        }
        else if(isKickHunt){//중복 공격을 막음
            return;
        }

        isKickHunt = true;
        isAttack = false;//적이 공격 도중에 공격을 받으면 공격 강제 중단
        isRecoil = false;
        isRecoilMove = false;

        if(EnemyType == "DogShield"){//방패병일때는 공격을 막아줌
            if((transform.position.x < playerTransform.position.x && transform.localScale.x == 1) 
            || (transform.position.x > playerTransform.position.x && transform.localScale.x == -1)){

                if(transform.position.x < playerTransform.position.x){
                    nextMove = 1;
                    transform.localScale = new Vector2(1 ,1);
                }
                else{
                    nextMove = -1;
                    transform.localScale = new Vector2(-1 ,1);
                }
                rigid.velocity = new Vector2((hurtForce * transform.localScale.x * -1) / 3, rigid.velocity.y);
                state = State.defend;
                return;
            }
        }

        hp -= 1;
        state = State.hunt;
        if(transform.position.x < playerTransform.position.x){
            nextMove = 1;
            transform.localScale = new Vector2(1 ,1);
        }
        else{
            nextMove = -1;
            transform.localScale = new Vector2(-1 ,1);
        }
        rigid.velocity = new Vector2((hurtForce * playerTransform.localScale.x) / 2, hurtForce / 2);
        renderer.color = new Color(1, 0, 0, 1);
        Invoke("HuntOff", 0.3f); 
    }
    public void ArrowDamage()//화살 프리펩 스크립트에서 실행함
    {
        if(isDie){ //적이 죽으면 화살 공격은 받지 않음
            return;
        }

        if(EnemyType == "DogShield"){//방패병일때는 공격을 막아줌
            if((transform.position.x < playerTransform.position.x && transform.localScale.x == 1) 
            || (transform.position.x > playerTransform.position.x && transform.localScale.x == -1)){

                if(transform.position.x < playerTransform.position.x){
                    nextMove = 1;
                    transform.localScale = new Vector2(1 ,1);
                }
                else{
                    nextMove = -1;
                    transform.localScale = new Vector2(-1 ,1);
                }
                isDefend = true;
                state = State.defend;
                Invoke("HuntOff", 0.3f);
                Debug.Log("defend");
                return;
            }
        }

        isAttack = false;
        isArrowHunt = true;
        isRecoil = false;
        isRecoilMove = false;

        hp -= 1;
        state = State.hunt2;
        if(transform.position.x < playerTransform.position.x){
            nextMove = 1;
            transform.localScale = new Vector2(1 ,1);
        }
        else{
            nextMove = -1;
            transform.localScale = new Vector2(-1 ,1);
        }
        rigid.velocity = new Vector2((hurtForce * playerTransform.localScale.x) / 3, rigid.velocity.y);

        renderer.color = new Color(1, 0, 0, 1);
        Invoke("HuntOff", 0.3f);
    }
    void HuntOff()//Invoke에서 실행함
    {
        renderer.color = new Color(1, 1, 1, 1);
        
        isArrowHunt = false;
        isDefend = false;
    }
    void HuntMove()
    {
        if(Mathf.Abs(rigid.velocity.x) < 0.1f && isKickHunt)
        {
            state = State.idle;
            isKickHunt = false;
        }
    }

    void Die()
    {
        if(hp <= 0){
            state = State.die;
            isDie = true;
        }
    }

    void Back()
    {
        if(isDie || EnemyType != "DogSpear"){
            return;
        }
        
        Collider2D[] colliderDamage = Physics2D.OverlapBoxAll(backPos.position, backSize, 0);        
        foreach (Collider2D collider in colliderDamage)
        {   
            if(collider.tag == "Player" || collider.tag == "PlayerDamaged"){

                isBack = true;//이것이 켜져있으면 다른 동작은 못함
                isAttack = false;
                state = State.back;

                if(transform.position.x < playerTransform.position.x){

                    transform.localScale = new Vector2(1, 1);
                    rigid.velocity = new Vector2(-1, rigid.velocity.y);
                }
                else{
                    transform.localScale = new Vector2(-1, 1);
                    rigid.velocity = new Vector2(1, rigid.velocity.y);
                }
            }
            else{
                isBack = false;
            }
        }

    }

    void Defend()//플레이어의 화살을 막음
    {
        if(EnemyType != "DogWarlord" || isDie || isDefend){
            return;
        }

        Collider2D[] colliderDefend = Physics2D.OverlapBoxAll(defendPos.position, defendSize, 0);        
        foreach (Collider2D collider in colliderDefend)
        {   
            if(collider.tag == "PlayerBullet"){
                isDefend = true;
                isAttack = false;
                state = State.defend;
                Debug.Log("Defend");
                collider.GetComponent<PlayerBullet>().Delay();//화살을 자르는 함수를 실행하는 부분
            }
        }
    }
    public void DefendOff()//동작을 종료하는 함수
    {
        isDefend = false;
    }

    void Recoil()
    {
        if(isRecoilMove == true){//구르는 속도 설정
            rigid.velocity = new Vector2(nextMove * 3, rigid.velocity.y);//움직임 속도
        }

        if(isDie || EnemyType != "DogWarlord" || isRecoil){
            return;
        }

        RaycastHit2D recoilRayHit = Physics2D.Raycast(recoilView.position, Vector2.right * transform.localScale.x, 1.5f, LayerMask.GetMask("Player"));
        if(recoilRayHit.collider != null){
            isAttack = false;//동작 오작동을 방지함
            isRecoil = true;
            isRecoilMove = true;
            state = State.recoil;
        }
    }
    public void RecoilMoveOff()
    {
        isRecoilMove = false;
    }
    public void RecoilOff()//공격도 진행하면서 recoil을 끔
    {
        Collider2D[] colliderRecoilAttack = Physics2D.OverlapBoxAll(recoilAttackPos.position, recoilAttackSize, 0);        
        foreach (Collider2D collider in colliderRecoilAttack)
        {   
            if(collider.tag == "Player"){
                Rigidbody2D playerRigid = collider.GetComponent<Rigidbody2D>();
                collider.GetComponent<PlayerMove>().isHunt = true;//플레이어가 공격 당함을 전달함

                if(transform.position.x < playerTransform.position.x){
                    playerRigid.velocity = new Vector2(hurtForce / 3, hurtForce / 2);
                    playerTransform.localScale = new Vector2(-1, 1); 
                }
                else{
                    playerRigid.velocity = new Vector2(-hurtForce / 3, hurtForce / 2);
                    playerTransform.localScale = new Vector2(1, 1); 
                }
                collider.GetComponent<PlayerMove>().Hunt();
            }
        }
        isRecoil = false;
    }

    void OnDrawGizmos()//공격 범위 표시
    {
        if(isDie){//죽으면 작동하지 않도록 함
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos.position, attackSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(damagePos.position, damageSize);

        if(EnemyType == "DogBow" || EnemyType == "DogGun" || EnemyType =="DogWarlord"){
            Gizmos.color = Color.red;
            Gizmos.DrawRay(bulletView.position, Vector2.right * transform.localScale.x * 5f);
        }

        // Gizmos.color = Color.blue;
        // Gizmos.DrawRay(bulletView.position, Vector3.right * transform.localScale.x * 5f);

        if(EnemyType == "DogSpear"){
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(backPos.position, backSize);
        }

        if(EnemyType == "DogWarlord"){

            Gizmos.color = Color.red;
            Gizmos.DrawRay(recoilView.position, Vector3.right * transform.localScale.x * 1.5f);
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(recoilAttackPos.position, recoilAttackSize);//구르는 부분

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(defendPos.position, defendSize);//화살을 제거하는 부분
        }

        Gizmos.color = Color.green;
        Gizmos.DrawRay(falling.position, Vector3.down * 0.5f);
    }
    
}
