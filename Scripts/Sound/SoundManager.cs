using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
	// シングルトン
	private static SoundManager instance;
	public static SoundManager Instance
	{
		get
		{
			if (null == instance)
			{
				instance = (SoundManager)FindObjectOfType(typeof(SoundManager));
				if (null == instance) Debug.Log(" SoundManager Instance Error ");
			}
			return instance;
		}
	}
    void Start()
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += SceneLoaded;
    }


    void SceneLoaded (Scene nextScene, LoadSceneMode mode) 
    {
        BGMSet();
    }

    void BGMSet()
    {
        var str = SceneManager.GetActiveScene().name;
        if(str == "Title")
        {
            BGMPlayer.Instance.PlayBGM("B_Title",1f,true);
        }
        else if(str == "stage05_scene")
        {
            BGMPlayer.Instance.PlayBGM("B_FinalBoss",1f,true);
        }
        else
        {
            BGMPlayer.Instance.PlayBGM("B_NormalStage",1f,true);
        }
    }

}
