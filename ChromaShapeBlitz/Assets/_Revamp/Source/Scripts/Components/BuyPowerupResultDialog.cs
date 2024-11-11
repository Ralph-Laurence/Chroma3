 public class BuyPowerupResultDialog : BuyItemResultDialog
{
    protected override void OnDialogShown()
    {
        DialogEventNotifier.NotifyObserver(DialogEventNames.BuyPowerupsDialogResultShown);
    }

    protected override void OnDialogHidden()
    {
        DialogEventNotifier.NotifyObserver(DialogEventNames.BuyPowerupsDialogResultHidden);
    }
}