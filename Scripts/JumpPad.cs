using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField]
    private int jumpForce;

    // 触れた相手を上に飛ばす
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * jumpForce,ForceMode.Impulse);
    }
}
