using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WeaponStats : MonoBehaviour
{
    public static WeaponStats instance;
    public int Power;
    public int StanPower;
    public float BulletSpeed;
    public int MaxBullet;
    public float FireRate;
    public float ReloadStartTime;
    public float ReloadSpeed;
    
    public int Pellet;
    public int ExpRange;
    public int StartBullet;
    public float FOV;
    public float ZoomTime;
    public float ZoomCancelTime;
    
    
    private List<string[]> csvData = new List<string[]>();  //CSVファイルの中身を入れるリスト

    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public bool Set()
    {
        #if UNITY_EDITOR
            StreamReader fs = new StreamReader(Application.dataPath + "/StreamingAssets/csv/WeaponStats.csv");    
        #elif UNITY_STANDALONE_WIN
            StreamReader fs = new StreamReader(Application.dataPath + "/StreamingAssets/csv/WeaponStats.csv");    
        #elif UNITY_STANDALONE_OSX
            StreamReader fs = new StreamReader(Application.dataPath + "/Resources/Data/StreamingAssets/csv/WeaponStats.csv");    
        #endif
        {
            while (fs.Peek() != -1)
            {
                var str = fs.ReadLine();
                csvData.Add(str.Split(',')); 
            }
            
            //Debug.Log(fireRate);
            for(int i = 2; i < 27; i += 4)
            {
                //Debug.Log(csvData[i][0]);
                Weapon.instance.BulletPower.Add(int.Parse(csvData[i][0]));
                Weapon.instance.StanPower.Add(int.Parse(csvData[i][1]));
                Weapon.instance.BulletSpeed.Add(int.Parse(csvData[i][2]));
                Weapon.instance.MaxBullet.Add(int.Parse(csvData[i][3]));
                var fireRate = 1 / float.Parse(csvData[i][4]);
                Weapon.instance.FireRate.Add(fireRate);
                Weapon.instance.ReloadStartTime.Add(float.Parse(csvData[i][5]));
                var reloadSpeed = 1 / float.Parse(csvData[i][6]);
                Weapon.instance.ReloadSpeed.Add(reloadSpeed);

                if(i == 18)
                {
                    Weapon.instance.BulletPower.Add(int.Parse(csvData[6][0]));
                    Weapon.instance.StanPower.Add(int.Parse(csvData[6][1]));
                    Weapon.instance.BulletSpeed.Add(int.Parse(csvData[6][2]));
                    Weapon.instance.MaxBullet.Add(int.Parse(csvData[6][3]));
                    var fireRate2 = 1 / float.Parse(csvData[6][4]);
                    Weapon.instance.FireRate.Add(fireRate2);
                    Weapon.instance.ReloadStartTime.Add(float.Parse(csvData[6][5]));
                    var reloadSpeed2 = 1 / float.Parse(csvData[6][6]);
                    Weapon.instance.ReloadSpeed.Add(reloadSpeed2);
                }
                
            }
            Pellet = int.Parse(csvData[2][7]);
            FOV = float.Parse(csvData[10][7]);
            ZoomTime = float.Parse(csvData[10][8]);
            ZoomCancelTime = float.Parse(csvData[10][9]);
            ExpRange = int.Parse(csvData[14][7]);
            StartBullet = int.Parse(csvData[18][7]);
            fs.Close();
        }
        return true;
    }
    
}
