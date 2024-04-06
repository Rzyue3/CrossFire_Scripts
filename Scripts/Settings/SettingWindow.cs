using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingWindow : MonoBehaviour
{
    public static SettingWindow instance = null;
    
    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    private GameObject deadCanvas;

    [SerializeField]
    private TMP_Dropdown diffDrop;
    [SerializeField]
    private UnityEngine.UI.Slider sensSlider;
    [SerializeField]
    private UnityEngine.UI.Slider fovSlider;
    [SerializeField]
    private Text reverseText;
    private bool reverse;


    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if(instance == null)
        {
            instance = this;
        }
    }


    // Update is called once per frame
    void Update()
    {
        /*
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(canvasOpen)
            {
                Confirm();
            }
            else
            {
                canvas.SetActive(true);
                Time.timeScale = 0f;
                canvasOpen = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        */
    }
    public void ReverseSet()
    {
        if(reverse)
        {
            reverse = false;
            reverseText.text = "通常";
        }
        else
        {
            reverse = true;
            reverseText.text = "反転";
        }
    }

    public int DiffSet()
    {
        int i;
        switch(diffDrop.value)
        {
            case 0:
            {
                i = 3;
                break;
            }
            case 1:
            {
                i = 19;
                break;
            }
            case 2:
            {
                i = 35;
                break;
            }
            default:
            {
                i = 19;
                break;
            }
        }
        return i;
    }
    public void DeadCanvasSet(bool b)
    {
        Instantiate(deadCanvas,new Vector3(0f,0f,0f),this.transform.rotation);

    }

    public void SettingSet()
    {
        var playerMove = GameObject.Find("Player").GetComponent<PlayerMove>();
        playerMove.sensValue = sensSlider.value;
        playerMove.reverseCam = reverse;
    }


    public void Confirm()
    {
        BGMPlayer.Instance.PlaySE("S_Click",0.8f);
        canvas.SetActive(false);
        //Time.timeScale = 1.0f;

        if(SceneManager.GetActiveScene().name != "Title")
            SettingSet();
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }

}
