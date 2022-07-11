using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InkManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset _jsonAsset;

    private Story _story;

    [SerializeField]
    private Text _textField;
    [SerializeField]
    private VerticalLayoutGroup _buttonsContainer;
    [SerializeField]
    private Button _buttonPrefab;

    [SerializeField]
    private Color _normalTextColor;

    [SerializeField]
    private Color _thoughtTextColor;

    // Start is called before the first frame update
    void Start()
    {
        StartStory();
    }
   
    void StartStory()
    {
        _story = new Story(_jsonAsset.text);
        DisplayNextLine();
    }
    public void DisplayNextLine()
    {
        if (_story.canContinue)
        {
            string text = _story.Continue();
            text = text?.Trim();
            ApplyStyling();
            _textField.text = text;        

        }
        else if (_story.currentChoices.Count > 0)
        {
            DisplayChoiceButton();
        }
    }
    public void DisplayChoiceButton()
    {
        //check if the choices are displayed
        if (_buttonsContainer.GetComponentsInChildren<Button>().Length > 0) return;
        for(int i=0;i<_story.currentChoices.Count;i++)
        {
            var choice = _story.currentChoices[i];
            var button = CreateChoiceButton(choice.text);

            button.onClick.AddListener(()=>OnClickChoiceButton(choice));
        }
    }
    Button CreateChoiceButton(string text) // PUT TEXT IN THE BUTTON
    {
        //CREATE BUTTON FROM PREFAB
        var choiceButton = Instantiate(_buttonPrefab);
        choiceButton.transform.SetParent(_buttonsContainer.transform,false);

        //SET TEXT OF THE BUTTON
        var choiceButtonTXT = choiceButton.GetComponentInChildren<Text>();
        choiceButtonTXT.text = text;

        return choiceButton;
    }
    void OnClickChoiceButton(Choice choice)
    {
        _story.ChooseChoiceIndex(choice.index); //tell ink which choice was selected
        RemoveChoiceAfterClicked();
        DisplayNextLine();

    }
    void RemoveChoiceAfterClicked()
    {
        if (_buttonsContainer != null)
        {
            foreach(var button in _buttonsContainer.GetComponentsInChildren<Button>())
            {
                Destroy(button.gameObject);
            }
        }
    }
    private void ApplyStyling()
    {
        if(_story.currentTags.Contains("thought"))
        {
            _textField.color = _thoughtTextColor;
            _textField.fontStyle = FontStyle.Italic;
        }
        else
        {
            _textField.color = _normalTextColor;
            _textField.fontStyle = FontStyle.Normal;
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
}
