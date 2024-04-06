using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletMove : MonoBehaviour
{
    private EnemyAMock enemyAMock;
    [SerializeField]
    private GameObject bulletHole;
    [SerializeField]
    private GameObject hitEffect;
    [SerializeField]
    public GameObject cashObj;
    [SerializeField]
    private Material mat;
    [SerializeField]
    private Rigidbody rb;
    public bool RPG;
    public Vector3 endPos;
    public int bulletPower;
    [SerializeField]
    private float speed;
    public float offSet;

    [SerializeField]
    float m_force;
    [SerializeField]
    float m_radius;
    [SerializeField]
    float m_upwards;
    Vector3 m_position;
    
    // Start is called before the first frame update
    void Start()
    {
        this.transform.LookAt(endPos);
        Destroy(this.gameObject,10f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //rb.AddForce(transform.forward * speed * offSet * Time.deltaTime);
        rb.velocity = transform.forward * speed * offSet * Time.deltaTime;
    }


    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Body") || other.gameObject.CompareTag("Head"))
        {
            Instantiate(hitEffect,this.transform.position,this.transform.rotation);
            if(RPG) 
            {
                m_position = this.gameObject.transform.position;

                // 範囲内のRigidbodyにAddExplosionForce
                Collider[] hitColliders = Physics.OverlapSphere(m_position, m_radius);
                for(int i = 0; i < hitColliders.Length; i++)
                {
                    BGMPlayer.Instance.PlaySE("S_Explosion");
                    var rb = hitColliders[i].GetComponent<Rigidbody>();
                    float distance = Vector3.Distance(hitColliders[i].transform.position,this.gameObject.transform.position);
                    if(hitColliders[i].gameObject.CompareTag("Player"))
                    {
                        rb.AddExplosionForce(m_force, m_position, m_radius, m_upwards, ForceMode.Impulse);
                    }

                }
            }
            if(other.gameObject.CompareTag("Head"))
            {
                Instantiate(hitEffect,this.transform.position,this.transform.rotation);
                other.gameObject.GetComponent<EnemyColCheck>().Head(bulletPower);
                
            }
            else if(other.gameObject.CompareTag("Body"))
            {
                Instantiate(hitEffect,this.transform.position,this.transform.rotation);
                other.gameObject.GetComponent<EnemyColCheck>().Body(bulletPower);
                
            }
            Debug.Log("Hit");
            Destroy(this.gameObject);
        }
        //Destroy(this.gameObject);
    }

    public int GetDamage()
    {
        return bulletPower;
    }

}
