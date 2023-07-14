/*using System.Threading.Tasks;
using Microsoft.Azure.Kinect.Sensor;
using OpenCvSharp;
using UnityEngine;
using UnityEngine.UI;
using Image = Microsoft.Azure.Kinect.Sensor.Image;
using Cysharp.Threading.Tasks;

public class OpenCVPlusUnity : MonoBehaviour
{
    [SerializeField] private RawImage _renderer;

    [SerializeField] private int _depthDistanceMin = 500;
    [SerializeField] private int _depthDistanceMax = 2000;

    [Space(10)] [SerializeField] private int _irDistanceMin = 500;
    [SerializeField] private int _irDistanceMax = 2000;

    [Space(10)] [SerializeField] private int _depthThresholdMin = 254;
    [SerializeField] private int _depthThresholdMax = 255;


    [Space(10)] [SerializeField] private int _irThresholdMin = 254;
    [SerializeField] private int _irThresholdMax = 255;

    [Space(10)] [SerializeField] private bool isMask = true;

    private Device _kinectDevice = null;


    private void Start()
    {
        InitKinect();
        StartLoop().Forget();
    }

    void OnDestroy()
    {
        //Kinectの停止
        _kinectDevice.StopCameras();
    }

    /// <summary>
    /// Kinectの初期化
    /// </summary>
    private void InitKinect()
    {
        _kinectDevice = Device.Open(0);
        _kinectDevice.StartCameras(new DeviceConfiguration
        {
            ColorFormat = ImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_Unbinned,
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS30
        });
    }

    private async UniTaskVoid StartLoop()
    {
        //最終的に表示する画像サイズに合わせる 
        //640 x 576 * 2
        _renderer.rectTransform.sizeDelta = new Vector2(1280, 1152);
        Texture2D outDstTexture = new Texture2D(1, 1);
        while (true)
        {
            using (Capture capture = await Task.Run(() => _kinectDevice.GetCapture()).ConfigureAwait(true))
            {
                MonoBehaviour.Destroy(_renderer.texture);
                //深度カメラの処理
                Mat depthMat = new Mat();
                depthMat = ViewDepthImage(capture);
                depthMat.Reshape(1);

                //深度カメラの画像のチャンネル数とタイプを変更
                Mat tempDepthMatGray = new Mat();
                Cv2.CvtColor(depthMat, tempDepthMatGray, ColorConversionCodes.RGB2GRAY);
                depthMat.Dispose();
                Mat tempDepthMatBit = new Mat();
                Cv2.Threshold(tempDepthMatGray, tempDepthMatBit, _depthThresholdMin, _depthThresholdMax,
                    ThresholdTypes.Binary);
                tempDepthMatGray.Dispose();

                //IRカメラの処理
                Mat irMat = new Mat();
                irMat = ViewIRImage(capture);
                irMat.Reshape(1);

                //IRカメラの画像のチャンネル数とタイプを変更
                Mat tempIrMatGray = new Mat();
                Cv2.CvtColor(irMat, tempIrMatGray, ColorConversionCodes.RGB2GRAY);
                irMat.Dispose();
                Mat tempIrMatBit = new Mat();
                Cv2.Threshold(tempIrMatGray, tempIrMatBit, _irThresholdMin, _irThresholdMax, ThresholdTypes.BinaryInv);
                tempIrMatGray.Dispose();

                if (isMask)
                {
                    //マスクをかける
                    Mat outDst = new Mat();
                    Cv2.BitwiseAnd(tempDepthMatBit, tempDepthMatBit, outDst, tempIrMatBit);


                    //Texture2Dに変換と適応
                    outDstTexture = new Texture2D(outDst.Width, outDst.Height, TextureFormat.RGBA32, false);
                    OpenCvSharp.Unity.MatToTexture(outDst, outDstTexture);
                    _renderer.texture = outDstTexture;
                }
                else
                {
                    outDstTexture = new Texture2D(tempDepthMatBit.Width, tempDepthMatBit.Height,
                        TextureFormat.RGBA32, false);
                    OpenCvSharp.Unity.MatToTexture(tempDepthMatBit, outDstTexture);
                    _renderer.texture = outDstTexture;
                }

                tempDepthMatBit.Dispose();
                tempIrMatBit.Dispose();
                capture.Dispose();
            }
        }
    }

    private Mat ViewColorImage(Capture capture)
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

        colorImage.Dispose();
        return GetMat(pixelWidth, pixelHeight, colorArr);
    }

    private Mat ViewDepthImage(Capture capture)
    {
        Image depthImage = capture.Depth;
        int pixelWidth = depthImage.WidthPixels;
        int pixelHeight = depthImage.HeightPixels;

        ushort[] depthByteArr = depthImage.GetPixels<ushort>().ToArray();
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

        depthImage.Dispose();
        return GetMat(pixelWidth, pixelHeight, colorArr);
    }


    private Mat ViewIRImage(Capture capture)
    {
        Image irImage = capture.IR;
        int pixelWidth = irImage.WidthPixels;
        int pixelHeight = irImage.HeightPixels;

        ushort[] irByteArr = irImage.GetPixels<ushort>().ToArray();
        Color32[] colorArr = new Color32[irByteArr.Length];

        for (int i = 0; i < colorArr.Length; i++)
        {
            int index = colorArr.Length - 1 - i;

            int irVal = 255 - (255 * (irByteArr[index] - _irDistanceMin) / _irDistanceMax);
            if (irVal < 0)
            {
                irVal = 0;
            }
            else if (irVal > 255)
            {
                irVal = 255;
            }

            colorArr[i] = new Color32(
                (byte)irVal,
                (byte)irVal,
                (byte)irVal,
                255
            );
        }

        irImage.Dispose();
        return GetMat(pixelWidth, pixelHeight, colorArr);
    }

    /// <summary>
    /// Color32[]からMatを作成する
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="colorArr"></param>
    /// <returns></returns>
    private Mat GetMat(int width, int height, Color32[] colorArr)
    {
        return OpenCvSharp.Unity.PixelsToMat(colorArr, width, height, false, false, 0);
    }
}*/