using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAMock : MonoBehaviour
{
    private WaitForSeconds AIntWait;
    private WaitForSeconds SpWait;
    private WaitForSeconds WayWait;

    [SerializeField]
    private int EnemyHP;
    public BulletMove bulletMove;
    public int damageMulti;

    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject Bullet;
    [SerializeField]
    private List<int> MaxBulletCount = new List<int>();
    [SerializeField]
    private List<int> BulletCount = new List<int>();

    [SerializeField]
    private List<int> Cash = new List<int>();

    [SerializeField]
    private float attackWait;
    [SerializeField]
    private float attackWaitRandMin;
    [SerializeField]
    private float attackWaitRandMax;
    [SerializeField]
    private float spWaitTime;
    [SerializeField]
    private float wayWaitTime;

    private Animator anim;

    public enum NTable
    {
        Sp,
        Way,
        B140,
        LightRp,
        LightPw,
        Lz
    }

    public void InitNTable()
    {
        for(int i = 0; i < 3; i++) 
        {
            Cash[i] = -1;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
        player = PlayerStats.instance.PlayerObjSet();
        InitNTable();
        AIntWait = new WaitForSeconds(attackWait + Random.Range(attackWaitRandMin,attackWaitRandMax));  // 攻撃wait時間

        SpWait = new WaitForSeconds(1f/8f);
        WayWait = new WaitForSeconds(1f/4f);
        StartAttack();
    }

    // Update is called once per frame
    void Update()
    {
            var direction = player.transform.position - transform.position;
            direction.y = 0;
 
            var lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.1f);
    }
    public void StartAttack()
    {
        StartCoroutine(NTabAttack());
    }

    IEnumerator NTabAttack()
    {
        int i;
        while(true)
        {
            i = Random.Range(0,3);
            if(i == Cash[0]|| i == Cash[1]) continue;
            if(Cash[0] == -1)       Cash[0] = i;
            else if(Cash[1] == -1)  Cash[1] = i;
            else if(Cash[2] == -1)  Cash[2] = 1; 
            
            yield return AIntWait;
            Debug.Log("攻撃開始");
            switch(i)
            {
                case 0:
                {
                    StartCoroutine(BulletSpred());
                    break;
                }
                case 1:
                {
                    StartCoroutine(Bullet3Way());
                    break;
                }
                case 2:
                {
                    StartCoroutine(Bullet140());
                    break;
                }
                default:
                    break;
            }
            if(Cash[2] == 1)
            {
                yield return new WaitForSeconds(2f);
                InitNTable();
            }
        }
    }

    IEnumerator BulletSpred()
    {
        while(true)
        {
            if (BulletCount[0] < MaxBulletCount[0])
            {
                yield return SpWait;
                BulletCount[0]++;
                Quaternion direction = Quaternion.identity;
                Vector3 anglefoword = Vector3.zero;
                var vectUp = new Vector3(0, 0f, 0);
                if (player != null) 
                {
                    //プレイヤーに飛んでいく弾の向きの処理
                    anglefoword = (player.transform.position - this.transform.position).normalized;
                    direction = Quaternion.LookRotation(anglefoword + vectUp, Vector3.up);
                }
                //プレイヤーの球を基準に角度とランダムの処理をつける
                var direction2 = Quaternion.AngleAxis(UnityEngine.Random.Range(-9.0f,9.0f), Vector3.up) * direction;
                Instantiate(Bullet, transform.position, direction2);
                yield return null;
            }
            else
            {
                BulletCount[0] = 0;
                yield break;
            }
        }

    }

    IEnumerator Bullet3Way()
    {
        while(true)
        {
            if (BulletCount[1] < MaxBulletCount[1])
            {
                yield return WayWait;
                BulletCount[1]++;
                Quaternion direction = Quaternion.identity;
                Vector3 anglefoword = Vector3.zero;

                var vectUp = new Vector3(0, 0f, 0);
                if (player != null)
                {
                //プレイヤーに飛んでいく弾の向きの処理
                    anglefoword = (player.transform.position - this.transform.position).normalized;
                    direction = Quaternion.LookRotation(anglefoword + vectUp, Vector3.up);
                }
                //プレイヤーの球を基準に角度をつける処理
                var direction2 = Quaternion.AngleAxis(30, Vector3.up) * direction;
                var direction3 = Quaternion.AngleAxis(-30, Vector3.up) * direction;
                Instantiate(Bullet, transform.position, direction);
                Instantiate(Bullet, transform.position, direction2);
                Instantiate(Bullet, transform.position, direction3);
                yield return null;
            }
            else
            {
                BulletCount[1] = 0;
                yield break;
            }
        }
    }
    IEnumerator Bullet140()
    {
        Quaternion direction = Quaternion.identity;
        Vector3 anglefoword = Vector3.zero;
        var vectUp = new Vector3(0, 0f, 0);
        if (player != null)
        {
            //プレイヤーに飛んでいく弾の向きの処理
            anglefoword = (player.transform.position - this.transform.position).normalized;
            direction = Quaternion.LookRotation(anglefoword + vectUp, Vector3.up);
        }
        Instantiate(Bullet, transform.position, direction);
        int j = 60;
        for(int i = 0; i < MaxBulletCount[2]; i++)
        {
            var direction2 = Quaternion.AngleAxis(j, Vector3.up) * direction;
            Instantiate(Bullet, transform.position, direction2);
            j -= 10;
        }
        yield return null;
    }

    public void HeadDamageHit(int i)
    {
        damageMulti = 2;
        EnemyDamageHit(i);
        BGMPlayer.Instance.PlaySE("S_HeadShot");
    }

    public void BodyDamageHit(int i)
    {
        damageMulti = 1;
        EnemyDamageHit(i);
        BGMPlayer.Instance.PlaySE("S_BodyShot");
    }
    void EnemyDamageHit(int damage)
    {
        EnemyHP -= damage * damageMulti;
        if(EnemyHP <= 0)
        {
            BGMPlayer.Instance.PlaySE("S_EnemyKill");
            GameObject.Find("ClearManager").GetComponent<ClearManager>().EnemyDeath();
            Destroy(this.gameObject);
        }
        Debug.Log(EnemyHP);
    }
}
