using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{

    AudioClip[] sfxClip;
    AudioClip[] bgmClip;

    AudioSource sfxSource;
    AudioSource bgmSource;

    Coroutine coroutine;

    private void Start()
    {
        gameObject.name = "(SINGLETON) Audio Manager";
        DontDestroyOnLoad(this);
    }

    public void PlaySFX(int clip) => sfxSource.PlayOneShot(sfxClip[clip]);
    public void PlaySFX(AudioClip clip) => sfxSource.PlayOneShot(clip);
  
    public void PlayBGM(int clip)
    {
        if (bgmSource.isPlaying)
        {
            if (bgmSource.clip.Equals(bgmClip[clip])) return;
            
        }
        else
        {
            bgmSource.clip = bgmClip[clip];
            bgmSource.Play();
        }
    }

    public int GetBGMCount { get { return bgmClip.Length; } }

    public void LoadAudio() => coroutine = StartCoroutine(_preloadAudio());
    public void StopCoroutine() => StopCoroutine(coroutine);

    IEnumerator _preloadAudio()
    {

        sfxSource = gameObject.AddComponent<AudioSource>();
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;

        //anyway, use addressables so no resources
        bgmClip = ResourcesManager.Instance.AssignClipsToArray("BGM");
        sfxClip = ResourcesManager.Instance.AssignClipsToArray("SFX");
        yield return new WaitForFixedUpdate();

    }

    IEnumerator LerpAudio(AudioSource source, float from, float to, float duration)
    {
        var refresh = new WaitForEndOfFrame();
        float timeStart = 0;
        while (timeStart <= duration)
        {
            source.volume = Mathf.Lerp(from, to, timeStart);
            timeStart += Time.deltaTime;
            yield return refresh;
        }
    }

}
