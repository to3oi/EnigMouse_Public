using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class AudioSourceInfo
{
    public AudioSource AudioSource;
    public SoundHash SoundHash;
}

public class SoundManager : SingletonMonoBehaviour4Manager<SoundManager>
{
    private List<AudioSource> sounds = new List<AudioSource>();

    private BGMType _bgmtype;
    private SEType _setype;

    List<AudioSourceInfo> pyaingSEAudioSources = new List<AudioSourceInfo>();
    List<AudioSourceInfo> pyaingBGMAudioSources = new List<AudioSourceInfo>();

    [SerializeField] private SEClips seClips;

    [SerializeField] private BGMClips bgmClips;

    void Start()
    {
        //画面遷移してもオブジェクトが壊れないようにする
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i <= 5; i++)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            sounds.Add(audioSource);
        }
    }

    public SoundHash PlaySE(SEType seType)
    {
        AudioSource audioSource = GetAudioSourceSE();

        SEClip seClip = null;

        for (int i = 0; i < seClips.seClip.Count; i++)
        {
            if (seClips.seClip[i].SEType == seType)
            {
                seClip = seClips.seClip[i];
                break;
            }
        }

        audioSource.clip = seClip?.clip;
        audioSource.Play();
        var asInfo = new AudioSourceInfo
        {
            AudioSource = audioSource,
            SoundHash = new SoundHash()
        };

        pyaingSEAudioSources.Add(asInfo);
        return asInfo.SoundHash;
    }

    public SoundHash PlayBGM(BGMType bgmType)
    {
        AudioSource audioSource = GetAudioSourceBGM();
        BGMClip bgmClip = null;

        for (int i = 0; i < bgmClips.bgmClip.Count; i++)
        {
            if (bgmClips.bgmClip[i].BGMType == bgmType)
            {
                bgmClip = bgmClips.bgmClip[i];
                break;
            }
        }

        audioSource.clip = bgmClip?.clip;
        audioSource.Play();
        var asInfo = new AudioSourceInfo
        {
            AudioSource = audioSource,
            SoundHash = new SoundHash()
        };
            pyaingBGMAudioSources.Add(asInfo);
        return asInfo.SoundHash;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="soundHash"></param>
    /// <param name="fadeTime">SE消滅する時間</param>
    public async UniTask StopSE(SoundHash soundHash,float fadeTime = 0.25f)
    {
        AudioSourceInfo asInfo = null;
        for (int i = 0; i < pyaingSEAudioSources.Count; i++)
        {
            if (pyaingSEAudioSources[i].SoundHash == soundHash && pyaingSEAudioSources[i].AudioSource.isPlaying)
            {
                asInfo = pyaingSEAudioSources[i];
                await DOVirtual.Float(asInfo.AudioSource.volume, 0, fadeTime, v =>
                {
                    asInfo.AudioSource.volume = v;
                    if (v == 0)
                    {
                        asInfo.AudioSource.Stop();
                        if (asInfo != null)
                        {
                            pyaingSEAudioSources.Remove(asInfo);
                        }
                    }
                }).ToUniTask();
            }
        }
    }


    public async UniTask StopBGM(SoundHash soundHash,float fadeTime = 0.25f)
    {
        
        AudioSourceInfo asInfo = null;
        for (int i = 0; i < pyaingBGMAudioSources.Count; i++)
        {
            if (pyaingBGMAudioSources[i].SoundHash == soundHash && pyaingBGMAudioSources[i].AudioSource.isPlaying)
            {
                asInfo = pyaingBGMAudioSources[i];
                await  DOVirtual.Float(asInfo.AudioSource.volume, 0, fadeTime, v =>
                {
                    asInfo.AudioSource.volume = v;
                    if (v == 0)
                    {
                        asInfo.AudioSource.Stop();
                        if (asInfo != null)
                        {
                            pyaingBGMAudioSources.Remove(asInfo);
                        }
                    }
                }).ToUniTask();
            }
        }
    }


    private AudioSource GetAudioSourceSE()
    {
        var audioSource = GetAudioSource();
        audioSource.loop = false;
        audioSource.volume = 1.0f;
        return audioSource;
    }

    private AudioSource GetAudioSourceBGM()
    {
        var audioSource = GetAudioSource();
        audioSource.loop = true;
        audioSource.volume = 0.1f;
        return audioSource;
    }

    private AudioSource GetAudioSource()
    {
        foreach (var audio in sounds)
        {
            if (!audio.isPlaying)
            {
                audio.volume = 1;
                return audio;
            }
        }

        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 1;
        sounds.Add(audioSource);
        return audioSource;
    }

    public async UniTask AllStopSE()
    {
        List<UniTask> task = new List<UniTask>();
        foreach (var asInfo in pyaingSEAudioSources)
        { 
            task.Add(
            DOVirtual.Float(asInfo.AudioSource.volume, 0, 0.25f, v =>
            {
                asInfo.AudioSource.volume = v;
                if (v == 0)
                {
                    asInfo.AudioSource.Stop();
                    if (asInfo != null)
                    {
                        pyaingSEAudioSources.Remove(asInfo);
                    }
                }
            }).ToUniTask());
        }

        await UniTask.WhenAll(task);
    }
    
    public async UniTask AllStopBGM()
    {
        List<UniTask> task = new List<UniTask>();
        foreach (var asInfo in pyaingBGMAudioSources)
        { 
            task.Add(
                DOVirtual.Float(asInfo.AudioSource.volume, 0, 0.25f, v =>
                {
                    asInfo.AudioSource.volume = v;
                    if (v == 0)
                    {
                        asInfo.AudioSource.Stop();
                        if (asInfo != null)
                        {
                            pyaingBGMAudioSources.Remove(asInfo);
                        }
                    }
                }).ToUniTask());
        }

        await UniTask.WhenAll(task);
    }
}