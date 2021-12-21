using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ImageMove : MonoBehaviour
{
    [SerializeField] int chapter = 1;
    [SerializeField] bool select;

    [SerializeField] GameObject image;
    [SerializeField] GameObject backNext;
    [SerializeField] GameObject stageButton;
    [SerializeField] GameObject background;

    [SerializeField] Text text1;
    [SerializeField] Text text2;
    [SerializeField] Text text3;
    [SerializeField] Text text4;
    [SerializeField] Text text5;
    [SerializeField] Text text6;
    [SerializeField] Text text7;
    [SerializeField] Text text8;
    [SerializeField] Text text9;
    [SerializeField] Text text10;
    [SerializeField] Text title;

    string[] titleName = {"숲속 맵", "해변 맵", "바다 맵", "눈 맵", "화산 맵"};

    void Start()
    {
        image.transform.position = new Vector2(0, 0);
        select = false;
    }

    void Update()
    {
        ImageScroll();
        ImageColor();
        StageText();
    }
    // public void Left()
    // {
    //     if(chapter != 1){
    //         image.transform.position = new Vector2(18 * (chapter -1), 0);
    //         chapter--;
    //     }
    //     else if(chapter == 1){
    //         image.transform.position = new Vector2(0, 0);
    //     }
    // }
    // public void Right()
    // {
    //     if(chapter != 5){
    //         image.transform.position = new Vector2(18 * (chapter -1), 0);
    //         chapter++;
    //     }
    //     else if(chapter == 5){
    //         image.transform.position = new Vector2(72, 0);
    //     }
    // }

    public void Level1()
    {
        SceneManager.LoadScene(chapter + "-1");
    }

    void ImageScroll()
    {
        //그림이 왼쪽으로 넘어감
        if(Input.GetKeyDown(KeyCode.LeftArrow) && chapter != 1){
            image.transform.position = new Vector2(image.transform.position.x + 18, 0);
            chapter--;
        }
        //그림이 오른쪽으로 넘어감
        else if(Input.GetKeyDown(KeyCode.RightArrow) && chapter != 5){
            image.transform.position = new Vector2(image.transform.position.x - 18, 0);
            chapter++;
        }
        if(!select){
            //쳅터에서 스테이지창으로 넘어감
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)){
                
                image.gameObject.SetActive(false);
                backNext.gameObject.SetActive(false);
                stageButton.gameObject.SetActive(true);
                select = true;
            }
            //쳅터에서 메인화면으로 나감
            else if(Input.GetKeyDown(KeyCode.Escape)){
                SceneManager.LoadScene("main");
            }
        }
        else{
            //스테이지를 선택해서 게임을 실행함
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)){
            
            }
            //스테이지 창에서 쳅터창으로 나감
            else if(Input.GetKeyDown(KeyCode.Escape)){
                image.gameObject.SetActive(true);
                backNext.gameObject.SetActive(true);
                stageButton.gameObject.SetActive(false);
                select = false;
            }
        }
    }

    void ImageColor()
    {
        switch (chapter)
        {
            case 1:
                background.GetComponent<SpriteRenderer>().color = Color.green;
                break;
                
            case 2:
                background.GetComponent<SpriteRenderer>().color = Color.yellow;
                break;

            case 3:
                background.GetComponent<SpriteRenderer>().color = Color.blue;
                break;

            case 4:
                background.GetComponent<SpriteRenderer>().color = Color.white;
                break;
            
            case 5:
                background.GetComponent<SpriteRenderer>().color = Color.red;
                break;
        }
    }

    void StageText()
    {
        text1.GetComponent<Text>().text = chapter + "-1";
        text2.GetComponent<Text>().text = chapter + "-2";
        text3.GetComponent<Text>().text = chapter + "-3";
        text4.GetComponent<Text>().text = chapter + "-4";
        text5.GetComponent<Text>().text = chapter + "-5";
        text6.GetComponent<Text>().text = chapter + "-6";
        text7.GetComponent<Text>().text = chapter + "-7";
        text8.GetComponent<Text>().text = chapter + "-8";
        text9.GetComponent<Text>().text = chapter + "-9";
        text10.GetComponent<Text>().text = chapter + "-10";
        
        title.GetComponent<Text>().text = titleName[chapter - 1];
    }
}
