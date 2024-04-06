using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField]
    private MeleeCol meleeCol;
    [SerializeField]
    private PlayerMove playerMove;
    [SerializeField]
    private GameObject col;
    [SerializeField]
    private GameObject overLapObj;
    private bool attackFlag;
    public bool OverLapCheck;
    [SerializeField]
    private int frontAttackMoveRange;

    [SerializeField]
    private float frontAttackTimer;
    [SerializeField]
    private float normalAttackTimer;
    private WaitForSeconds fAttack;
    private WaitForSeconds nAttack;
    private WaitForSeconds fAttackReload;
    private WaitForSeconds nAttackReload;
    

    void Start()
    {
//        fAttack = new WaitForSeconds(frontAttackTimer);
//        nAttack = new WaitForSeconds(normalAttackTimer);
//        fAttackReload = new WaitForSeconds(Weapon.instance.ReloadStartTime[6]);
//        nAttackReload = new WaitForSeconds(Weapon.instance.ReloadStartTime[7]);

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            Melee();
        }
    }

    public void SetLayer(int layerNumber)
    {
        this.gameObject.layer = layerNumber;
    }

    void Melee()
    {
        StartCoroutine(ColEnable(PlayerStats.instance.nowWeapons));
    }

    IEnumerator ColEnable(int i)
    {
        if(attackFlag) yield break;
        attackFlag = true;
        
        /*
        if(Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) i = 0;
        else i = 1;
        if(i == 0 && Weapon.instance.NowBullet[6] != 0)
        {
            var vec = playerMove.moveDirection;
            var rb = playerMove.rb;
            Weapon.instance.NowBullet[6]--;
            PlayerStats.instance.meleeFlag = true;
            rb.drag = 0;
            col.SetActive(true);
            SetLayer(3);
            while(true)
            {
                timer += Time.deltaTime;
                rb.velocity += vec;
                Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                if (flatVel.magnitude > frontAttackMoveRange)
                {
                    Vector3 limitedVel = flatVel.normalized * frontAttackMoveRange;
                    rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                }
                if(timer > frontAttackTimer)
                    break;
            }
            yield return fAttack;
            OverLapCheck = true;
            overLapObj.SetActive(true);
        }
        */
        if(Input.GetKey(KeyCode.S))
        {
            BGMPlayer.Instance.PlaySE("S_RPG");
            Weapon.instance.RPG(3);
        }
        else
        {
            Weapon.instance.cashWeapon.Complete();
            Weapon.instance.nowWeapon.Complete();
            col.SetActive(true);
            Weapon.instance.NowBullet[7]--;
            PlayerStats.instance.meleeFlag = true;
            if(PlayerStats.instance.nowWeapons == 1)
            {
                Weapon.instance.WeaponList[1].transform.DOLocalMove(Weapon.instance.WeaponPos[3].localPosition, 0.1f);
            }
            Weapon.instance.WeaponList[6].transform.DOLocalMove(Weapon.instance.WeaponPos[8].localPosition, 0.2f);
            Weapon.instance.WeaponList[6].transform.DOLocalMove(Weapon.instance.WeaponPos[9].localPosition, 0.2f).SetDelay(0.3f).OnComplete(() =>
            {
                PlayerStats.instance.meleeFlag = false;
            });
            Weapon.instance.nowWeapon = Weapon.instance.WeaponList[PlayerStats.instance.nowWeapons].transform.DOLocalMove(Weapon.instance.WeaponPos[Weapon.instance.AnimNumberSet(PlayerStats.instance.nowWeapons)].localPosition, 0.2f).SetDelay(0.5f);
            yield return nAttack;
        }
        StartCoroutine(Reload(i));
        PlayerStats.instance.meleeFlag = false;
        meleeCol.ListInit();
        col.SetActive(false);
        attackFlag = false;
    }

    IEnumerator Reload(int i)
    {
        if(i == 0)
        {
            yield return fAttackReload;
            Weapon.instance.NowBullet[6]++;
        }
        else
        {
            yield return nAttackReload;
            Weapon.instance.NowBullet[7]++;
        }
    }
}
