using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UXTab : MonoBehaviour
{
    [SerializeField] private List<UXTabItem> tabItems;
    [SerializeField] private string tabGUID;
    //private ToggleGroup toggleGroup;

    void Awake()
    {
        tabGUID = Guid.NewGuid().ToString();

        //TryGetComponent(out toggleGroup);

        var tabs = GetComponentsInChildren<UXTabItem>();
        tabItems = new List<UXTabItem>(tabs);               // Initialize the list of tab items

        for (var i = 0; i < tabItems.Count; i++)
        {
            tabItems[i].TabIndex = i;                       // Assign each tab item with their index
            tabItems[i].SetGuid(tabGUID);

            if (i == 0 && tabItems[i] != null)              // The very first tab item must always be selected
                tabItems[i].Select();
        }
    }

    //
    //*********************************************
    // ---------------- TAB EVENTS ----------------
    //*********************************************
    //
    void OnEnable()  => OnTabItemSelectedNotifier.Event.AddListener(Subscribe);

    void OnDisable() => OnTabItemSelectedNotifier.Event.RemoveListener(Subscribe);

    public void Subscribe(UXTabItem tabItem)
    {
        if (tabItem.Guid.Equals(tabGUID))
        {
            //Debug.Log(tabItem.Guid);
            HandleTabSelected(tabItem);
        }
    }

    private void HandleTabSelected(UXTabItem tabItem)
    {
        var selected = tabItem.TabIndex;

        tabItems.ForEach(tab => {
            if (tab.TabIndex != selected)
            {
                tab.HidePage();
                return;
            }

            tab.ShowPage();
        });
    }
}