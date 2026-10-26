using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public List<Item> items;
    public int selectedItem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
           selectedItem=Mathf.Clamp(selectedItem+(int)Mouse.current.scroll.value.y,0,items.Count);
           if (selectedItem>0)
        {
            
        }
    }
}
