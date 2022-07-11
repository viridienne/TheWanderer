using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public float itemID;
    private void Start()
    {
        itemID = transform.position.x;
        GameManager.Instance.RegisterItemInScene(this);
        if(GameManager.Instance.ingameData.itemID.Count>0)
        {
            if (GameManager.Instance.ingameData.itemID.Contains(itemID)) gameObject.SetActive(false); // set object inactive when reload if the id is registered
        }
    }
}
