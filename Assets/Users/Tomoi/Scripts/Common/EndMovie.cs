using UnityEngine;
using UnityEngine.Video;
[RequireComponent(typeof(VideoPlayer))]
public class EndMovie : MonoBehaviour
{
    VideoPlayer videoPlayer;

    [SerializeField] private VideoClip videoClip;
    [SerializeField] private BGMType bgmType;
    private SoundHash BGMHash;
    
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.clip = videoClip;
        videoPlayer.playOnAwake = true;
        videoPlayer.isLooping = false;
        videoPlayer.targetCamera = Camera.main;
        videoPlayer.loopPointReached += FinishPlayingVideo;
        BGMHash = SoundManager.Instance.PlayBGM(bgmType);
    }

    private void FinishPlayingVideo(VideoPlayer vp)
    {
        //TODO:タイミング調整
        SoundManager.Instance.StopBGM(BGMHash);
        SceneManager.Instance.SceneChange(SceneList.Title,true,true);
    }
}
