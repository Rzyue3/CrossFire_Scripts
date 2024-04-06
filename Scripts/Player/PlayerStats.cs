using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;
    [SerializeField]
    private PlayerMove playerMove;
    [SerializeField]
    private GameObject deadCanvas;
    
    public LinkedList<GameObject> nearWallList = new LinkedList<GameObject>();

    public bool isGround;
    // 0: SG 1: SMG 2:SR 3:MR 4:N
    public int nowWeapons;

    [SerializeField]
    private UnityEngine.UI.Slider boostSlider;
    [SerializeField]
    private UnityEngine.UI.Slider shildSlider;
    [SerializeField]
    private UnityEngine.UI.Slider PlayerHpSlider;
    
    private Vector3 PlayerPos;
    private float shildTimer;
    private float timer;
    public float boostWeaponTimer;
    public bool boostWeaponFlag;
    public bool boostWeaponRPFlag;
    public bool doubleJump;
    public bool meleeFlag;
    public bool playerDead;

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }
    // Start is called before the first frame update
    void Start()
    {
        
        boostSlider.maxValue = 100f;
        boostSlider.value = 100f;
        shildSlider.maxValue = 100f;
        shildSlider.value = 100f;
        PlayerHpSlider.maxValue = 100f;
        PlayerHpSlider.value = 100f;
        shildTimer = 2.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerDead) return;
        if(boostWeaponFlag || boostWeaponRPFlag)
        {
            boostWeaponTimer += Time.deltaTime;
            if(boostWeaponTimer > 1.5f)
            {
                boostWeaponRPFlag = false;
                boostWeaponFlag = false;
            }
        }
        if(!playerMove.slidingFlag && playerMove.boostDelayFlag)
        {
            boostSlider.value += 25f * Time.deltaTime;

        }

        
        timer += Time.deltaTime;
        if(timer > shildTimer)
        {
            shildSlider.value += 33.3f * Time.deltaTime;
        }
    }

    public bool BoostCost(float cost)
    {
        var f = boostSlider.value;
        if(f - cost < 0 )
            return false;
        else
            boostSlider.value -= cost;
        return true;
    }
    public void PlayerDamage(float f)
    {
        if(timer < 0.5f) return;
        CameraShake.instance.ShakeStart();
        PlayerDamageHit.instance.Damage();
        timer = 0f;
        BGMPlayer.Instance.PlaySE("S_PlayerDamage");
        if(shildSlider.value == 0f)
        {
            PlayerHpSlider.value -= f;
            if(PlayerHpSlider.value <= 0)
            {
                BGMPlayer.Instance.PlayBGM("B_GameOver");
                playerDead = true;
                Time.timeScale = 0;
                SettingWindow.instance.DeadCanvasSet(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

            }
        }
        else
        {
            shildSlider.value -= f;
        }
    }

    public bool DoubleJump()
    {
        var objF = nearWallList.First.Value;
        var objL = nearWallList.Last.Value;
        var distanceF = (PlayerPos - objF.transform.position).sqrMagnitude;
        var distanceL = (PlayerPos - objL.transform.position).sqrMagnitude;

        if(distanceF <= distanceL)
        {
            var obj = nearWallList.First.Value;
            var check = obj.GetComponent<WallJump>().flag;
            if(check)
                obj.GetComponent<WallJump>().Jump();
            return check;
        }
        else
        {
            var obj = nearWallList.Last.Value;
            var check = obj.GetComponent<WallJump>().flag;
            if(check)
                obj.GetComponent<WallJump>().Jump();
            return check;
        }
    }

    public Vector3 PlayerPosSet()
    {
        return PlayerPos = this.transform.position;
    }
    public GameObject PlayerObjSet()
    {
        return this.gameObject;
    }
}
