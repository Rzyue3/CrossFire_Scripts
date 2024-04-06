using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMove : MonoBehaviour
{
    private PlayerStats playerStats;
    public float playerheight;  // 地面との設置判定用
    public LayerMask Ground;
    [SerializeField]
    public Rigidbody rb;
    [SerializeField]
    private float groundDrag;   // 地面での抵抗
    private float x;
    private float z;
    public Vector3 moveDirection;      // 移動方向
    [SerializeField]
    private float speed;        // 移動速度
    [SerializeField]
    private float jumpForce;//ジャンプ力の値
    [SerializeField]
    private float wallJumpForce;
    [SerializeField]
    private float airMultiplier;//空中での操作性を少し悪くするための値
    public float RotationSpeed = 1.0f;  // カメラ
    public GameObject CinemachineCameraTarget;
    private float _cinemachineTargetPitch;
    private float _rotationVelocity;
	public float TopClamp = 90.0f;
	public float BottomClamp = -90.0f;

    private float totalFallTime;
    [SerializeField]
    private float gravityPower;

    private bool boostFlag;
    private float boostTimer;
    [SerializeField]
    private float boostPower;       // ブーストの力
    [SerializeField]
    private float boostCost;        // ブーストに必要なコスト
    [SerializeField]
    private float boostRD;          // ブースト後ブーストゲージの回復まで
    [SerializeField]
    public bool boostDelayFlag;     // ブーストゲージが回復可能か
    [SerializeField]
    private float boostReload;      // ブーストクールタイム
    [SerializeField]
    private float slidingStartPower;    // スライディング開始時の力
    [SerializeField]
    private float slidingNowPower;      // スライディング移動中の力
    [SerializeField]
    private float slidingStartCost;     // 開始するのに必要なコスト
    [SerializeField]
    private float slidingCost;          // 継続するのに必要なコスト
    public bool slidingFlag;
    private bool slidingNowFlag;
    private WaitForSeconds slidingStartCD; // 強制的に姿勢移行する為に待つ
    private WaitForSeconds slidingNextCD;   // 連続発動できないようにする
    [SerializeField]
    private GameObject slidingObj;          // プレイヤーobj入れる

    private bool hitStop;
    private int cashWeapon;

    public float sensValue;
    public bool reverseCam;


    // Start is called before the first frame update
    void Start()
    {
        boostDelayFlag = true;
        slidingStartCD = new WaitForSeconds(0.3f);
        slidingNextCD = new WaitForSeconds(0.5f);
        playerStats = PlayerStats.instance;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(hitStop || playerStats.playerDead) return;
        if(Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(PlayerHitDamage());
        }

        

        playerStats.isGround = Physics.Raycast(slidingObj.transform.position, Vector3.down, playerheight * 0.5f + 0.2f, Ground);
        // ブーストクールタイム関係
        if(boostFlag)
        {
            boostTimer += Time.deltaTime;
            if(boostTimer > boostRD && !boostDelayFlag)
            {
                boostDelayFlag = true;
            }
            if(boostTimer > boostReload)
                boostFlag = false;
        }

        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        // 地上か判定して抗力を発生
        if (playerStats.isGround || boostFlag || !PlayerStats.instance.meleeFlag)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
        // 移動系
        Move();
        // 速度制限
        SpeedControl();
        // カメラ操作
        CameraRotation();
    }

    private void Move()
    {
        //if(PlayerStats.instance.meleeFlag) return;
        //向いている方向に進む
        moveDirection =  CinemachineCameraTarget.transform.forward * z +  CinemachineCameraTarget.transform.right * x;
        /*
        if(Input.GetKey(KeyCode.A)  && Weapon.instance.NowBullet[3] != 1 || Input.GetKey(KeyCode.A) && playerStats.isGround)
            playerStats.nowWeapons = 1;
        else if(Input.GetKey(KeyCode.D) && Weapon.instance.NowBullet[3] != 1 ||Input.GetKey(KeyCode.D) && playerStats.isGround)
            playerStats.nowWeapons = 5;
        else if(Input.GetKey(KeyCode.W) && Weapon.instance.NowBullet[3] != 1 || Input.GetKey(KeyCode.W) && playerStats.isGround)
            playerStats.nowWeapons = 0;
        else if(Input.GetKey(KeyCode.S) && Weapon.instance.NowBullet[3] != 1 || Input.GetKey(KeyCode.S) && playerStats.isGround)
            playerStats.nowWeapons = 2;
        */
        

        if(Input.GetKey(KeyCode.A))
            playerStats.nowWeapons = NowWeaponCheck(1);
        else if(Input.GetKey(KeyCode.D))
            playerStats.nowWeapons = NowWeaponCheck(5);
        else if(Input.GetKey(KeyCode.W))
            playerStats.nowWeapons = NowWeaponCheck(0);
        else if(Input.GetKey(KeyCode.S))
            playerStats.nowWeapons = NowWeaponCheck(2);


        if(!Input.anyKey)
            playerStats.nowWeapons = NowWeaponCheck(4);

        // 後ろ格闘に移行の為廃止
        /*
        if(Weapon.instance.NowBullet[3] == 1 && !playerStats.isGround)
            playerStats.nowWeapons = 3;
        */

        // ブースト
        if(Input.GetKeyDown(KeyCode.LeftShift) && !boostFlag)
        {
            if(!playerStats.BoostCost(boostCost)) return;
            BGMPlayer.Instance.PlaySE("S_Boost");
            Debug.Log("Boost");
            if(playerStats.nowWeapons == 4)
                rb.AddForce(CinemachineCameraTarget.transform.forward * boostPower);
            else
                rb.AddForce(new Vector3(moveDirection.x,0f,moveDirection.z) * boostPower);
            playerStats.boostWeaponFlag = true;
            playerStats.boostWeaponRPFlag = true;
            playerStats.boostWeaponTimer = 0f;
            boostFlag = true;
            boostDelayFlag = false;
            boostTimer = 0.0f;
            return;
        }

        // 通常移動
        if(boostFlag && rb.velocity.magnitude > speed || slidingFlag)
        {

        }
        else
        {
            if(playerStats.isGround && !slidingFlag)
                rb.velocity += new Vector3(moveDirection.x,0f,moveDirection.z).normalized * speed;
            else
                rb.velocity += new Vector3(moveDirection.x,0f,moveDirection.z).normalized * speed * airMultiplier;
                //rb.AddForce(new Vector3(moveDirection.x,0f,moveDirection.z).normalized * speed * airMultiplier, ForceMode.Force);
        }

        if(!playerStats.isGround)
        {
            totalFallTime += Time.deltaTime;
            rb.velocity -= new Vector3(0f,gravityPower * totalFallTime,0f);
    		//rb.velocity += new Vector3(0f,Physics.gravity.y * totalFallTime,0f);
        }
        else
            totalFallTime = 0.0f;

        // ジャンプ
        if(Input.GetKeyDown(KeyCode.Space) && playerStats.isGround || Input.GetKeyDown(KeyCode.Space) && !playerStats.isGround && playerStats.nearWallList.Count != 0)
        {
            if(!playerStats.isGround)
            {
                if(!playerStats.DoubleJump())
                {
                    playerStats.doubleJump = false;
                    return;
                }
                Debug.Log("ダブルジャンプ");
                rb.velocity = Vector3.zero;
                playerStats.isGround = false;
                slidingFlag = false;
                totalFallTime = 0.0f;
                rb.AddForce(CinemachineCameraTarget.transform.forward * 3 + (transform.up * 0.04f) * wallJumpForce, ForceMode.Impulse);
                //Weapon.instance.NowBullet[3] = 1;
                //Weapon.instance.BulletTextUpdate(3,1);
            }
            else
            {
                if(slidingNowFlag)
                {
                    rb.AddForce(CinemachineCameraTarget.transform.forward + (transform.up * 0.1f) * jumpForce * 0.5f, ForceMode.Impulse);
                    slidingNowFlag = false;
                }
                Weapon.instance.FireFlag[3] = true;
                playerStats.isGround = false;
                totalFallTime = 0.0f;
                rb.AddForce(transform.up * 0.25f * jumpForce, ForceMode.Impulse);
                //Weapon.instance.NowBullet[3] = 1;
                //playerStats.nowWeapons = 3;
                //Weapon.instance.BulletTextUpdate(3,1);
            }
        }

        
        // スライディング
        if(Input.GetKeyDown(KeyCode.LeftControl) && !slidingFlag)
        {
            if(!playerStats.BoostCost(slidingStartCost)) return;
            slidingFlag = true;
            rb.AddForce(new Vector3(moveDirection.x,0f,moveDirection.z) * slidingStartPower);
            SlidingStart();
            StartCoroutine(SlidingNow(moveDirection));
        }
    }
    
    void SlidingStart()
    {
        slidingObj.transform.DOScaleY(0.4f, 0.25f).SetEase(Ease.OutSine);
    }
    IEnumerator SlidingNow(Vector3 vec)
    {
        gravityPower *= 2f;
        groundDrag *= 0.2f;
        //slidingObj.transform.DOLocalMoveY(1f, 0.25f).SetEase(Ease.OutSine);
        
        // POSとSIZEを-0.5fを0.25秒で行う
        //OutSine // 姿勢変更
        //Vector3.Leap(Time.deltaTime * 4);
        slidingFlag = true;
        slidingNowFlag = true;
        yield return slidingStartCD;
        
        var speed =  new Vector3(vec.x,0f,vec.z).normalized * slidingNowPower;
        float cost = slidingCost;
        while(true)
        {
            // コストを速度から割合に
            cost = Mathf.InverseLerp(0, 2.5f, rb.velocity.magnitude);
            if(cost <= 0.5f) cost = 0.5f;
            Debug.Log(cost);
            if(slidingFlag && !slidingNowFlag)
            {
                yield return slidingNextCD;
                slidingFlag = false;
                yield return null;
                /*
                gravityPower *= 0.5f;
                groundDrag *= 5f;
                rb.AddForce(CinemachineCameraTarget.transform.forward + (transform.up * 0.2f) * jumpForce, ForceMode.Impulse);
                // ジャンプ動作の障害になるので戻す数値設定後強制終了
                slidingObj.transform.DOScaleY(0.8f, 0.25f).SetEase(Ease.OutSine);
                //slidingObj.transform.DOLocalMoveY(1.3f, 0.25f).SetEase(Ease.OutSine);
                slidingObj.transform.DOComplete();
                slidingNowFlag = false;
                yield return slidingNextCD;
                slidingFlag = false;
                yield break;
                */
            }
            // コストを支払えるか、キーを話していないか、ジャンプでキャンセルしたか
            else if(!Input.GetKey(KeyCode.C) || !playerStats.BoostCost(slidingCost * cost * Time.deltaTime) || rb.velocity.magnitude < 0.08f || !slidingFlag)
            {
                slidingFlag = false;
                // 体制を戻す
                yield return null;
                gravityPower *= 0.5f;
                groundDrag *= 5f;
                slidingObj.transform.DOScaleY(0.8f, 0.25f).SetEase(Ease.OutSine);
                //slidingObj.transform.DOLocalMoveY(1.3f, 0.25f).SetEase(Ease.OutSine);
                slidingNowFlag = false;
                yield return slidingNextCD;
                slidingFlag = false;
                yield break;
            }

            else
            {
                //継続
                Debug.Log("継続");
                //rb.AddForce(slidingNowPower * (vec - rb.velocity));
                //rb.velocity += new Vector3(vec.x,0f,vec.z).normalized * slidingNowPower;
                /*
                rb.velocity += speed;
                Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                Vector3 limitedVel = flatVel.normalized * slidingNowPower;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                */
                yield return null;
            } 
        }
    }


    private void SpeedControl()
    {
        // 速度を超える動作の際は終了
        if(boostFlag || slidingFlag) return;
        if(!playerStats.isGround) return;
        //プレイヤーのスピードを制限
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if(!playerStats.isGround)
        {
            Vector3 limitedVel = flatVel.normalized * speed * 1.5f;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }


    private void CameraRotation()
    {
        if(reverseCam)
            _cinemachineTargetPitch -= Input.GetAxis("Mouse Y") * RotationSpeed * sensValue * Time.deltaTime;
        else
            _cinemachineTargetPitch += Input.GetAxis("Mouse Y") * RotationSpeed * sensValue * Time.deltaTime;
        _rotationVelocity = Input.GetAxis("Mouse X") * RotationSpeed * sensValue * Time.deltaTime;

        // clamp our pitch rotation
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Update Cinemachine camera target pitch
        CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(-_cinemachineTargetPitch, 0.0f, 0.0f);

        // rotate the player left and right
        transform.Rotate(Vector3.up * _rotationVelocity);
    }

    int NowWeaponCheck(int i)
    {
        if(i != cashWeapon && !PlayerStats.instance.meleeFlag)
        {
            Weapon.instance.WeaponChangeAnimation(cashWeapon,i);
            cashWeapon = i;
        }
        return i;
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            // 被弾処理

        }
    }

    IEnumerator PlayerHitDamage()
    {
        CameraShake.instance.ShakeStart();
        hitStop = true;
        PlayerDamageHit.instance.Damage();
        yield return new WaitForSeconds(0.25f);
        hitStop = false;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}



}
