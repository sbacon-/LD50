using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestBehaviour : MonoBehaviour
{
    public Animator anim;
    GameManager gm;
    public GameObject path;
    Vector3[] pathPoints;
    public Renderer renderer;
    public Color[] possibleColor;
    public GameObject[] hatPrefabs;
    public int target = 0, floor = 0, currentFloor=0;
    public float speed = 0.5f, turnSpeed = 360;
    public int nights = 4;
    public bool staff = false;
    bool waiting = false, leaving = false, onElevator = false;
    // Start is called before the first frame update
    void Start()
    {
        Material mat = renderer.material;
        mat.color = possibleColor[staff?0:Random.Range(1,possibleColor.Length)];
        gm = FindObjectOfType<GameManager>();
        pathPoints=new Vector3[path.transform.childCount];
        anim = GetComponent<Animator>(); 
        anim.speed = 1+speed;
        UpdatePath();    
        if(!staff)anim.SetBool("Walk",true); 
        GameObject go = Instantiate(hatPrefabs[staff?0:Random.Range(1,hatPrefabs.Length)]);
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    { 
        if(staff){
            Vector3 dirToLookTarget2 = ((waiting?Vector3.back:pathPoints[target])-transform.position).normalized;
            float targetAngle2 = 90 - Mathf.Atan2(dirToLookTarget2.z,dirToLookTarget2.x) * Mathf.Rad2Deg;
            float angle2 = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle2, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up*angle2;
            return;
            
        }

        if(!waiting){
            transform.position = Vector3.MoveTowards(transform.position,pathPoints[target],Time.deltaTime*speed);
            if(Vector3.Distance(transform.position,pathPoints[target])<0.05f)UpdateTarget(target+1);
        }

        if(onElevator){
            gm.hotel[currentFloor].RequestFloor(leaving?0:floor);
        }
       Vector3 dirToLookTarget = ((waiting?Vector3.back:pathPoints[target])-transform.position).normalized;
       float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z,dirToLookTarget.x) * Mathf.Rad2Deg;
       float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
       transform.eulerAngles = Vector3.up*angle;

       if(waiting && currentFloor==gm.elevatorLevel)GetOnElevator();
       if(waiting && gm.elevatorLevel == floor && onElevator)GetOffElevator();
    }

    public void UpdatePath(){
        if(staff)return;
        for(int i=0; i<pathPoints.Length; i++){
            pathPoints[i] = path.transform.GetChild(i).position;
        }
        target = 0;
    }

    public void UpdateTarget(int t){
        if(currentFloor==0 && target==1 && !leaving && !onElevator && !waiting)gm.am.Play("Arpeggio");
        if(floor==currentFloor && !leaving){
            t%=2;
            gm.hotel[currentFloor].Damage();
            nights--;
            if(nights<=0){
                target = 2;
                floor=0;
                leaving=true;
                gm.hotel[currentFloor].expectedOccupancy--;
            }
            target = t;
            return;

        }
        if(leaving && currentFloor == 0){
            target--;
            if(target<0){
                target=0;
                Destroy(gameObject);
            }
            return;
        }
        if(target==pathPoints.Length-1){
            RequestFloor();
            anim.SetBool("Walk",false);
            return;
        }
        anim.SetBool("Walk",true);
        target = t;
    }

    void RequestFloor(){
        path.transform.parent.GetComponent<BuildMenuBehaviour>().RequestFloor(leaving?0:floor);
        path = gm.hotel[floor].GetComponent<BuildMenuBehaviour>().path;
        waiting=true;
    }

    void GetOnElevator(){
        transform.parent = gm.elevator;
        transform.localPosition = Vector3.zero;
        waiting=true;
        onElevator = true;
        
    }

    void GetOffElevator(){
        gm.hotel[currentFloor].ClearRequest();
        if(leaving)gm.hotel[currentFloor].occupancy--; 
        currentFloor = gm.elevatorLevel;
        if(!leaving)gm.hotel[currentFloor].occupancy++; 
        if(currentFloor != 0)gm.checkIn = false;
        UpdatePath();
        transform.parent = GameObject.Find("Guests").transform;
        waiting = false;
        anim.SetBool("Walk",true);
        target = leaving?2:0;
        onElevator = false;
    }

}
