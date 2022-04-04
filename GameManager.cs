using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public AudioManager am;
    public LeaderboardController lbc;
    public string sceneMusic;
    public Animator easterEgg;

    public int height = 0; 
    public GameObject[] builds, gameOver;
    public GameObject directionalLight, depTimer;
    public string[] loseText;
    public GameObject roomPrefab, npcPrefab,moneyText,deltaText, noMoney;
    public BuildMenuBehaviour[] hotel;
    public Transform elevator;
    Vector3 elevatorTarget;
    public int elevatorLevel = 0,elevatorTargetLevel = 0;
    float elevatorSpeed = 1f;
    public int money = 0, deposit = 0, depTime = 30;
    public bool checkIn = false,danger=false, endGame = false;
    
    // Start is called before the first frame update
    void Start()
    {
        am = FindObjectOfType<AudioManager>();
        am.Play(sceneMusic); 
        money=600;
        elevatorTarget = elevator.position;
        InvokeRepeating("NewGuest",1,3);
        InvokeRepeating("Deposit",1,1);
        easterEgg.speed=0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        depTimer.GetComponent<RectTransform>().localScale = new Vector3(1,(30 - depTime)/30.0f,1);
        //directionalLight.transform.RotateAround(Vector3.zero,Vector3.right,10.0f * Time.deltaTime);
        elevator.position = Vector3.MoveTowards(elevator.position,elevatorTarget,elevatorSpeed*Time.deltaTime);
        if(Vector3.Distance(elevator.position,elevatorTarget)<0.1f){
            PlayElevator();
            elevatorLevel = elevatorTargetLevel; 
            elevator.position = elevatorTarget;
        }
        if(deltaText.GetComponent<TextMeshProUGUI>().color.a > 0.0f){
            TextMeshProUGUI dt = deltaText.GetComponent<TextMeshProUGUI>();
            dt.color = new Color(dt.color.r,dt.color.g,dt.color.b,dt.color.a-0.4f*Time.deltaTime);
            dt.transform.localPosition = Vector3.MoveTowards(dt.transform.localPosition,Vector3.down*30,30.3f*Time.deltaTime);
        }
        if(noMoney.GetComponentInChildren<TextMeshProUGUI>().color.a > 0.0f){
            TextMeshProUGUI mt1 = noMoney.GetComponentInChildren<TextMeshProUGUI>();
            RawImage[] bg = noMoney.GetComponentsInChildren<RawImage>();

            mt1.color = new Color(mt1.color.r,mt1.color.g,mt1.color.b,mt1.color.a-0.4f*Time.deltaTime);
            foreach(RawImage ri in bg){
                ri.color = new Color (ri.color.r,ri.color.g,ri.color.b,mt1.color.a);
            }
        }else{
            noMoney.SetActive(false);
        }

        GetDanger();
    }

    public void Build(int t, GameObject bhb){
        am.Play("Cowbell");
        GameObject go = Instantiate(builds[t]);
        go.transform.parent = bhb.transform;
        go.transform.localPosition = new Vector3(-0.3f,0,0);
        go.transform.rotation = Quaternion.Euler(0,180,0);
         
        height++; 
        BuildMenuBehaviour[] hotelCopy = new BuildMenuBehaviour[height];
        for(int i=0; i<height; i++){
            hotelCopy[i]=hotel[i].gameObject.GetComponent<BuildMenuBehaviour>();
        }
        hotel = new BuildMenuBehaviour[height+1];
        for(int i=0; i<height;i++){
            hotel[i] = hotelCopy[i].gameObject.GetComponent<BuildMenuBehaviour>();
        }
        hotel[height] = bhb.GetComponent<BuildMenuBehaviour>();
        go = Instantiate(roomPrefab);
        go.transform.parent = GameObject.Find("Hotel").transform;
        go.name = go.GetComponent<BuildMenuBehaviour>().roomType + (height+1);
        go.transform.position = Vector3.up*(height+1);
    }

    public void PlayElevator(){
        if(elevatorLevel == -1) am.Play("Maracas");
    }

    public void CallElevator(int level){
        am.Play("Pedal");

        elevatorLevel = -1;
        elevatorTargetLevel = level;
        elevatorTarget = new Vector3(1.0f,level,-0.3f);
    }

    public void NewGuest(){

        if(GetVacancy()==0 || checkIn)return;
        BuildMenuBehaviour bmb = hotel[Random.Range(1,height+1)];
        while(bmb.GetVacancy()==0) bmb = hotel[Random.Range(1,height+1)];
        checkIn = true;

        for(int i = 0; i<Random.Range(1,bmb.GetVacancy()); i++){
        GameObject npc = Instantiate(npcPrefab);
        npc.transform.parent = GameObject.Find("Guests").transform;
        npc.transform.position = new Vector3(Random.Range(0,2)==1?2:-2,0,-1f);

        GuestBehaviour gb = npc.GetComponent<GuestBehaviour>();
        gb.speed = Random.Range(0.2f,0.7f);
        gb.nights = Random.Range(2,100);
        gb.floor = bmb.level;
        gb.path = hotel[0].path;
        bmb.expectedOccupancy++;
        }
    }

    int GetVacancy(){
        int vacancy = 0;
        foreach(BuildMenuBehaviour bmb in hotel){
            vacancy+=bmb.GetVacancy();
        }
        return vacancy;
    }

    public bool Deduct(int amt){
        if(money<amt){
            noMoney.SetActive(true);
            TextMeshProUGUI mt1 = noMoney.GetComponentInChildren<TextMeshProUGUI>();
            mt1.color = new Color(mt1.color.r,mt1.color.g,mt1.color.b,1.0f);
            mt1.text = "Insufficient Funds ($"+amt+")";
            return false;
        }
        money-=amt;
        moneyText.GetComponent<TextMeshProUGUI>().text = "$"+money;
        deltaText.GetComponent<TextMeshProUGUI>().text = "-$"+amt;
        deltaText.GetComponent<TextMeshProUGUI>().color = hotel[0].red;
        return true;
    }

    public void Income(int amt){
        deposit+=amt; 
    }
    void Deposit(){
        depTime--;
        if(depTime!=0)return;
        depTime = 30;
        money+=deposit; 
        moneyText.GetComponent<TextMeshProUGUI>().text = "$"+money;
        deltaText.GetComponent<TextMeshProUGUI>().text = "+$"+deposit;
        deltaText.GetComponent<TextMeshProUGUI>().color = hotel[0].green;
        deposit = 0;
        am.Play("Tambourine");
        if(money>=1000000)money=999999;
    }

    public void GameOver(){
        if(endGame)return;
        gameOver[0].GetComponentInChildren<TextMeshProUGUI>().text = loseText[Random.Range(0,loseText.Length)];
        gameOver[0].SetActive(true);
        lbc.score = (height*1000000)+money;
        BuildMenuBehaviour[] bmbs = GameObject.Find("Hotel").GetComponentsInChildren<BuildMenuBehaviour>();
        foreach(BuildMenuBehaviour bmb in bmbs){
            bmb.HideUI();
        }
        CancelInvoke("Deposit");
        elevator.GetComponent<Rigidbody>().isKinematic=false;
        Rigidbody[] bodies = GameObject.Find("Hotel").GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rb in bodies){
            rb.isKinematic = false;
        } 
        elevator.GetComponent<Rigidbody>().AddExplosionForce(50.0f,Vector3.zero,30,0,ForceMode.Force);
        am.Play("Crash");
        am.Stop("Danger");
        endGame = true;
    }

    public void FailedToSend(){
        gameOver[1].SetActive(true);
    }
    public void SentScore(){
        gameOver[2].SetActive(true);
    }

    public void Restart(){
        SceneManager.LoadScene(1);
    }
    public void Quit(){

        SceneManager.LoadScene(0);
    }
    public void GetDanger(){
        if(endGame) return;
        bool dangerPresent = danger;

        danger = false;
        foreach(BuildMenuBehaviour bmb in hotel){
            if(bmb.health < 0.1f)danger=true;
        }

        if(dangerPresent && !danger)am.Stop("Danger");
        if(!dangerPresent && danger)am.Play("Danger");
    }
    public void EasterEgg(){
        easterEgg.speed = 1.0f;
    }
}
