using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Slider slider;
    [SerializeField]
    private Text text;
    [SerializeField]
    private InputField inputField;

    public void SliderSet()
    {
        var value = Mathf.Clamp(float.Parse(inputField.text), slider.minValue, slider.maxValue);
        slider.value = value;
    }
    public void InputFieldSet()
    {
        inputField.text = slider.value.ToString("N1");
    }
}
