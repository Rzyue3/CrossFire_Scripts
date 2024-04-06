using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyADemo : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private int rand;
    NavMeshAgent navMesh; /// NavMeshAgent
    private float f;
    private float timer;
    private bool b;
    // Start is called before the first frame update
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        b = true;
    }

    // Update is called once per frame
    void Update()
    {
                Vector3 target = player.transform.position;
                target.y = this.transform.position.y;
                this.transform.LookAt (target);
        timer += Time.deltaTime;
        //&& navMesh.velocity.sqrMagnitude <= 0.5f
        if(Vector3.Distance(player.transform.position, this.transform.position) <= 5  && timer > 2f)
        {
            rand = Random.Range(0,4);
            var i = 0f;
            if(rand == 0)
            {
                i = 2f;
            }
            else if(rand == 1)
            {
                i = 3f;
            }
            else if(rand == 2)
            {
                i = -2f;
            }
            else if(rand == 3)
            {
                i = -3f;
            }


            //navMesh.SetDestination(player.transform.position);
            var moveDirection =  this.transform.right * i;
            navMesh.SetDestination(new Vector3(moveDirection.x,this.transform.position.y,moveDirection.z));
            Debug.Log("rrr");
            timer = 0f;
            return;
        }
        else if(Vector3.Distance(player.transform.position, this.transform.position) > 6f)
        {
            navMesh.SetDestination(player.transform.position);

        }
        /*
        if (!navMesh.hasPath || navMesh.velocity.sqrMagnitude == 0f && b) 
        {
            Debug.Log("れれれ");
			navMesh.SetDestination(new Vector3(this.transform.localPosition.x + (Random.Range(-5f,5f) * 5),this.transform.position.y,this.transform.position.z));
            b = false;
            f = 0f;
		}
        else
        {
            f += Time.deltaTime;
            if(f > 1f && !b)
            {
                b = true;
                f = 0;
            }
        }
        */
    }
}
