using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    [SerializeField]
    public int _activescene;
    [SerializeField]
    public List<float> _listItemIDinGM = new List<float>();
    [SerializeField]
    public List<ItemController> _listItemGM = new List<ItemController>();
    [SerializeField]
    public IngameData ingameData = new IngameData();


    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if(_instance==null)
                {
                    GameObject gameobj = new GameObject();
                    gameobj.name = "GameManager";
                    _instance = gameobj.AddComponent<GameManager>();
                }
            }
            return _instance;
        }       
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (_instance == null)
        {
            _instance = this;
        }
        else Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {       
    }

    // Update is called once per frame
    void Update()
    {
        _activescene = SceneManager.GetActiveScene().buildIndex;

    }
    public void RegisterItemInScene(ItemController item)
    {
        _listItemGM.Add(item);
        if (_listItemIDinGM.Contains(item.itemID) == false) _listItemIDinGM.Add(item.itemID);
    }
    public void MoveToNextLevel()
    {
        ingameData.keyobtained = 0;
        ingameData._isLevelCompleted = true;
        ingameData.position = Vector3.zero; 
        string json = JsonUtility.ToJson(GameManager.Instance.ingameData);
        PlayerPrefs.SetString("INGAME_DATA", json);
        StartCoroutine(LoadLevel());
    }
    IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadSceneAsync(_activescene + 1);
    }
}
