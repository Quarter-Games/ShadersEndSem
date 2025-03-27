using UnityEngine;

public class SnowPathDrawer : MonoBehaviour
{
    public ComputeShader snowComputeShader;
    public RenderTexture snowRT;
    public RenderTexture colorRT;

    private string snowImageProperty = "snowImage";
    private string colorValueProperty = "colorValueToAdd";
    private string resolutionProperty = "resolution";
    private string positionXProperty = "positionX";
    private string positionYProperty = "positionY";
    private string spotSizeProperty = "spotSize";
    private string drawingColorProperty = "drawingColor";

    private string drawSpotKernel = "DrawSpot";
    [SerializeField]
    [ColorUsage(true, true)] Color color = Color.black;
    public bool isDrawing = false;


    private Vector2Int position = new Vector2Int(256, 256);
    public float spotSize = 5f;

    private SnowController snowController;
    private GameObject[] snowControllerObjs;

    private void Awake()
    {
        snowControllerObjs = GameObject.FindGameObjectsWithTag("SnowGround");
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < snowControllerObjs.Length; i++)
        {
            if (Vector3.Distance(snowControllerObjs[i].transform.position, transform.position) > spotSize * 5f) continue;

            snowController = snowControllerObjs[i].GetComponent<SnowController>();
            snowRT = snowController.snowRT;
            colorRT = snowController.colorRT;
            //snowComputeShader = snowController.snowComputeShader;
            GetPosition();
            DrawSpot();
        }
    }

    void GetPosition()
    {
        float scaleX = snowController.transform.localScale.x;
        float scaleY = snowController.transform.localScale.z;

        float snowPosX = snowController.transform.position.x;
        float snowPosY = snowController.transform.position.z;


        int posX = snowRT.width / 2 - (int)(((transform.position.x - snowPosX) * snowRT.width / 2) / scaleX);
        int posY = snowRT.width / 2 - (int)(((transform.position.z - snowPosY) * snowRT.height / 2) / scaleY);
        position = new Vector2Int(posX, posY);
    }

    void DrawSpot()
    {
        if (snowRT == null) return;
        if (snowComputeShader == null) return;

        int kernel_handle = snowComputeShader.FindKernel(drawSpotKernel);
        snowComputeShader.SetTexture(kernel_handle, snowImageProperty, isDrawing ? colorRT : snowRT);
        snowComputeShader.SetFloat(colorValueProperty, 0);
        snowComputeShader.SetFloats(drawingColorProperty, isDrawing ? new float[] { color.r, color.g, color.b, color.a } : new float[] { 0, 0, 0, 0 });
        snowComputeShader.SetFloat(resolutionProperty, isDrawing ? colorRT.width : snowRT.width);
        snowComputeShader.SetFloat(positionXProperty, position.x);
        snowComputeShader.SetFloat(positionYProperty, position.y);
        snowComputeShader.SetFloat(spotSizeProperty, spotSize);
        snowComputeShader.Dispatch(kernel_handle, isDrawing ? colorRT.width : snowRT.width / 8, isDrawing ? colorRT.height : snowRT.height / 8, 1);
    }

    public void drawing(bool isDrawing)
    {
        this.isDrawing = isDrawing;
    }

    public void ColorRed(float R)
    {
        color.r = R;
    }

    public void ColorGreen(float G)
    {
        color.g = G;
    }

    public void ColorBlue(float Blue)
    {
        color.b = Blue;
    }
}
