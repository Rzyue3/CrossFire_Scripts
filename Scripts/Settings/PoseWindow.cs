using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoseWindow : MonoBehaviour
{
    [SerializeField]
    private SettingWindow settingWindow;
    [SerializeField]
    private WarningWindow warningWindow;
    [SerializeField]
    private GameObject poseObj;
    private bool canvasOpen;
    private int num;
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        num -= 1;
        settingWindow = GameObject.Find("Setting").GetComponent<SettingWindow>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "Title")
        {
            if(PlayerStats.instance.playerDead) return;
            if(canvasOpen)
            {
                poseObj.SetActive(false);
                Time.timeScale = 1f;
                canvasOpen = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                poseObj.SetActive(true);
                Time.timeScale = 0f;
                canvasOpen = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    private void SceneLoaded(Scene next, LoadSceneMode mode)
    {
        if(SceneManager.GetActiveScene().name != "Title")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            settingWindow.SettingSet();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if(num == 0)
        {
            var homeCanvasManager = GameObject.Find("HomeCanvas").GetComponent<HomeCanvasManager>();
            homeCanvasManager.HideStageSelectCanvas();
            homeCanvasManager.StageClear(4);
            num = -1;
        }
        else if(num == 1)
        {
            var homeCanvasManager = GameObject.Find("HomeCanvas").GetComponent<HomeCanvasManager>();
            homeCanvasManager.ShowStageSelectCanvas();
            homeCanvasManager.StageClear(4);
            num = -1;
        }
        else if(num == 2)
        {
            settingWindow.SettingSet();
            num = -1;
        }

//        poseObj.SetActive(false);
        Time.timeScale = 1f;
        canvasOpen = false;
    }

    public void ToResume()
    {
        poseObj.SetActive(false);
        Time.timeScale = 1f;
        canvasOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ToTitle()
    {
        num = 0;
        poseObj.SetActive(false);
        Time.timeScale = 1f;
        canvasOpen = false;
        SceneManager.LoadScene("Title");
        warningWindow.HideBackToTitle();
        
    }
    public void ToStageSelect()
    {
        num = 1;
        poseObj.SetActive(false);
        Time.timeScale = 1f;
        canvasOpen = false;
        SceneManager.LoadScene("Title");
        warningWindow.HideStageSelect();
        
    }
    public void ToRetry()
    {
        num = 2;
        poseObj.SetActive(false);
        Time.timeScale = 1f;
        canvasOpen = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        warningWindow.HideRetry();
        
    }

}
