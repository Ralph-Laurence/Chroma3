public class TutorialSelectionPage : LevelSelectPage
{
    public const int MaxStages = 5;

    public override void MarkCompleted()
    {
        base.MarkCompleted();
        this.button.interactable = false;
    }
}
