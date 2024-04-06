using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEShild : MonoBehaviour
{
    [SerializeField]
    private EnemyE enemyE;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && timer > 1f)
            PlayerStats.instance.PlayerDamage(enemyE.enemyATK);
        else if(other.gameObject.CompareTag("PlayerBullet"))
            Destroy(other.gameObject);
        
    }
}
