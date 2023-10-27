using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Text;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    public AudioMixer audioMixer;
    public AudioClip[] bglist;

    private AudioSource bgSound;

    StringBuilder sb = new StringBuilder();

    private void Awake()
    {
        if(instance != null)
        {
            DestroyImmediate(this.gameObject);
        }
        instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        bgSound = GetComponent<AudioSource>();
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        for(int i = 0; i < bglist.Length; i++)
        {
            if(arg0.name == bglist[i].name)
            {
                BgSoundPlay(bglist[i]);
            }
        }
    }

    public void SFXPlay(string sfxName, AudioClip audioClip)
    {
        sb.Append($"{sfxName}+Sound");
        GameObject go = new GameObject(sb.ToString());
        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        audioSource.clip = audioClip;
        audioSource.Play();
        sb.Clear();

        Destroy(go, audioClip.length);
    }

    public void BgSoundPlay(AudioClip clip)
    {
        if (bgSound == null) return;

        bgSound.outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGSound")[0];
        bgSound.clip = clip;
        bgSound.loop = true;
        bgSound.volume = 0.1f;
        bgSound.Play();
    }

    private void OnDestroy()
    {
        bgSound.Stop();
        instance = null;
   
    }

}
