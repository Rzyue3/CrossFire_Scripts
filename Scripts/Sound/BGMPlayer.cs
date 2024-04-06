using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 使い方
// AudioSourceを二つ用意します。
// Inspectorでアタッチします。[完] NHK...
// クラス名   静的なやつ 関数(音の名前(クリップの名前ではなくInspectorで入れた名前),ボリューム,ループするかどうか)     
// BGMPlayer.Instance.PlayBGM("BGMName", 1f,true);
// BGMPlayer.Instance.PlaySE("SEName", 1f);
// ボリュームとループはデフォルト値が入っているので↓のように書かなくてもOKなはず。
// BGMPlayer.Instance.PlayBGM("BGMName");

public class BGMPlayer : MonoBehaviour
{
    private static BGMPlayer instance = null;
    public static BGMPlayer Instance
    {
        get{
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<BGMPlayer>();

                if (instance == null)
                {
                    GameObject obj = new GameObject("BGMPlayer");
                    instance = obj.AddComponent<BGMPlayer>();
                }
            }
            return instance;
        }
    }

    [SerializeField]
    private AudioSource asBGM;
    [SerializeField]
    private AudioSource asSE;


    /// <summary>
    /// BGMを鳴らせるよ!
    /// </summary>
    /// <param name="name">使いたいBGMの名前</param>
    /// <param name="volume">ボリューム設定</param>
    /// <param name="loop">ループするかどうか</param>
    public void PlayBGM(string name, float volume = 0.5f, bool loop = false)
    {
        if (BGMRack.AudioClips
            .TryGetValue(name, out AudioClip ac))
        {
            asBGM.volume = volume;
            asBGM.loop = loop;
            asBGM.clip = ac;
            asBGM.Play();
        }
        else
        {
            Debug.LogWarning
            ("Name:"+name +"の音楽が見つからなかったため、PlayBGMの命令が通りませんでした");
        }
    }

    /// <summary>
    /// SEを鳴らせるよ!
    /// </summary>
    /// <param name="name">使いたい効果音の名前</param>
    /// <param name="volume">ボリューム設定</param>
    public void PlaySE(string name, float volume = 0.5f)
    {
        if (BGMRack.AudioClips
            .TryGetValue(name, out AudioClip ac))
        {
            //asSE.volume = volume;
            //asSE.loop = loop;
            //asSE.clip = ac;
            asSE.PlayOneShot(ac,volume);
        }
        else
        {
            Debug.LogWarning
            ("Name:"+name +"の効果音が見つからなかったため、PlayBGMの命令が通りませんでした");
        }
    }

}
