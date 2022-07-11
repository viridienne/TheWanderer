using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextButtonScript : MonoBehaviour
{
    [SerializeField]
    private InkManager _inkManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClick()
    {
        _inkManager.DisplayNextLine();
    }

}

