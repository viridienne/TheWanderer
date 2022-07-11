using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StaminaBarScript : MonoBehaviour
{
    [SerializeField]
    private PlayerConfig _configData;
    [SerializeField]
    private int configStamina;
    [SerializeField]
    private int currentStamina;
    [SerializeField]
    private Image staminaIMG;
    [SerializeField]
    private Image staminaIMG2;
    [SerializeField]
    private PlayerController _playerscript;
    [SerializeField]
    private RectTransform _staminaBar;
    float stapercent;
    // Start is called before the first frame update
    void Start()
    {
        _playerscript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        configStamina = _configData.PlayerConfigStamina;
    }

    // Update is called once per frame
    void Update()
    {
        //currentStamina = _playerscript.PlayerStamina;
        //stapercent = (float)currentStamina / configStamina;
        //staminaIMG.fillAmount = Mathf.Lerp(staminaIMG.fillAmount, stapercent, Time.deltaTime);
    }
    public void UpdateGraphic()
    {
        //using cross fade alpha for effect
        staminaIMG2.fillAmount = staminaIMG.fillAmount;
        staminaIMG2.CrossFadeAlpha(1f, 0f, true);
        staminaIMG2.CrossFadeAlpha(0f, 2f, true); 
        
        currentStamina = _playerscript.PlayerStamina;
        stapercent = (float)currentStamina / configStamina;
        DOVirtual.Float(staminaIMG.fillAmount, stapercent, 1.5f, (amount) =>
        {
            staminaIMG.fillAmount = amount;
        });
    }
}
