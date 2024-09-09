public class PowerupItemData : BaseItemData
{

    public PowerupType ItemType;
    public PowerupActivation ActivationMode;
    public PowerupItemCardColor CardColor;
    public int MaxCount = 1;

    // Used in inventory
    public int CurrentAmount { get; set; }
    public bool IsOwned { get; set; }
}
