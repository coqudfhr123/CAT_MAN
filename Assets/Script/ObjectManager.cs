using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    // public GameObject bulletPlayerPrefab;
    // public GameObject bulletEnemyPrefab;
    // public GameObject playerArrowPrefab;
    public GameObject enemyArrowPrefab;
    public GameObject enemyBulletPrefab;
    
    //오브젝트 풀링을 하기위한 변수 생성

    // GameObject[] bulletPlayer;
    // GameObject[] bulletEnemy;
    // GameObject[] playerArrow;
    GameObject[] enemyArrow;
    GameObject[] enemyBullet;

    GameObject[] targetPool;
    void Awake()
    {
        //한번에 등장할 갯수를 고려하여 배열 길이를 할당함
        // bulletPlayer = new GameObject[100];
        // bulletEnemy = new GameObject[100];
        // playerArrow = new GameObject[20];
        enemyArrow = new GameObject[50];
        enemyBullet = new GameObject[50];
        Generate();
    }

    void Generate()
    {
        // for(int index = 0; index < bulletPlayer.Length; index++){
        //     bulletPlayer[index] = Instantiate(bulletPlayerPrefab);//Instantiate()로 생성한 인스턴스를 배열에 저장
        //     bulletPlayer[index].SetActive(false);
        // }
        // for(int index = 0; index < bulletEnemy.Length; index++){
        //     bulletEnemy[index] = Instantiate(bulletEnemyPrefab);//Instantiate()로 생성한 인스턴스를 배열에 저장
        //     bulletEnemy[index].SetActive(false);
        // }
        // for(int index = 0; index < playerArrow.Length; index++){
        //     playerArrow[index] = Instantiate(playerArrowPrefab);//Instantiate()로 생성한 인스턴스를 배열에 저장
        //     playerArrow[index].SetActive(false);
        // }
        for(int index = 0; index < enemyArrow.Length; index++){
            enemyArrow[index] = Instantiate(enemyArrowPrefab);//Instantiate()로 생성한 인스턴스를 배열에 저장
            enemyArrow[index].SetActive(false);
        }
        for(int index = 0; index < enemyBullet.Length; index++){
            enemyBullet[index] = Instantiate(enemyBulletPrefab);//Instantiate()로 생성한 인스턴스를 배열에 저장
            enemyBullet[index].SetActive(false);
        }
    }

    public GameObject MakeObj(string type)
    {
        switch (type){
            // //bullet
            // case "BulletPlayer":
            //     targetPool = bulletPlayer;
            //     break;
            // case "BulletEnemy":
            //     targetPool = bulletEnemy;
            //     break;
            // case "playerArrow":
            //     targetPool = playerArrow;
            //     break;
            case "enemyArrow":
                targetPool = enemyArrow;
                break;
            case "enemyBullet":
                targetPool = enemyBullet;
                break;            
        }
        //비활성화된 오브젝트에 접근하여 활성화 후 반환
        for(int index = 0; index < targetPool.Length; index++){
            if(!targetPool[index].activeSelf){
                
                targetPool[index].SetActive(true);
                return targetPool[index];
            }
        }
        return null;//MakeObj 함수를 실행하려면 return을 해야하므로 null로 반환시킨다
    }

    public GameObject[] GetPool(string type)//지정한 오브젝트 풀을 가져오는 함수
    {
        switch (type){
            //bullet
            // case "BulletPlayer":
            //     targetPool = bulletPlayer;
            //     break;

            // case "BulletEnemy":
            //     targetPool = bulletEnemy;
            //     break;
            // case "playerArrow":
            //     targetPool = playerArrow;
            //     break; 
            case "enemyArrow":
                targetPool = enemyArrow;
                break;
            case "enemyBullet":
                targetPool = enemyBullet;
                break;    
        }        
        return targetPool;
    }
}


