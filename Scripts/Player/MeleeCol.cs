using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCol : MonoBehaviour
{
    public LinkedList<GameObject> hitObj = new LinkedList<GameObject>();

    public void ListInit()
    {
        hitObj.Clear();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            foreach(GameObject obj in hitObj)
            {
                if(obj == other.gameObject)
                {
                    Debug.Log("同じ敵にあたってるよ");
                    return;
                }
            }
            hitObj.AddFirst(other.gameObject);
            Debug.Log("違う敵にあたってるかも");
        }
    }
}
