using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTest : MonoBehaviour
{
    [SerializeField]
    private Animator p_Animator;
    [SerializeField]
    private Transform LegIKTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnAnimatorIK()
    {
        // 右足のIKを有効化する(重み:1.0)
        p_Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1.0f);
        p_Animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1.0f);

        // 右足のIKのターゲットを設定する
        p_Animator.SetIKPosition(AvatarIKGoal.RightFoot, LegIKTarget.position);
        p_Animator.SetIKRotation(AvatarIKGoal.RightFoot, LegIKTarget.rotation);
    }
}
