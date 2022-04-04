using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehaviour : MonoBehaviour
{
    public bool mainMenu;
    public GameObject background,titleCard;

    bool grow = true;
    float bgScale = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<AudioManager>().Play("MainTheme");
    }

    // Update is called once per frame
    void Update()
    {
        if(mainMenu){
            if(bgScale>1.0f) grow = false;
            if(bgScale<0.5f) grow = true;
            bgScale+=((grow)?0.05f:-0.05f)*Time.deltaTime;
            background.GetComponent<RectTransform>().localScale = Vector3.one *bgScale;
            titleCard.GetComponent<RectTransform>().localScale = Vector3.one * (1+ 0.5f * bgScale);
        }

    }

    public void StartGame(){
        SceneManager.LoadScene("Game");
    }

}
