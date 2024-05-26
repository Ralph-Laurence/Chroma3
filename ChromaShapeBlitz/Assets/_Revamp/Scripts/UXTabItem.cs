using UnityEngine.UI;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Toggle))]
public class UXTabItem : MonoBehaviour
{
    private const string UXResPath = "Sprites/UX";
    private const string AssetTabActive = "ux-tab-active";
    private const string AssetTabInactive = "ux-tab-inactive";
    
    [Space(5)] [Header("Background Properties")]
    [SerializeField] private Image background;
    [SerializeField] private Sprite defaultBackground;
    [SerializeField] private Sprite activeBackground;
        
    [Space(5)] [Header("Icon Properties")]
    [SerializeField] private Image icon;
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Sprite activeIcon;

    [Space(5)] [Header("Content Behaviour")]
    [SerializeField] private GameObject tabContent;
    [SerializeField] private int _tabIndex;
    public int TabIndex { get => _tabIndex; set => _tabIndex = value; }

    [SerializeField] private Toggle toggle;
    [SerializeField] private string guid;

    void Awake()
    {
        if (background == null)
            transform.Find("Background").TryGetComponent(out background);

        if (icon == null)
           background.transform.Find("Icon").TryGetComponent(out icon);

        if (defaultBackground == null)
           defaultBackground = Resources.Load<Sprite>( $"{UXResPath}/{AssetTabInactive}" );

        if (activeBackground == null)
           activeBackground = Resources.Load<Sprite>( $"{UXResPath}/{AssetTabActive}" );

        TryGetComponent(out toggle);

        toggle.onValueChanged.AddListener( delegate { ToggleChanged(toggle); } );
    }

    public void Select() => toggle.isOn = true;

    private void ToggleChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            background.sprite = activeBackground;

            if (activeIcon != null)
                icon.sprite = activeIcon;

            OnTabItemSelectedNotifier.Publish(this);
            return;
        }

        background.sprite = defaultBackground;

        if (defaultIcon != null)
            icon.sprite = defaultIcon;
    }

    public void SetGuid(string guid) => this.guid = guid;
    public string Guid => guid;

    public void ShowPage() => tabContent.SetActive(true);
    public void HidePage() => tabContent.SetActive(false);
}
