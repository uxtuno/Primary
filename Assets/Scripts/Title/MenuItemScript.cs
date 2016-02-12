using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuItemScript : MonoBehaviour
{
    public int Index
    {
        get { return this.index; }
    }

    public string Name
    {
        get { return this.itemName; }
    }

    public float Value { set; get; }

    public int UpItemIndex
    {
        get { return upItemIndex; }
    }

    public int DownItemIndex
    {
        get { return downItemIndex; }
    }

    public int LeftItemIndex
    {
        get { return leftItemIndex; }
    }

    public int RightItemIndex
    {
        get { return rightItemIndex; }
    }

    [SerializeField]
    private int index = 0; // 項目ID
    [SerializeField]
    private string itemName = ""; // 項目名

    [SerializeField]
    private int upItemIndex = -1; // 上に存在する項目番号
    [SerializeField]
    private int downItemIndex = -1; // 上に存在する項目番号
    [SerializeField]
    private int leftItemIndex = -1; // 上に存在する項目番号
    [SerializeField]
    private int rightItemIndex = -1; // 上に存在する項目番号

    public GameObject ChildMenuPrefab = null; // 子メニューのプレハブ
}

