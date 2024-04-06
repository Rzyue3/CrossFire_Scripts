using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverLapping : MonoBehaviour
{
    [SerializeField]
    private MeleeAttack meleeAttack;
    [SerializeField]
    private float timer;
    public void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy") && meleeAttack.OverLapCheck)
        {
            timer = 0f;
            meleeAttack.SetLayer(3);

        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 0.1f && meleeAttack.OverLapCheck)
        {
            meleeAttack.SetLayer(0);
            meleeAttack.OverLapCheck = false;
            return;
        }
        if(timer > 1f)
        {
            timer = 0f;
            this.gameObject.SetActive(false);
        }
    }
}
