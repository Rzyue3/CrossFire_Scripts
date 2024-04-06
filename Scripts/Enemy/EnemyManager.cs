using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public List<int> EnemyHP = new List<int>();
    public List<int> EnemyATK = new List<int>();
    public List<float> EnemyAGI = new List<float>();
    public List<float> EnemyACC = new List<float>();
    private List<string[]> csvData = new List<string[]>();  //CSVファイルの中身を入れるリスト
    private int StageNum;
    private int offset;
    [SerializeField]
    private List<int> spownCountList = new List<int>();
    [SerializeField]
    private List<GameObject> enemyList = new List<GameObject>();
    private GameObject cashObj;
    public int totalSpown;
    public int DestroyCount;
    [SerializeField]
    private GameObject parentObject;
    private bool SpownEnd;



    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if(instance == null)
            instance = this;
        SceneManager.sceneLoaded += SceneLoaded;
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
            Debug.Log(DestroyCount);
    }

    public bool Set()
    {
        #if UNITY_EDITOR
            StreamReader fs = new StreamReader(Application.dataPath + "/StreamingAssets/csv/enemy_data.csv");    
        #elif UNITY_STANDALONE_WIN
            StreamReader fs = new StreamReader(Application.dataPath + "/StreamingAssets/csv/enemy_data.csv");    
        #elif UNITY_STANDALONE_OSX
            StreamReader fs = new StreamReader(Application.dataPath + "/Resources/Data/StreamingAssets/csv/enemy_data.csv");    
        #endif
        {
            while (fs.Peek() != -1)
            {
                var str = fs.ReadLine();
                csvData.Add(str.Split(',')); 
            }
            int j = 0;
            //Debug.Log(fireRate);
            for(int i = SettingWindow.instance.DiffSet(); i < 16; i += 2)
            {
                EnemyHP[j] = int.Parse(csvData[i][1]);
                EnemyATK[j] = int.Parse(csvData[i][2]);
                EnemyAGI[j] = float.Parse(csvData[i][3]);
                EnemyACC[j] = float.Parse(csvData[i][4]);
                j++;
            }
            fs.Close();
        }
        return true;
    }

    void SceneLoaded (Scene nextScene, LoadSceneMode mode) 
    {
        StageSet();
    }

    void StageSet()
    {
        totalSpown = 0;
        DestroyCount = 0;
        var str = SceneManager.GetActiveScene().name;
        if(str == "stage01_scene")
        {
            StageNum = 0;
            offset = 0;
        }
        else if(str == "stage02_scene")
        {
            StageNum = 3;
            offset = 1;
        }
        else if(str == "stage03_scene")
        {
            StageNum = 6;
            offset = 2;
        }
        else if(str == "stage04_scene")
        {
            StageNum = 9;
            offset = 3;
        }
        else if(str == "stage05_scene")
        {
            StageNum = 12;
            offset = 4;
        }
        else if(str == "stage06_scene")
        {
            StageNum = 15;
            offset = 4;
        }
        else if(str == "stage07_scene")
        {
            StageNum = 18;
            offset = 4;
        }
        else
            StageNum = -1;
        
        StartCoroutine(EnemyGen(StageNum)); //ステージ番号
    }

    IEnumerator EnemyGen(int i)
    {
        Debug.Log("EnemySpownWait");
        if(StageNum == -1) yield break;
        // ステージ番号

        // 3ウェーブ
        for(int j = 0; j < 3; j++)
        {
            SpownEnd = false;
            
            Spown(j,i);
            totalSpown += spownCountList[i + j];
            yield return new WaitUntil(() => totalSpown == DestroyCount && SpownEnd);
            if(i != StageNum) yield break;
            Debug.Log("j" + j);
            
            if(j == 2) 
            {
                Debug.Log("*** endFlag");
                GameObject.Find("ClearManager").GetComponent<ClearManager>().EnemyDeath();
                yield break;
            }
            yield return null;
        }
    }

    private void Spown(int i,int stagenum)
    {
        if(StageNum == -1) return;
        for(int j = 0; j < spownCountList[i + stagenum];j++)
        {
            // 配置取得
            Transform childTransform = parentObject.transform.GetChild(stagenum/3).
                                                                GetChild(j + DestroyCount);
            GameObject childObject = childTransform.gameObject;
            var enemyNum = childObject.GetComponent<EnemyType>().EnemyNum;
            // 指定位置に生成
            Instantiate(enemyList[enemyNum],childObject.transform.position,Quaternion.identity);
        }
        SpownEnd = true;
    }

    public void DestroyCheck(GameObject obj)
    {
        if(cashObj == obj) return;
        cashObj = obj;
        DestroyCount++;
    }


}
