public class BuySkinsResultDialog : BuyItemResultDialog
{
    protected override void OnDialogShown()
    {
        DialogEventNotifier.NotifyObserver(DialogEventNames.BuySkinsDialogResultShown);
    }

    protected override void OnDialogHidden()
    {
        DialogEventNotifier.NotifyObserver(DialogEventNames.BuySkinsDialogResultHidden);
    }
}
