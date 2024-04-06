using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public static Weapon instance;
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private ScopeEffect scopeEffect;
    [SerializeField]
    private GameObject FireEndPos;
    
    // 0:SG 1:LeftSMG 2:SR 3:RL 4:MG 5:RightSMG 6:FrontMelee 7:NormalMelee
    [SerializeField]
    private List<GameObject> BulletList = new List<GameObject>(); // 弾リスト
    [SerializeField]
    public List<GameObject> WeaponList = new List<GameObject>(); // 武器の表示
    [SerializeField]
    public List<Transform> WeaponPos = new List<Transform>();    // 武器のアニメーション座標 
    [SerializeField]
    private List<GameObject> UIAcon = new List<GameObject>();
    public List<int> BulletPower = new List<int>();
    public List<int> StanPower = new List<int>();
    public List<int> BulletSpeed = new List<int>();
    public List<int> MaxBullet = new List<int>();                // 最大弾薬数
    public List<float> FireRate = new List<float>();             // 撃つ間隔
    public List<float> ReloadStartTime = new List<float>();      // リロード始まるまで
    public List<float> ReloadSpeed = new List<float>();              // 弾薬補充速度
    [SerializeField]
    private List<float> ReloadBulletTimer = new List<float>();   // 始まるまでの時間計測
    [SerializeField]
    private List<float> ReloadSpeedTimer = new List<float>();        // 補充時間計測

    [SerializeField]
    public List<bool> FireFlag = new List<bool>();                // 撃てるかどうか
    [SerializeField]
    private List<bool> ReloadFlag = new List<bool>();             // リロードしているか

    [SerializeField]
    public List<int> NowBullet = new List<int>();                 // 現在の弾薬数
    [SerializeField]
    private List<float> GunTimer = new List<float>();             // 発射間隔

    
    [SerializeField]
    private Transform firePos;              // 発射地点
    [SerializeField]
    private Vector3 endPos;                 // 着弾地点
    [SerializeField]
    private TextMeshProUGUI weaponText;     // 現在の武器
    [SerializeField]
    private List<Text> bulletText = new List<Text>(); // 残弾表示
    private int Pellet;         // SGのペレット数
    private int ExpRange;       // 爆発範囲
    private int StartBullet;    // スタート時のミニガンの弾薬数
    private bool zoomFlag;
    private float zoomFOV;
    private float defFOV;
    private float zoomTime;
    private float zoomCanselTime;
    private Tweener zoom;
    private Tweener scope;

    public Tweener cashWeapon;
    public Tweener nowWeapon;

    private RaycastHit[] srHit;
    [SerializeField]
    private GameObject hitEffect;
    [SerializeField]
    private GameObject srEffect;


    void Awake()
    {
        instance = this;
    }

    async void Start()
    {
        await UniTask.WaitUntil(() => WeaponStats.instance.Set());
        for(int i = 0;i < MaxBullet.Count;i++)
        {
            NowBullet[i] = MaxBullet[i];
        }
        NowBullet[4] = WeaponStats.instance.StartBullet;
        zoomFOV = WeaponStats.instance.FOV;
        zoomTime = WeaponStats.instance.ZoomTime;
        zoomCanselTime = WeaponStats.instance.ZoomCancelTime;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerStats.instance.playerDead);
        float f = Time.deltaTime;
        for(int i = 0; i < MaxBullet.Count;i++)
        {
            if(NowBullet[i] != MaxBullet[i])
            {
                ReloadBulletTimer[i] += f;
                if(ReloadBulletTimer[i] > ReloadStartTime[i])
                {
                    ReloadSpeedTimer[i] += f;
                    if(ReloadSpeedTimer[i] > ReloadSpeed[i])
                    {
                        NowBullet[i]++;
                        BulletTextUpdate(i,1);
                        ReloadSpeedTimer[i] = 0f;
                        ReloadFlag[i] = true;
                    }
                }
            }
            if(GunTimer[i] < FireRate[i])
            {
                GunTimer[i] += f;
                if(GunTimer[i] > FireRate[i])
                {
                    FireFlag[i] = true;
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.E))
            WeaponChangeAnimation(0, 1);
        if(Input.GetKeyDown(KeyCode.R))
            WeaponChangeAnimation(1, 0);

/*
        if(timer < SMGFire)
        {
            timer += Time.deltaTime;
            if(timer > SMGFire)
                SMGFlag = true;
        }
*/
        //TextUpdate(PlayerStats.instance.nowWeapons);
        Shot(PlayerStats.instance.nowWeapons);
    }

    void Shot(int i)
    {
        if(PlayerStats.instance.meleeFlag || !FireFlag[i] || NowBullet[i] == 0) return;
        /*
        RaycastHit hit;
        var ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        if (Physics.Raycast(ray, out hit)) 
        {
            endPos = hit.point;
        }
        */
        
/*
        if(Input.GetMouseButton(0) && i == 1 && FireFlag[i] && ReloadFlag[i]
        || Input.GetMouseButton(0) && i == 4 && FireFlag[i] && ReloadFlag[i])
        {            
            //var obj = BulletIns(i);
            //obj.GetComponent<BulletMove>().endPos = endPos;
            NowBullet[i]--;
            BulletTextUpdate(i);
            psList[i].Play();
            GunTimer[i] = 0f;
            FireFlag[i] = false;

            if(NowBullet[i] == 0 )
            {
                StartCoroutine(ReloadSet(i));
                return;
            }

            return;
        }
        
*/
        endPos = FireEndPos.transform.position; 
        if(zoomFlag)
        {
            if(Input.GetMouseButtonUp(0))
            {
                // DoTweenKill
                zoom.Kill();
                scope.Kill();
                zoomFlag = false;
                // ズームアウト再設定
                var fov = (float)camera.fieldOfView;
                var sc = scopeEffect.vignette.intensity.value;
                DOTween.To(() => fov, (x) => camera.fieldOfView = x, defFOV, zoomCanselTime);
                DOTween.To(() => sc,(y) => scopeEffect.vignette.intensity.value = y, 0, zoomCanselTime);
                // 弾初期設定
                var obj = BulletIns(2);
                GunFireInit(2);
                // エフェクト生成
                Instantiate(srEffect,firePos.position,camera.transform.rotation);
                var bulletMove = obj.GetComponent<BulletMove>();
                // 終点設定
                bulletMove.endPos = endPos;
                bulletMove.offSet = BulletSpeed[2];
                bulletMove.bulletPower = BulletPower[2];
                // SE再生
                BGMPlayer.Instance.PlaySE("S_SR");
            }
            return;
        }
        if(Input.GetMouseButton(0))
        {
            switch(i)
            {
                case 0: // SG
                {
                    for(int j = 0; j < 12;j++)
                    {
                        var obj = BulletIns(i);
                        
                        var bulletMove = obj.GetComponent<BulletMove>();
                        obj.GetComponent<BulletMove>().endPos = 7.5f * Random.insideUnitSphere + endPos;
                        bulletMove.offSet = BulletSpeed[i];
                        bulletMove.bulletPower = BulletPower[i];
                    }
                    BGMPlayer.Instance.PlaySE("S_Shotgun");
                    GunFireInit(i);
                    break;
                }
                case 1: //LeftSMG
                {
                    if(NowBullet[i] < MaxBullet[i] * 1f && ReloadFlag[i]) return;
                    else ReloadFlag[i] = false;
                    var obj = BulletIns(i);
                    GunFireInit(i);
                    var bulletMove = obj.GetComponent<BulletMove>();
                    bulletMove.endPos = 1f * Random.insideUnitSphere + endPos;
                    bulletMove.offSet = BulletSpeed[i];
                    bulletMove.bulletPower = BulletPower[i];
                    BGMPlayer.Instance.PlaySE("S_SMG");
                    break;
                }
                case 2: //SR
                {
                    // ズーム
                    zoomFlag = true;
                    defFOV = camera.fieldOfView;
                    Debug.Log(zoomFOV);
                    zoom = DOTween.To(() => defFOV, (x) => camera.fieldOfView = x, zoomFOV, zoomTime);
                    scope = DOTween.To(() => 0f,(y) => scopeEffect.vignette.intensity.value = y, 1f, zoomTime);

                    break;
                }
                case 3: //RL
                {
                    // 後ろ格闘に変更
                    var obj = BulletIns(i);
                    GunFireInit(i);
                    var bulletMove = obj.GetComponent<BulletMove>();
                    bulletMove.RPG = true;
                    bulletMove.endPos = endPos;
                    bulletMove.offSet = BulletSpeed[i];
                    bulletMove.bulletPower = BulletPower[i];
                    break;
                }

                case 4: //MG
                {
                    var obj = BulletIns(i);
                    GunFireInit(i);
                    var bulletMove = obj.GetComponent<BulletMove>();
                    bulletMove.endPos = 1.5f * Random.insideUnitSphere + endPos;
                    bulletMove.offSet = BulletSpeed[i];
                    bulletMove.bulletPower = BulletPower[i];
                    BGMPlayer.Instance.PlaySE("S_Minigun");
                    break;
                }
                case 5: //RightSMG
                {
                    if(NowBullet[i] < MaxBullet[i] * 1f && ReloadFlag[i]) return;
                    else ReloadFlag[i] = false;
                    var obj = BulletIns(i);
                    GunFireInit(i);
                    var bulletMove = obj.GetComponent<BulletMove>();
                    bulletMove.endPos = 1f * Random.insideUnitSphere + endPos;
                    bulletMove.offSet = BulletSpeed[i];
                    bulletMove.bulletPower = BulletPower[i];
                    BGMPlayer.Instance.PlaySE("S_SMG");
                    break;
                }
            }
        }


/*
        if(!Input.GetMouseButton(0) && !PlayerStats.instance.boostWeaponFlag)
        {
            if(i == 1 || i == 5)
                PlayerStats.instance.boostWeaponRPFlag = false;

        }

        if(Input.GetMouseButton(0) && i == 1 || Input.GetMouseButton(0) && i == 4 || Input.GetMouseButton(0) && i == 5)
        {
            switch(i)
            {
                case 1: //LeftSMG
                {
                    if(NowBullet[i] < MaxBullet[i] * 0.5f && ReloadFlag[i]) return;
                    else ReloadFlag[i] = false;
                    var obj = BulletIns(i);
                    GunFireInit(i);
                    var bulletMove = obj.GetComponent<BulletMove>();
                    bulletMove.endPos = 0.3f * Random.insideUnitSphere + endPos;
                    bulletMove.offSet = BulletSpeed[i];
                    break;
                }
                case 4: //MG
                {
                    var obj = BulletIns(i);
                    GunFireInit(i);
                    var bulletMove = obj.GetComponent<BulletMove>();
                    bulletMove.endPos = 0.3f * Random.insideUnitSphere + endPos;
                    bulletMove.offSet = BulletSpeed[i];
                    break;
                }
                case 5: //RightSMG
                {
                    if(NowBullet[i] < MaxBullet[i] * 0.5f && ReloadFlag[i]) return;
                    else ReloadFlag[i] = false;
                    var obj = BulletIns(i);
                    GunFireInit(i);
                    var bulletMove = obj.GetComponent<BulletMove>();
                    bulletMove.endPos = 0.3f * Random.insideUnitSphere + endPos;
                    bulletMove.offSet = BulletSpeed[i];
                    break;
                }

            }

        }
        else if(Input.GetMouseButtonDown(0))
        {
            switch(i)
            {
                case 0: // SG
                {
                    for(int j = 0; j < 12;j++)
                    {
                        var obj = BulletIns(i);
                        
                        var bulletMove = obj.GetComponent<BulletMove>();
                        obj.GetComponent<BulletMove>().endPos = 0.5f * Random.insideUnitSphere + endPos;
                        bulletMove.offSet = BulletSpeed[i];
                    }
                    GunFireInit(i);
                    break;
                }
                case 2: //SR
                {
                    // ズーム
                    zoomFlag = true;
                    defFOV = camera.fieldOfView;
                    Debug.Log(zoomFOV);
                    zoom = DOTween.To(() => defFOV, (x) => camera.fieldOfView = x, zoomFOV, zoomTime);
                    break;
                }
                case 3: //RL
                {
                    
                    var obj = BulletIns(i);
                    GunFireInit(i);
                    var bulletMove = obj.GetComponent<BulletMove>();
                    bulletMove.RPG = true;
                    bulletMove.endPos = endPos;
                    bulletMove.offSet = BulletSpeed[i];
                    break;
                }
            }
        }
        */
    }
    

    public void RPG(int i)
    {
        if(NowBullet[i] == 0) return;
        WeaponList[i].transform.DOLocalMove(WeaponPos[4].localPosition, 0.2f);
        WeaponList[i].transform.DOLocalMove(WeaponPos[5].localPosition, 0.25f).SetDelay(0.75f);
        var obj = BulletIns(i);
        GunFireInit(i);
        var bulletMove = obj.GetComponent<BulletMove>();
        bulletMove.RPG = true;
        bulletMove.endPos = endPos;
        bulletMove.offSet = BulletSpeed[i];
    }

    public int AnimNumberSet(int i)
    {
        switch(i)
        {
            case 0:
            {
                i = 0;
                break;
            }
            case 1:
            {
                i = 2;
                break;
            }
            case 2:
            {
                i = 0;
                break;
            }
            case 3:
            {
                i = 4;
                break;
            }
            case 4:
            {
                i = 6;
                break;
            }
            case 5:
            {
                i = 0;
                break;
            }
        }
        return i;
    }

    // i:cashWeapon,j:NowWeapon
    public void WeaponChangeAnimation(int i , int j)
    {
        //cashWeapon.Kill();
        //nowWeapon.Kill();
        //DOTween.Kill(WeaponList[j]);
        //DOTween.Kill(WeaponList[cashNumWeapon]);
        cashWeapon.Complete();
        nowWeapon.Complete();
        
        int animNumber = AnimNumberSet(i);
        cashWeapon = WeaponList[i].transform.DOLocalMove(WeaponPos[animNumber+1].localPosition, 0.25f);
        animNumber = AnimNumberSet(j);
        nowWeapon = WeaponList[j].transform.DOLocalMove(WeaponPos[animNumber].localPosition, 0.25f).SetDelay(0.25f);
/*        
        cashWeapon = DOTween.To(() => WeaponPos[animNumber].position, (x) => WeaponList[i].transform.position = x, WeaponPos[animNumber+1].position, 0.25f);
        animNumber = AnimNumberSet(j);
        nowWeapon = DOTween.To(() => WeaponPos[animNumber].position,(y) => WeaponList[j].transform.position = y, WeaponPos[animNumber+1].position, 0.25f).SetDelay(0.25f);
*/
    }

    void GunFireInit(int i)
    {
        if(i != 1 && i != 4)
            PlayerStats.instance.boostWeaponRPFlag = false;
        GunTimer[i] = 0f;
        ReloadSpeedTimer[i] = 0f;
        ReloadBulletTimer[i] = 0f;
        FireFlag[i] = false;
        NowBullet[i]--;
        BulletTextUpdate(i,0);
    }

    public void BulletTextUpdate(int i,int j)
    {
        switch(i)
        {
            case 0: // SG
            {
                if(j == 0)
                {
                    if(NowBullet[i] == 1)
                        UIAcon[0].SetActive(false);
                    else
                        UIAcon[1].SetActive(false);
                }
                else
                {
                    if(NowBullet[i] == 1)
                        UIAcon[1].SetActive(true);
                    else
                        UIAcon[0].SetActive(true);
                }

                break;
            }
            case 1: // LeftSMG
            {
                bulletText[0].text = NowBullet[i].ToString();
                break;
            }
            case 2: // SR
            {
                if(j == 0)  UIAcon[2].SetActive(false);
                else UIAcon[2].SetActive(true);
                break;
            }
            case 3: // RL
            {
                if(j == 0)  UIAcon[3].SetActive(false);
                else UIAcon[3].SetActive(true);
                break;
            }
            case 4: // MG
            {
                bulletText[1].text = NowBullet[i].ToString();
                break;
            }
            case 5: // RightSMG
            {
                bulletText[2].text = NowBullet[i].ToString();
                break;
            }
            case 6: // FrontM
            {
                break;
            }
            case 7: // NormalM
            {
                break;
            }

        }
        
    }

    void TextUpdate(int i)
    {  
        WeaponUpdate(i);
        switch(i)
        {
            case 0:
            {
                weaponText.text = "GR";
                break;
            }
            case 1:
            {
                weaponText.text = "AR";
                break;
            }
            case 2:
            {
                weaponText.text = "SR";
                break;
            }
            case 3:
            {
                weaponText.text = "RL";
                break;
            }
            case 4:
            {
                weaponText.text = "Minigun";
                break;
            }
        }
    }
    void WeaponUpdate(int i)
    {
        for(int j = 0;j < 5;j++)
        {
            if(j == i) continue;
//            WeaponList[j].SetActive(false);
        }
//        WeaponList[i].SetActive(true);
    }

    GameObject BulletIns(int i)
    {
        // ブースト強化状態であれば
        if(PlayerStats.instance.boostWeaponFlag || PlayerStats.instance.boostWeaponRPFlag && i == 1 || i == 4)
        {
            PlayerStats.instance.boostWeaponFlag = false;
            var obj = Instantiate(BulletList[i],firePos.position,this.transform.rotation);
            return obj;
        }
        else
        {
            var obj = Instantiate(BulletList[i],firePos.position,this.transform.rotation);
            return obj;
        }
        
    }


}
