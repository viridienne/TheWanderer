using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HPBAR : MonoBehaviour
{
    [SerializeField]
    private PlayerConfig _configData;
    [SerializeField]
    private int configHP;
    [SerializeField]
    private int currentHP;
    [SerializeField]
    private Image imgHP;
    [SerializeField]
    private Image imgHP2;
    [SerializeField]
    private PlayerController _playerscript;
    [SerializeField]
    private RectTransform _hpBar;
    float hppercent;
    // Start is called before the first frame update
    void Start()
    {
        _playerscript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        configHP = _configData.PlayerConfigHP;


        //_hpBar.DOPunchScale(Vector3.one * 1.3f, 1.5f);

        //ALTERNATIVE SOLUTION:
        //_hpBar.DOScale(Vector3.one * 1.3f, 1f).OnComplete(() =>
        //   {
        //       _hpBar.DOScale(Vector3.one, 1f);
        //   });
    }

    // Update is called once per frame
    void Update()
    {
        //100: 1
        //90 : ?
        //hpbar.fillAmount = (float)currentHP / configHP;
       // currentHP = _playerscript.PlayerHP;

        //hppercent = (float)currentHP / configHP;

        ////lerp value
        //imgHP.fillAmount = Mathf.Lerp(imgHP.fillAmount, hppercent, Time.deltaTime);

        //lerp percentage
        //hpbar.fillAmount = Mathf.Lerp(hpbar.fillAmount, hppercent, hppercent/hpbar.fillAmount);
    }
    public void UpdateGraphic()
    {
        //using cross fade alpha for effect
        imgHP2.fillAmount = imgHP.fillAmount;
        imgHP2.CrossFadeAlpha(1f, 0f, true);
        imgHP2.CrossFadeAlpha(0f, 2f, true);

        currentHP = _playerscript.PlayerHP;
        hppercent = (float)currentHP / configHP;
        DOVirtual.Float(imgHP.fillAmount, hppercent, 1.5f, (amount) =>
           {
               imgHP.fillAmount = amount;
           });
    }
}
