using System;
using UnityEngine;
using UnityEngine.Events;

public class EnemyColCheck : MonoBehaviour
{
    [Serializable] private class HitEvent : UnityEvent<int> { }
    [SerializeField] private HitEvent hitEvent = null;
    void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("PlayerBullet")) return;
        var bulletMove = other.gameObject.GetComponent<BulletMove>();
        var damage = bulletMove.GetDamage();
        var obj = other.transform.root.gameObject;
        if(obj == bulletMove.cashObj) return;
        bulletMove.cashObj = obj;
        hitEvent.Invoke(damage);
        Destroy(other.gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        if(!other.gameObject.CompareTag("PlayerBullet")) return;
        Debug.Log("BodyHit");
        var bulletMove = other.gameObject.GetComponent<BulletMove>();
        var damage = bulletMove.GetDamage();
        var obj = other.transform.root.gameObject;
        if(obj == bulletMove.cashObj) return;
        bulletMove.cashObj = obj;
        hitEvent.Invoke(damage);
        Destroy(other.gameObject);
    }

    public void Head(int i)
    {
        hitEvent.Invoke(i);
    }

    public void Body(int i)
    {
        hitEvent.Invoke(i);
    }


}
