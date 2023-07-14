using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    private List<AudioSource> sounds = new List<AudioSource>();

    private BGMType _bgmtype;
    private SEType _setype;

    private float MaxVol;

    public AudioClip[] clips = new AudioClip[5];

    List<AudioSource> pyaingSEAudioSources = new List<AudioSource>();
    List<AudioSource> pyaingBGMAudioSources = new List<AudioSource>();

    [SerializeField]
    private SEClips seClips;

    [SerializeField]
    private BGMClips bgmClips;

    // Start is called before the first frame update
    void Start()
    {
        //画面遷移してもオブジェクトが壊れないようにする
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < 5; i++)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            sounds.Add(audioSource);
        }
        //sounds = GetComponents<AudioSource>();
        sounds[1].clip = clips[1];
        sounds[2].clip = clips[2];
        sounds[3].clip = clips[3];
        sounds[4].clip = clips[4];
        sounds[5].clip = clips[5];
    }

    public void PlaySE(SEType seType)
    {
        AudioSource audioSource = GetAudioSource();

        foreach (var seClip in seClips.seClip)
        {
            if (seClip.SEType == seType)
            {
                audioSource.clip = seClip.clip;
                audioSource.Play();
                pyaingSEAudioSources.Add(audioSource);
                return;
            }
        }
    }

    public void PlayBGM(BGMType bgmType)
    {
        AudioSource audioSource = GetAudioSource();

        foreach (var bgmClip in bgmClips.bgmClip)
        {
            if (bgmClip.BGMType == bgmType)
            {
                audioSource.clip = bgmClip.clip;
                audioSource.Play();
                pyaingBGMAudioSources.Add(audioSource);
                return;
            }
        }
    }

    public void StopSE(SEType seType)
    {
        foreach (var audioSource in pyaingSEAudioSources)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            pyaingSEAudioSources.Remove(audioSource);
        }
    }


    public void StopBGM(BGMType bgmType)
    {
        foreach (var audioSource in pyaingBGMAudioSources)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            pyaingBGMAudioSources.Remove(audioSource);
        }
    }

    private AudioSource GetAudioSource()
    {
        foreach (var audio in sounds)
        {
            if (!audio.isPlaying)
            {
                return audio;
            }
        }
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        sounds.Add(audioSource);
        return audioSource;
    }

    //最大音量変更
    public void SetMaxVol(float _vol)
    {
        //ボリュームの最大値超過チェック
        if(_vol >= 1.0f)
        {
            _vol = 1.0f;
        }

        //ボリュームの反映
        MaxVol = _vol;
    }

    // 中に記述された処理が一定間隔で繰り返し実行される
    //void Update()
    //{
    //    //音源の重複無しで再生
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        sounds[0].Play();
    //    }
    //    if (Input.GetKeyDown(KeyCode.Alpha2))
    //    {
    //        sounds[1].Play();
    //    }

    //    //音源の重複ありで再生
    //    if (Input.GetKeyDown(KeyCode.Alpha3))
    //    {
    //        sounds[0].PlayOneShot(clips[0]);
    //    }
    //    if (Input.GetKeyDown(KeyCode.Alpha4))
    //    {
    //        //soundsは[0]でも[1]でもどちらも可。
    //        sounds[0].PlayOneShot(clips[1]);
    //    }


    //}

}
