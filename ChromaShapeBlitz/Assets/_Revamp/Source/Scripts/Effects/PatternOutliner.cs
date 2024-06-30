using System;
using UnityEngine;
using UnityEngine.Rendering;


/// <summary>
/// Rules:  ONLY PARENT gets an outline;
///         ONLY VISIBLE objects render an outline
/// </summary>
public class PatternOutliner : MonoBehaviour
{
    private Shader _outlineShader;
    private Shader _targetShader;

    public enum SelMode : int
    {
        OnlyParent = 0,
        AndChildren = 1
    }
    public enum AlphaType : int
    {
        KeepHoles = 0,
        Intact = 1
    }
    public enum OutlineMode : int
    {
        Whole = 0,
        ColorizeOccluded = 1,
        OnlyVisible = 2
    }

    public SelMode SelectionMode = SelMode.AndChildren;
    [Tooltip("The last two type will require rendering an extra Camera Depth Texture.")]
    public OutlineMode OutlineType = OutlineMode.ColorizeOccluded;
    [Tooltip("Decide whether the alpha data of the main texture affect the outline.")]
    public AlphaType AlphaMode = AlphaType.KeepHoles;
    private Renderer TargetRenderer, lastTarget;

    [ColorUsageAttribute(true, true)]
    public Color OutlineColor = new Color(1f, 0.55f, 0f), OccludedColor = new Color(0.5f, 0.9f, 0.3f);
    [Range(0, 1)]
    public float OutlineWidth = 0.2f;
    [Range(0, 1)]
    public float OutlineHardness = 0.85f;

    [SerializeField] private Transform testObject;
    private Material outlineMat;
    private RenderTexture rtMask;
    private RenderTexture rtOutline;
    private Camera cam;
    private CommandBuffer cmd;
    void Awake()
    {
        _outlineShader = Shader.Find("Outline/PostprocessOutline");
        _targetShader  = Shader.Find("Outline/Target");
        
        if (!VerifyCanUse())
            return;

        TryGetComponent(out cam);

        cam.depthTextureMode = OutlineType > 0 ? DepthTextureMode.None : DepthTextureMode.Depth;

        outlineMat = new Material(_outlineShader);

        Shader.EnableKeyword("_COLORIZE");
        rtMask = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.RFloat);
        rtOutline = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.RG16);
        Shader.EnableKeyword("_OCCLUDED");

        cam.RemoveAllCommandBuffers();
        cmd = new CommandBuffer { name = "Outline Command Buffer" };
        cmd.SetRenderTarget(rtMask);
        cam.AddCommandBuffer(CameraEvent.BeforeImageEffects, cmd);
    }

    /// <summary>
    /// Check if we can use this component.
    /// </summary>
    private bool VerifyCanUse()
    {
        if (_outlineShader != null && _targetShader != null)
            return true;

        Debug.LogError("Can't find the outline shaders, please check the Always Included Shaders in Graphics settings.");
        return false;
    }
 
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        outlineMat.SetFloat("_OutlineWidth", OutlineWidth * 10f);
        outlineMat.SetFloat("_OutlineHardness", 8.99f * (1f - OutlineHardness) + 0.01f);
        outlineMat.SetColor("_OutlineColor", OutlineColor);
        outlineMat.SetColor("_OccludedColor", OccludedColor);

        outlineMat.SetTexture("_Mask", rtMask);
        Graphics.Blit(source, rtOutline, outlineMat, 0);
        outlineMat.SetTexture("_Outline", rtOutline);
        Graphics.Blit(source, destination, outlineMat, 1);

    }
    void RenderTarget(Renderer target)
    {
        var targetMat   = new Material(_targetShader);
        var mainTexFlag = false;
        string[] attrs  = target.sharedMaterial.GetTexturePropertyNames();

        foreach (var c in attrs)
        {
            if (c.Equals("_MainTex"))
            {
                mainTexFlag = true;
                break;
            }
        }

        if (mainTexFlag && target.sharedMaterial.mainTexture != null && AlphaMode == AlphaType.KeepHoles)
        {
            targetMat.mainTexture = target.sharedMaterial.mainTexture;
        }

        cmd.DrawRenderer(target, targetMat);
        Graphics.ExecuteCommandBuffer(cmd);
    }
    void SetTarget()
    {
        cmd.SetRenderTarget(rtMask);
        cmd.ClearRenderTarget(true, true, Color.black);

        if (TargetRenderer != null)
            RenderTarget(TargetRenderer);

        else
            Debug.LogWarning("No renderer provided for outline.");
    }
    void ClearTarget()
    {
        cmd.ClearRenderTarget(true, true, Color.black);

        Graphics.ExecuteCommandBuffer(cmd);
        cmd.Clear();
    }
    // Update is called once per frame
    void Start()
    {
        Reset();

        testObject.TryGetComponent(out TargetRenderer);

        if (lastTarget == null) lastTarget = TargetRenderer;

        SetTarget();
        // if (TargetRenderer != lastTarget)
        // {
        //     SetTarget();
        // }
        
        lastTarget = TargetRenderer;
    }

    private void Reset()
    {
        TargetRenderer = null;
        lastTarget = null;
        ClearTarget();
    }
}
