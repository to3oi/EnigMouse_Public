using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;
using UnityEngine;

public enum KinectImageType
{
    RGB = 0,
    Depth = 1,
}

public class KinectScript : MonoBehaviour
{
    [SerializeField] private KinectImageType _imageType = KinectImageType.RGB;
    [SerializeField] private UnityEngine.UI.RawImage _viewerRawImage = null;
    [SerializeField] private int _depthDistanceMin = 500;
    [SerializeField] private int _depthDistanceMax = 5000;

    private Device _kinectDevice = null;

    private void Start()
    {
        InitKinect();
        StartLoop();
    }

    private void OnDestroy()
    {
        _kinectDevice.StopCameras();
    }

    private void InitKinect()
    {
        _kinectDevice = Device.Open(0);
        _kinectDevice.StartCameras(new DeviceConfiguration
        {
            ColorFormat = ImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R1080p,
            DepthMode = DepthMode.WFOV_2x2Binned ,
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS30
        });
    }

    private async void StartLoop()
    {
        while (true)
        {
            using (Capture capture = await Task.Run(() => _kinectDevice.GetCapture()).ConfigureAwait(true))
            {
                if (_imageType == 0)
                {
                    ViewColorImage(capture);
                }
                else
                {
                    ViewDepthImage(capture);
                }
            }
        }
    }

    // RGB情報をRawImageに表示
    private void ViewColorImage(Capture capture)
    {
        Image colorImage = capture.Color;
        int pixelWidth = colorImage.WidthPixels;
        int pixelHeight = colorImage.HeightPixels;

        BGRA[] bgraArr = colorImage.GetPixels<BGRA>().ToArray();
        Color32[] colorArr = new Color32[bgraArr.Length];

        for (int i = 0; i < colorArr.Length; i++)
        {
            int index = colorArr.Length - 1 - i;
            colorArr[i] = new Color32(
                bgraArr[index].R,
                bgraArr[index].G,
                bgraArr[index].B,
                bgraArr[index].A
            );
        }

        _viewerRawImage.texture = GetTexture2D(pixelWidth, pixelHeight, colorArr);
    }

    // 深度情報をRawImageに表示
    private void ViewDepthImage(Capture capture)
    {
        Image colorImage = capture.Depth;
        int pixelWidth = colorImage.WidthPixels;
        int pixelHeight = colorImage.HeightPixels;

        ushort[] depthByteArr = colorImage.GetPixels<ushort>().ToArray();
        Color32[] colorArr = new Color32[depthByteArr.Length];

        for (int i = 0; i < colorArr.Length; i++)
        {
            int index = colorArr.Length - 1 - i;

            int depthVal = 255 - (255 * (depthByteArr[index] - _depthDistanceMin) / _depthDistanceMax);
            if (depthVal < 0)
            {
                depthVal = 0;
            }
            else if (depthVal > 255)
            {
                depthVal = 255;
            }

            colorArr[i] = new Color32(
                (byte)depthVal,
                (byte)depthVal,
                (byte)depthVal,
                255
            );
        }

        _viewerRawImage.texture = GetTexture2D(pixelWidth, pixelHeight, colorArr);
    }

    // カラー配列 -> Texture2D
    private Texture2D GetTexture2D(int width, int height, Color32[] colorArr)
    {
        _viewerRawImage.rectTransform.sizeDelta = new Vector2(width, height);

        Texture2D resultTex = new Texture2D(width, height);
        resultTex.SetPixels32(colorArr);
        resultTex.Apply();
        return resultTex;
    }
}