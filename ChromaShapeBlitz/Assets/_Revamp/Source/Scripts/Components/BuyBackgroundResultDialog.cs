using UnityEngine;

public class BuyBackgroundResultDialog : BuyItemResultDialog
{
    protected override void OnDialogShown()
    {
        DialogEventNotifier.NotifyObserver(DialogEventNames.BuyBackgroundDialogResultShown);
    }

    protected override void OnDialogHidden()
    {
        DialogEventNotifier.NotifyObserver(DialogEventNames.BuyBackgroundDialogResultHidden);
    }
}
