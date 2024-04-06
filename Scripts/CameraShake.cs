using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public CinemachineImpulseSource Source;//何かしらの方法で設定
    public static CameraShake instance;
    
    private void Start()
    {
        instance = this;
    }

    public void ShakeStart()
    {
        Debug.Log("Shake");
        this.transform.localPosition = new Vector3(0f,1.2f,0f);
        Shake(new Vector3(2,2,2),0.2f,0.2f);
        
    }

    /// <summary>
    /// カメラを揺らす
    /// </summary>
    /// <param name="dire"></param>
    /// <param name="decelerationTime"></param>
    /// <param name="maxTime"></param>
    public void Shake(Vector3 dire, float decelerationTime,float maxTime)
    {
        Source.m_ImpulseDefinition.m_TimeEnvelope.m_AttackTime = maxTime;
        Source.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime = decelerationTime;
        Source.GenerateImpulse(dire);
    }
}
