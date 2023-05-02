using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenenController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToGameScene(){
        SceneManager.LoadScene("MainScene");
    }
    
    public void GoToMenu(){
        SceneManager.LoadScene("TitleScene");
    }

    public void Exit(){
        Application.Quit();
    }
}
