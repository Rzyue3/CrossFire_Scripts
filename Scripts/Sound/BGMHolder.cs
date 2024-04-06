using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BGMPlayerに使用方法記載

public static class BGMRack
{
    public static 
        Dictionary<string, AudioClip> AudioClips
         = new Dictionary<string, AudioClip>();
}

public class BGMHolder : MonoBehaviour
{
    [System.Serializable]
    public class AudioData
    {
        public string name;
        public AudioClip audioClip;
    }

    [SerializeField]
    private List<AudioData> _listAudioData
         = new List<AudioData>();

    private void Awake()
    {
        foreach(var ad in _listAudioData)
        {
            BGMRack.AudioClips.Add(ad.name, ad.audioClip);
        }
    }
}