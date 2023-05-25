using UnityEngine;
//(追加1)AzureKinectSDKの読み込み
using Microsoft.Azure.Kinect.Sensor;
public class KinectScript : MonoBehaviour
{
    //(追加2)Kinectを扱う変数
    Device kinect;
    
    void Start()
    {
        //(追加5)最初の一回だけKinect初期化メソッドを呼び出す
        InitKinect();
    }
    //(追加3)Kinectの初期化(Form1コンストラクタから呼び出す)
    private void InitKinect()
    {

        //(追加4)0番目のKinectと接続したのちにKinectの各種モードを設定して動作開始
        kinect = Device.Open(0);     
        kinect.StartCameras(new DeviceConfiguration
        {
            ColorFormat = ImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_2x2Binned,
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS30
        });
    }
    //(追加6)このオブジェクトが消える(アプリ終了)と同時にKinectを停止
    private void OnDestroy()
    {
        kinect.StopCameras();
    }
}