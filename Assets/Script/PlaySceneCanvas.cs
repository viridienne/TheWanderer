using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class PlaySceneCanvas : MonoBehaviour
{
    [SerializeField]
    private RectTransform _panelGameOver;
    [SerializeField]
    private string _sceneName;
    [SerializeField]
    private RectTransform _statPnl;
    [SerializeField]
    private PlayerController _playerScript;
    [SerializeField]
    private RectTransform _panelSetting;
    [SerializeField]
    private RectTransform _panelMessage;
    [SerializeField]
    private RectTransform _pnlOnGameStart;
    [SerializeField]
    private Transform _pnlNeedToSlay;
    [SerializeField]
    private RectTransform _pnlEndgame;
    [SerializeField]
    private TextMeshProUGUI _txtMessageBoard;
    [SerializeField]
    private Image _pnlStartGameIMG;    
    [SerializeField]
    private Image _pnlEndGameIMG;
    [SerializeField]
    private RectTransform _dialoguePanel;

    private void Start()
    {
        _statPnl.DOScale(Vector3.one * 1.2f, 1.5f).OnComplete(() =>
           {
               _statPnl.DOScale(Vector3.one, 1.5f);
           });
        if (GameManager.Instance.ingameData._isNewGame)
        {
            StartMessagePopUp();
            Time.timeScale = 1f;
        }
    }
    private void Update()
    {
        //OnHPValueChanged();
        //OnStaminaValueChanged();
    }
    public void EnableDialoguePnl()
    {
        _dialoguePanel.gameObject.SetActive(true);
    }
    public void DisableDialoguePnl()
    {
        _dialoguePanel.gameObject.SetActive(false);
    }
    public void CloseDialogueBtnClicked()
    {
        DisableDialoguePnl();
    }
    public void EndgamePnlEnable()
    {
        _pnlEndgame.gameObject.SetActive(true);
        FadeIn(_pnlEndGameIMG);
    }
    public void EndgameButtonClicked()
    {
        FadeOut(_pnlEndGameIMG);
        StartCoroutine(Endgame());
    }
    public void StartMessagePopUp()
    {
        _pnlOnGameStart.gameObject.SetActive(true);
        FadeIn(_pnlStartGameIMG);
    }
    public void ContinueBtnClicked()
    {
        FadeOut(_pnlStartGameIMG);
        StartCoroutine(TurnOffPanel(_pnlOnGameStart));
    }
    void FadeIn(Image IMG)
    {
        IMG.CrossFadeAlpha(0f, 0f, true);
        IMG.GetComponentInChildren<TextMeshProUGUI>().CrossFadeAlpha(0f, 0f, true);
        IMG.GetComponentInChildren<Button>().image.CrossFadeAlpha(0f, 0f, true);
        IMG.GetComponentInChildren<Button>().GetComponentInChildren<TextMeshProUGUI>().CrossFadeAlpha(0f, 0f, true);
        IMG.CrossFadeAlpha(1f, 2f, true);
        IMG.GetComponentInChildren<TextMeshProUGUI>().CrossFadeAlpha(1f, 2f, true);
        IMG.GetComponentInChildren<Button>().image.CrossFadeAlpha(1f, 2f, true);
        IMG.GetComponentInChildren<Button>().GetComponentInChildren<TextMeshProUGUI>().CrossFadeAlpha(1f, 2f, true);
    }
    void FadeOut(Image IMG)
    {
        IMG.CrossFadeAlpha(0f, 2f, true);

        IMG.GetComponentInChildren<TextMeshProUGUI>().CrossFadeAlpha(0f, 2f, true);
        IMG.GetComponentInChildren<Button>().image.CrossFadeAlpha(0f, 2f, true);
        IMG.GetComponentInChildren<Button>().GetComponentInChildren<TextMeshProUGUI>().CrossFadeAlpha(0f, 2f, true);
    }
    IEnumerator TurnOffPanel(Transform pnl)
    {
        yield return new WaitForSeconds(2.5f);
        pnl.gameObject.SetActive(false);
    }   
    IEnumerator Endgame()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("Menu");
    }
    public void NeedToSlayMessage()
    {
        _pnlNeedToSlay.gameObject.SetActive(true);
    }
    public void NeedToSlayMessageContinueBtnClicked()
    {
        _pnlNeedToSlay.gameObject.SetActive(false);
    }
    //public void OnHPValueChanged()
    //{
    //    _sliderHP.value = Mathf.Lerp(_sliderHP.value, _playerScript.PlayerHP, Time.deltaTime);
    //}
    //public void OnStaminaValueChanged()
    //{
    //    _sliderStamina.value = _playerScript.PlayerStamina;

    //}

    public void GameOver()
    {
        _panelGameOver.gameObject.SetActive(true);
    }
    public void EnableSettingPanel()
    {
        _panelSetting.gameObject.SetActive(true);
    }
    public void MessageBoard()
    {
        var scene = SceneManager.GetActiveScene();
        Time.timeScale = 0f;

        _panelMessage.gameObject.SetActive(true);
        if (scene.name == "PlayScene1")
        {
            _txtMessageBoard.text = "You need to collect enough keys to open the church's door";
        }
        else
        {
            _txtMessageBoard.text = "You need to slay THE DEMON and collect enough keys";
        }
    }
    public void MessageOnCloseButtonClicked()
    {
        _panelMessage.gameObject.SetActive(false);

        Time.timeScale = 1f;

    }
    public void OnReloadButtonClicked()
    {
        SceneManager.LoadScene(_sceneName);
    }
    public void OnMenuButtonClicked()
    {
        SceneManager.LoadScene("Menu");
    }
    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    public void SettingOnMenuButtonClicked()
    {
        SceneManager.LoadScene("Menu");
    }
    public void SettingOnCloseButtonClicked()
    {
        _panelSetting.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
    public void SettingOnExitButtonClicked()
    {
        Application.Quit();
    }
    [ContextMenu("test")]
    void test()
    {
        EndgamePnlEnable();
    }
}
