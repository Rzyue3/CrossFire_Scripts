using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJump : MonoBehaviour
{
    [SerializeField] 
    Material[] materialArray = new Material[3];
    [SerializeField]
    private float waitTime;
    [SerializeField]
    private BoxCollider col;
    public bool flag;
    [SerializeField]
    private GameObject lightObj;   
    
    private Animator animator; 

    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        flag = true;
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && flag)
        {
            PlayerStats.instance.nearWallList.AddFirst(this.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerStats.instance.nearWallList.Remove(this.gameObject);
            PlayerStats.instance.doubleJump = false;
        }
    }

    public void Jump()
    {
        col.enabled = false;
        PlayerStats.instance.doubleJump = false;
        flag = false;
        PlayerStats.instance.nearWallList.Remove(this.gameObject);

        StartCoroutine(Wait());
        lightObj.SetActive(false);
        animator.SetTrigger("isColor");
    }

    IEnumerator Wait()
    {
        PlayerStats.instance.doubleJump = false;
        yield return new WaitForSeconds(waitTime);
        flag = true;
        col.enabled = true;
        lightObj.SetActive(true);
    }

    // 30f で消滅
    // 270f かけて復活

    
    // 切り替えモーションは武器を画面外に移動させて次の武器を画面外から移動させてくるイメージ
    // 画面外通常は右側 左SMGのみ左から描画
    // 切り替え始まったら次の銃を撃ち初めて良い
    // 左SMGは左に描画、近接振る時左のSMGを投げて使用
    // RPGは左上
}
