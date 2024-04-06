using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class ScopeEffect : MonoBehaviour
{
    [SerializeField]
    private Volume volume;
    public Vignette vignette;
    // Start is called before the first frame update
    void Start()
    {
        vignette = null;
        volume.profile.TryGet<Vignette>(out vignette);
    }

}
