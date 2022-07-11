using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossDemonCanvasScript : MonoBehaviour
{
    [SerializeField]
    private Slider sliderHP;
    [SerializeField]
    private DemonScript _demonscript;

    // Start is called before the first frame update
    void Start()
    {
        _demonscript = GetComponentInParent<DemonScript>();
        sliderHP.value = _demonscript.HP;

    }
    private void Update()
    {
        OnSliderValueChanged();
    }
    public void OnSliderValueChanged()
    {
        sliderHP.value = _demonscript.HP;
    }
}
