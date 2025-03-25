using UnityEngine;

public class SnowController : MonoBehaviour
{
    public ComputeShader snowComputeShader;
    public RenderTexture snowRT;
    public RenderTexture colorRT;
    public float colorValueToAdd;


    //Properties
    private string snowImageProperty = "snowImage";
    private string colorValueProperty = "colorValueToAdd";
    private string resolutionProperty = "resolution";
    private string positionXProperty = "positionX";
    private string positionYProperty = "positionY";
    private string spotSizeProperty = "spotSize";
    private string drawingColorProperty = "drawingColor";

    private string csMainKernel = "CSMain";
    private string fillWhiteKernel = "FillWhite";

    private MeshRenderer meshRenderer;
    public int resolution = 512;


    private void Awake()
    {
        CreateRenderTexture();
        SetRTColorToWhite(snowRT);
        SetRTColorToWhite(colorRT);
        SetMaterialTexture();
        InvokeRepeating(nameof(AddSnowLayer), 0.1f, 0.1f);
        ExtendBoundsOfMesh();
    }

    void CreateRenderTexture()
    {
        snowRT = new RenderTexture(resolution, resolution, 24);
        snowRT.enableRandomWrite = true;
        snowRT.Create();
        colorRT = new RenderTexture(resolution, resolution, 24);
        colorRT.enableRandomWrite = true;
        colorRT.Create();
    }

    void SetRTColorToWhite(RenderTexture text)
    {
        int kernel_handle = snowComputeShader.FindKernel(fillWhiteKernel);
        snowComputeShader.SetTexture(kernel_handle, snowImageProperty, text);
        snowComputeShader.SetFloat(colorValueProperty, colorValueToAdd);
        snowComputeShader.SetFloat(resolutionProperty, resolution);
        snowComputeShader.SetFloat(positionXProperty, 0);
        snowComputeShader.SetFloat(positionYProperty, 0);
        snowComputeShader.SetFloat(spotSizeProperty, 0);
        snowComputeShader.Dispatch(kernel_handle, text.width / 8, text.height / 8, 1);
    }

    void SetMaterialTexture()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.SetTexture("_PathTexture", snowRT);
        meshRenderer.material.SetTexture("_ColorTexture", colorRT);
    }

    void AddSnowLayer()
    {
        int kernel_handle = snowComputeShader.FindKernel(csMainKernel);
        snowComputeShader.SetTexture(kernel_handle, snowImageProperty, snowRT);
        snowComputeShader.SetFloat(colorValueProperty, colorValueToAdd);
        snowComputeShader.SetFloats(drawingColorProperty, new float[] { colorValueToAdd, colorValueToAdd, colorValueToAdd, colorValueToAdd });
        snowComputeShader.SetFloat(resolutionProperty, resolution);
        snowComputeShader.SetFloat(positionXProperty, 0);
        snowComputeShader.SetFloat(positionYProperty, 0);
        snowComputeShader.SetFloat(spotSizeProperty, 0);
        snowComputeShader.Dispatch(kernel_handle, snowRT.width / 8, snowRT.height / 8, 1);
    }

    void ExtendBoundsOfMesh()
    {
        Bounds bounds = GetComponent<MeshFilter>().mesh.bounds;
        bounds.extents = new Vector3(2, 0, 2);
        GetComponent<MeshFilter>().mesh.bounds = bounds;
    }
}
