using System.Collections;
using HighlightPlus;
using UnityEngine;

public class TutorialStage : ITutorialStage
{
    [SerializeField] private GameObject outliner;
    
    public void EmphasizeBlocks() => StartCoroutine(IEEmphasizeBlocks());
    public void EmphasizeTiles(bool emphasize) => StartCoroutine(IEEmphasizeTiles(emphasize));

    private IEnumerator IEEmphasizeBlocks()
    {
        outliner.SetActive(true);

        for (var i = 0; i < sequenceFillers.Length; i++)
        {
            var filler = sequenceFillers[i];

            yield return StartCoroutine(filler.IEHideSequences());
        }

        for (var i = 0; i < sequenceFillers.Length; i++)
        {
            var filler = sequenceFillers[i];

            yield return StartCoroutine(filler.IEShowSequences());
        }

        outliner.SetActive(false);

        TutorialEventNotifier.NotifyObserver(TutorialEventNames.ShowContinueButton, null);
        yield return null;
    }

    private IEnumerator IEEmphasizeTiles(bool emphasize)
    {
        for (var i = 0; i < sequenceFillers.Length; i++)
        {
            var highlight = sequenceFillers[i].GetComponentInChildren<HighlightEffect>();

            if (highlight != null)
                highlight.enabled = emphasize;

            yield return null;
        }
    }

    public void HighlightOrangeTile() => HighlightTile(ColorSwatches.Orange);
    public void HighlightBlueTile() => HighlightTile(ColorSwatches.Blue);
    public void HighlightPinkTile() => HighlightTile(ColorSwatches.Magenta);
    public void HighlightPurpleTile() => HighlightTile(ColorSwatches.Purple);

    private void HighlightTile(ColorSwatches tileColor)
    {
        for (var i = 0; i < sequenceFillers.Length; i++)
        {
            var sequence  = sequenceFillers[i];

            if (!sequence.transform.GetChild(0).gameObject.activeInHierarchy)
                continue;

            var highlight = sequence.GetComponentInChildren<HighlightEffect>();
            
            if (highlight == null)
                continue;

            sequence.TryGetComponent(out TutorialBlockFiller filler);

            if (filler.GetFillColor() == tileColor)
                highlight.enabled = true;
            else
                highlight.enabled = false;
        }
    }
}
