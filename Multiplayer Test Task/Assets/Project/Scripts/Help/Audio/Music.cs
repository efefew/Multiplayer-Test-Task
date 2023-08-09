using System.Collections;

using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class Music : MonoBehaviour
{
    #region Fields

    private const float minCutoffFrequency = 500, maxCutoffFrequency = 22000;

    private AudioSource audioSource;
    private AudioMixer audioMixer;
    private int musicTrackID;
    public AudioClip[] music;

    #endregion Fields

    #region Methods

    private void Awake()
    {
        audioMixer = (AudioMixer)Resources.Load("Audio/AudioMixer");
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() => audioMixer.SetFloat("musicCutoffFrequency", minCutoffFrequency);

    private void OnEnable()
    {
        ShuffleTheMusicTrack();
        _ = StartCoroutine(playMusic());
    }

    private IEnumerator playMusic()
    {
        while (true)
        {
            NextMusic();
            yield return new WaitForSeconds(music[musicTrackID - 1].length);
        }
    }

    /// <summary>
    /// перемешать музыковую дорожку
    /// </summary>
    [ContextMenu("ShuffleTheMusicTrack")]
    private void ShuffleTheMusicTrack()
    {
        musicTrackID = 0;
        int id;
        AudioClip temp;
        for (int i = 1; i < music.Length; i++)
        {
            int k = i - 1;
            id = Random.Range(k, music.Length - k);
            temp = music[k];
            music[k] = music[id];
            music[id] = temp;
        }
    }

    /// <summary>
    /// Плавная активация заглушки музыки (частота среза)
    /// </summary>
    /// <param name="duration">длительность активации</param>
    /// <param name="on">заглушить?</param>
    /// <returns></returns>
    private IEnumerator ActivateCutoffFrequencyCoroutine(float duration, bool on)
    {
        //Инициализируем счётчиков прошедшего времени
        float elapsed = 0f;
        _ = audioMixer.GetFloat("musicCutoffFrequency", out float startCutoffFrequency);
        float targetCutoffFrequency = on ? minCutoffFrequency : maxCutoffFrequency;

        //Выполняем код до тех пор пока не иссякнет время
        while (elapsed < duration)
        {
            _ = audioMixer.SetFloat("musicCutoffFrequency", Mathf.Lerp(startCutoffFrequency, targetCutoffFrequency, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        _ = audioMixer.SetFloat("musicCutoffFrequency", targetCutoffFrequency);
    }

    /// <summary>
    /// Плавное изменение громкости музыки
    /// </summary>
    /// <param name="duration">длительность изменения</param>
    /// <param name="on">убавить?</param>
    /// <returns></returns>
    private IEnumerator ChangeVolumeCoroutine(float duration, bool on)
    {
        //Инициализируем счётчиков прошедшего времени
        float elapsed = 0f;

        float startVolume = audioSource.volume;
        float targetVolume = on ? 0.5f : 1;
        //Выполняем код до тех пор пока не иссякнет время
        while (elapsed < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = targetVolume;
    }

    [ContextMenu("NextMusic")]
    public void NextMusic()
    {
        if (musicTrackID >= music.Length)
            ShuffleTheMusicTrack();
        if (audioSource.isPlaying)
            audioSource.Stop();
        audioSource.PlayOneShot(music[musicTrackID]);
        musicTrackID++;
    }

    /// <summary>
    /// Активация заглушки музыки (частота среза)
    /// </summary>
    /// <param name="on">заглушить?</param>
    public void ActivateCutoffFrequency(bool on)
    {
        _ = StartCoroutine(ActivateCutoffFrequencyCoroutine(2, on));
        _ = StartCoroutine(ChangeVolumeCoroutine(2, on));
    }

    #endregion Methods
}