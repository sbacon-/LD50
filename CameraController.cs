using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;

    public float slowSpeed,fastSpeed;
    public float zoomSpeed;
    public float movementTime;
    public float scrolls = 10.0f;

    public Vector3 newPosition;
    public Vector3 newZoom;
    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        
        newZoom = cameraTransform.localPosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        HandleMovementInput();        
    }

    void HandleMovementInput(){
        Debug.Log(InputEx.mousePosition.x+", "+InputEx.mousePosition.y);
        float movementSpeed = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))?fastSpeed:slowSpeed;
        float numScrolls = scrolls*Mathf.Abs(Input.mouseScrollDelta.y);
        if(numScrolls == 0.0f)numScrolls = 1.0f;
        if(InputEx.mousePosition.y > 500 || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
            newPosition+= (transform.up * movementSpeed * numScrolls);
        }
        if(InputEx.mousePosition.y < 100 || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.UpArrow)){
            newPosition+= (transform.up * -movementSpeed * numScrolls);
            if(newPosition.y<=0) newPosition = Vector3.zero;
        }
        if(Input.mouseScrollDelta.y != 0){
            newPosition+=(transform.forward * movementSpeed * Input.mouseScrollDelta.y*numScrolls);
        }
        transform.position = Vector3.Lerp(transform.position,newPosition,Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition,newZoom,Time.deltaTime*movementTime);

    }

    public void ZoomOut(){
        newPosition+=(transform.forward * -slowSpeed);
  
    }
}
