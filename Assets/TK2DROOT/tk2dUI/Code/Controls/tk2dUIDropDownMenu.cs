using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// DropDown Menu Control
/// </summary>
[AddComponentMenu("2D Toolkit/UI/tk2dUIDropDownMenu")]
public class tk2dUIDropDownMenu : MonoBehaviour
{
    /// <summary>
    /// Button that controls, dropdown list from appearing
    /// </summary>
    public tk2dUIItem dropDownButton;

    /// <summary>
    /// Primary textMesh, this will read what DropDownItem is selected
    /// </summary>
    public tk2dTextMesh selectedTextMesh;

    /// <summary>
    /// Visual height of this ui item, used for spacing
    /// </summary>
    public float height;

    /// <summary>
    /// Template for each drop down item. Will be cloned.
    /// </summary>
    public tk2dUIDropDownItem dropDownItemTemplate;

    /// <summary>
    /// List all all the text for the dropdown list
    /// </summary>
    [SerializeField] 
#pragma warning disable 649
    private string[] startingItemList;
#pragma warning restore 649
	
    /// <summary>
    /// Index of which item in the dropdown list will be selected first
    /// </summary>
    [SerializeField]
    private int startingIndex = 0;

    private List<string> itemList = new List<string>();

    /// <summary>
    /// List of all text item in dropdown menu
    /// </summary>
    public List<string> ItemList
    {
        get { return itemList; }
        set { itemList = value; }
    }

    /// <summary>
    /// Event, if different item is selected
    /// </summary>
    public event System.Action OnSelectedItemChange;

    private int index;

    /// <summary>
    /// Which list index is currently selected
    /// </summary>
    public int Index
    {
        get { return index; }
        set
        {
            index = Mathf.Clamp(value, 0, ItemList.Count - 1);
            SetSelectedItem();
        }
    }

    /// <summary>
    /// Text of the currently selected dropdown list item
    /// </summary>
    public string SelectedItem
    {
        get
        {
            if (index >= 0 && index < itemList.Count)
            {
                return itemList[index];
            }
            else
            {
                return "";
            }
        }
    }

    private List<tk2dUIDropDownItem> dropDownItems = new List<tk2dUIDropDownItem>();

    private bool isExpanded = false; //is currently in expanded input

    void Awake()
    {
        foreach (string itemStr in startingItemList)
        {
            itemList.Add(itemStr);
        }
        index = startingIndex;
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
        //disable all items in template, do make it so Unity 4.x works nicely
        dropDownItemTemplate.gameObject.SetActiveRecursively(false);
#endif
        UpdateList();
    }

    void OnEnable()
    {
        dropDownButton.OnDown += ExpandButtonPressed;
    }

    void OnDisable()
    {
        dropDownButton.OnDown -= ExpandButtonPressed;
    }

    /// <summary>
    /// Updates all items in list. Need to call this after manipulating strings
    /// </summary>
    public void UpdateList()
    {
        Vector3 localPos;
        tk2dUIDropDownItem item;
        if (dropDownItems.Count > ItemList.Count)
        {
            for (int n = ItemList.Count; n < dropDownItems.Count; n++)
            {
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
                dropDownItems[n].gameObject.SetActiveRecursively(false);
#else
                dropDownItems[n].gameObject.SetActive(false);
#endif
            }
        }

        while (dropDownItems.Count < ItemList.Count)
        {
            dropDownItems.Add(CreateAnotherDropDownItem());
        }

        for (int p = 0; p < ItemList.Count; p++)
        {
            item = dropDownItems[p];
            localPos = item.transform.localPosition;
            localPos.y = -height - (p * item.height);
            item.transform.localPosition = localPos;
            if (item.label != null)
            {
                item.LabelText = itemList[p];
            }
            item.Index = p;
        }

        SetSelectedItem();
    }

    /// <summary>
    /// Sets the selected item (based on index)
    /// </summary>
    public void SetSelectedItem()
    {
        if (index < 0 || index >= ItemList.Count)
        {
            index = 0;
        }
        if (index >= 0 && index < ItemList.Count)
        {
            selectedTextMesh.text = ItemList[index];
            selectedTextMesh.Commit();
        }
        else
        {
            selectedTextMesh.text = "";
            selectedTextMesh.Commit();
        }

        if (OnSelectedItemChange != null) { OnSelectedItemChange(); }
    }

    //clones another dropdown item from template
    private tk2dUIDropDownItem CreateAnotherDropDownItem()
    {
        GameObject go = Instantiate(dropDownItemTemplate.gameObject) as GameObject;
        go.name = "DropDownItem";
        go.transform.parent = transform;
        go.transform.localPosition = dropDownItemTemplate.transform.localPosition;
        go.transform.localRotation = dropDownItemTemplate.transform.localRotation;
        go.transform.localScale = dropDownItemTemplate.transform.localScale;
        tk2dUIDropDownItem item = go.GetComponent<tk2dUIDropDownItem>();

        item.OnItemSelected += ItemSelected;

        tk2dUIUpDownHoverButton itemUpDownHoverBtn = go.GetComponent<tk2dUIUpDownHoverButton>();
        item.upDownHoverBtn = itemUpDownHoverBtn;

        itemUpDownHoverBtn.OnToggleOver += DropDownItemHoverBtnToggle;

#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
        go.SetActiveRecursively(false);
#endif

        return item;
    }

    //when an item in list is selected
    private void ItemSelected(tk2dUIDropDownItem item)
    {
        if (isExpanded)
        {
            CollapseList();
        }
        Index = item.Index;
    }

    private void ExpandButtonPressed()
    {
        if (isExpanded)
        {
            CollapseList();
        }
        else
        {
            ExpandList();
        }
    }

    //drops list down
    private void ExpandList()
    {
        isExpanded = true;
        foreach (tk2dUIDropDownItem item in dropDownItems)
        {
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
            item.gameObject.SetActiveRecursively(true);
            item.upDownHoverBtn.SetState(); //deals with how active recursive needs to work in Unity 3.x
#else
            item.gameObject.SetActive(true);
#endif
        }

        tk2dUIDropDownItem selectedItem = dropDownItems[index];

        if (selectedItem.upDownHoverBtn != null)
        {
            selectedItem.upDownHoverBtn.IsOver = true;
        }
    }

    //collapses list on selecting item or closing
    private void CollapseList()
    {
        isExpanded = false;
        foreach (tk2dUIDropDownItem item in dropDownItems)
        {
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
            item.gameObject.SetActiveRecursively(false);
#else
            item.gameObject.SetActive(false);
#endif
        }
    }

    private void DropDownItemHoverBtnToggle(tk2dUIUpDownHoverButton upDownHoverButton)
    {
        if (upDownHoverButton.IsOver)
        {
            foreach (tk2dUIDropDownItem item in dropDownItems)
            {
                if (item.upDownHoverBtn != upDownHoverButton && item.upDownHoverBtn != null)
                {
                    item.upDownHoverBtn.IsOver = false;
                }
            }
        }
    }

    void OnDestroy()
    {
        foreach (tk2dUIDropDownItem item in dropDownItems)
        {
            item.OnItemSelected -= ItemSelected;
            if (item.upDownHoverBtn != null)
            {
                item.upDownHoverBtn.OnToggleOver -= DropDownItemHoverBtnToggle;
            }
        }
    }
}