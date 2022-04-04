using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildMenuBehaviour : MonoBehaviour
{
    GameManager gm;
    public GameObject build, room, healthBar,path,request;
    public RectTransform[] buttonPos;
    public RectTransform[] playButtons;
    public Color red,green;
    Vector3[] buttonPosTarget;
    float transitionSpeed = 4.0f;
    bool buildClick = false,built = false;
    public int type,stars=1;
    public string roomType;
    bool lobby;
    int roomRate = 20;
    public float health = 1.0f; 
    float dmgAmount = 0.05f;
    public int occupancy = 0, available = 0, level=0, expectedOccupancy=0;

    // Start is called before the first frame update
    void Awake()
    {
        lobby = (type==-1);
        if(lobby)type=0;
        roomType = "Floor";
        gm = FindObjectOfType<GameManager>();
        buttonPosTarget = new Vector3[5];
        
        for(int i = 0; i<5; i++){
            buttonPosTarget[i] = buttonPos[i].localPosition;
            buttonPos[i].localPosition = Vector3.zero;
            buttonPos[i].gameObject.SetActive(false);
        }

        foreach (RectTransform r in playButtons){
            r.gameObject.SetActive(false);
        }
        
        if(lobby){
           build.SetActive(false);
           available = 0;
           playButtons[2].gameObject.SetActive(true);
           playButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = "G";
        }

        request.SetActive(false);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(buildClick && type==0){
        for(int i = 0; i<5; i++){
            buttonPos[i].localPosition = Vector3.Lerp(buttonPos[i].localPosition,buttonPosTarget[i],Time.deltaTime*transitionSpeed);
            buttonPos[i].gameObject.SetActive(true);
        } 
       }
       if(type > 0){
           if(!gm.Deduct(type*500)){
               type=0;
               return;
           }
           build.SetActive(false);
           foreach(RectTransform r in playButtons){
               r.gameObject.SetActive(true);
           }
           switch(type){
                case 1:
                    roomType = "GuestRoom";
                    break;
                case 2:
                    roomType = "Restaurant";
                    break;
                case 3:
                    roomType = "Casino";
                    break;
                case 4:
                    roomType = "Lounge";
                    break;
                case 5:
                    roomType = "GiftShop";
                    break;
                default:
                    break;
           }

           available=5; 
           roomRate = type * 10;
           gm.Build(type,gameObject);
           playButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = gm.height+"";
           level=gm.height;
            built=true; 
           type=-1;
       }

       if(built){
           
           playButtons[4].GetComponentInChildren<TextMeshProUGUI>().text = occupancy+"/"+available;
           healthBar.transform.localScale = new Vector3(1,health, 1);
       }

    }

    public void ShowBuildOptions(){
       buildClick=true; 
       gm.am.Play("Pedal");
    }
    
    public void SetType1() => type=1;
    public void SetType2() => type=2;
    public void SetType3() => type=3;
    public void SetType4() => type=4;
    public void SetType5() => type=5;

    public void CallElevator(){
        gm.CallElevator(level);
    }

    public void Repair(){
        int repairCost = (roomRate*3)+(stars*5);
        if(!gm.Deduct(repairCost))return;
        health = 1.0f;
        healthBar.GetComponent<RawImage>().color = health<0.3f?red:green; 
       gm.am.Play("Pedal");
    }
    public void Upgrade(){
        if(stars==5 || !gm.Deduct(available*20))return;
        available++;
        stars++;
        if(stars==5)available++;
        dmgAmount-=0.01f;
       gm.am.Play("Pedal");
    }
    public void Damage(){
        gm.Income(roomRate);
        if(health<0.0f)gm.GameOver();
        health -= dmgAmount;
        healthBar.GetComponent<RawImage>().color = health<0.3f?red:green; 
    }

    public void RequestFloor(int f){
        request.SetActive(true);
        request.GetComponentInChildren<TextMeshProUGUI>().text = f+"";
        if(f==0){
            request.GetComponentInChildren<TextMeshProUGUI>().text = "G";
        }
    }
    public void ClearRequest(){
        request.SetActive(false);
    }
    
    public int GetVacancy(){

        return available-expectedOccupancy;

    }

    public void HideUI(){
        gameObject.GetComponentInChildren<Canvas>().enabled=false;
    }

}
