using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCanvasController : MonoBehaviour
{
    [SerializeField]
    private Transform _pnlSetting;
    [SerializeField]
    private VolumeController volumeController;
    private void Start()
    {
        volumeController.SetUp();
    }
    public void OnContinueBtnClicked()
    {
        if(PlayerPrefs.HasKey("INGAME_DATA"))
        {
        GameManager.Instance.ingameData._isNewGame = false;
        SceneManager.LoadScene(PlayerPrefs.GetString("ActiveScene"));
        }
    }

    public void OnSettingButtonClicked()
    {
        _pnlSetting.gameObject.SetActive(true);
    }
    public void OnStartButtonClicked()
    {
        PlayerPrefs.DeleteKey("INGAME_DATA");
        PlayerPrefs.SetString("ActiveScene", "PlayScene1");
        GameManager.Instance.ingameData.itemID = null;
        GameManager.Instance.ingameData._isLevelCompleted = true;
        GameManager.Instance.ingameData._isNewGame = true;
        SceneManager.LoadScene("PlayScene1");
    }
    public void OnExitButtonClicked()
    {
        Application.Quit();
    }
    public void OnCloseBtnClicked()
    {
        _pnlSetting.gameObject.SetActive(false);
    }
}
