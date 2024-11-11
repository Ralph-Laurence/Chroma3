using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MaskReverseStencil : Image
{
    public override Material materialForRendering
    {
        get {
            var mat = new Material(base.materialForRendering);
            mat.SetFloat("_StencilComp", (float)CompareFunction.NotEqual);

            return mat;
        }
    }
}
