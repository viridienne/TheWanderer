using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class IngameData
{
    public bool _isNewGame;
    public bool _isLevelCompleted;
    public Vector3 position;
    public int keyobtained;

    public List<float> itemID = new List<float>();
    public IngameData()
    {

    }
    public IngameData(Vector3 pos)
    {
        this.position = pos;
    }
}
