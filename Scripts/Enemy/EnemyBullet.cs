using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerStats.instance.PlayerDamage(EnemyManager.instance.EnemyATK[1]);
        }
    }
}
