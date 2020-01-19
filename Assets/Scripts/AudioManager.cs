using UnityEngine;
using System;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    // Soundクラス配列
    public Sound[] sounds;
    // シングルトン化
    private AudioManager audioManager;

    private void Awake()
    {
        // AudioManagerインスタンスが存在しなければ生成
        // 存在すればDestroy, return
        if (audioManager == null)
        {
            audioManager = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        // Soundクラスに入れたデータをAudioSourceに当てはめる
        foreach (Sound s in sounds)
        {
            s.audioSource = gameObject.AddComponent<AudioSource>();
            s.audioSource.clip = s.clip;
            s.audioSource.volume = s.volume;
        }
    }

    public void Play(string name)
    {
        // ラムダ式　第二引数はPredicate
        // Soundクラスの配列の中の名前に，
        // 引数nameに等しいものがあるかどうか確認
        Sound s = Array.Find(sounds, sound => sound.name == name);
        // なければreturn
        if (s == null)
        {
            print("Sound" + name + "was not found");
            return;
        }
        // あればPlay()
        s.audioSource.Play();
    }

    public void PlayOneShot(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            print("Sound" + name + "was not found");
            return;
        }
        s.audioSource.PlayOneShot(s.clip);
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            return;
        }

        s.audioSource.Stop();
        s.audioSource.clip = null;
    }
}
