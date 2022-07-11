using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossCanvas : MonoBehaviour
{
    [SerializeField]
    private Slider sliderHP;
    [SerializeField]
    private NecromancerScript _necrosript;

    // Start is called before the first frame update
    void Start()
    {
        _necrosript = GetComponentInParent<NecromancerScript>();
        sliderHP.value = _necrosript.HP;

    }
    private void Update()
    {
        OnSliderValueChanged();
    }
    public void OnSliderValueChanged()
    {
        sliderHP.value = _necrosript.HP;
    }
}
