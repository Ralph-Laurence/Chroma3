using UnityEngine.Events;

public class BlockSkinShopItemClickedEvent : UnityEvent<BlockSkinShopItem> { }

public class OnBlockSkinShopItemClickedNotifier
{
    public static BlockSkinShopItemClickedEvent Event = new BlockSkinShopItemClickedEvent();

    public static void Publish(BlockSkinShopItem sender) => Event.Invoke(sender);
}
