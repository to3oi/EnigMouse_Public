using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MovieTest : MonoBehaviour
{
    VideoPlayer videoPlayer;
    AsyncOperation asyncOperation;
    [SerializeField] VideoClip videoClip;
    [SerializeField] Text text;

    // Start is called before the first frame update
    void Start()
    {
        // ビデオプレイヤーを取得
        videoPlayer = GetComponent<VideoPlayer>();

        // ビデオクリップを設定
        videoPlayer.clip = videoClip;

        // すぐに再生を始める
        videoPlayer.playOnAwake = true;

        // 最後まで再生したときの処理を追加
        videoPlayer.loopPointReached += FinishPlayingVideo;

        // 3秒後にシーンの読み込みを開始
        Invoke("StartLoadScene", 3f);
    }

    void StartLoadScene()
    {
        StartCoroutine("LoadScene");
    }

    IEnumerator LoadScene()
    {

        //asyncOperation = SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            // 進行度をテキストに表示
            text.text = "読み込み中: " + (asyncOperation.progress * 100) + "%";

            // シーンの読み込みが終わった後の処理
            if (asyncOperation.progress >= 0.9f)
            {
                text.text = "読み込み中: 100%";
            }

            yield return null;
        }
    }

    // 動画が終わったときの処理
    void FinishPlayingVideo(VideoPlayer vp)
    {
        // 再生を停止
        videoPlayer.Stop();

        // シーンを遷移
        asyncOperation.allowSceneActivation = true;

    }
}