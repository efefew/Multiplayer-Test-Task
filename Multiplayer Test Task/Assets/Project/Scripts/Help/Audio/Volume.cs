using UnityEngine;
using UnityEngine.Audio;

public class Volume : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private AudioMixer audioMixer;

    public float minVolume = -20;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Конвертация еденицы измерения громкости
    /// </summary>
    /// <param name="volume">громкость</param>
    private void ConvertVolume(ref float volume)
    {
        volume = (volume * 2.5f) - 20;
        if (volume <= minVolume)
            volume = -80;
    }

    /// <summary>
    /// Функция для управления мастер-громкостью
    /// </summary>
    /// <param name="volume">громкость</param>
    public void SetMasterVolume(float volume)
    {
        Setting.mainVolume = volume;
        ConvertVolume(ref volume);
        _ = audioMixer.SetFloat("masterVolume", volume);
    }

    /// <summary>
    /// Функция управления громкостью звуковых эффектов
    /// </summary>
    /// <param name="volume">громкость</param>
    public void SetEffectsVolume(float volume)
    {
        Setting.effectsVolume = volume;
        ConvertVolume(ref volume);
        _ = audioMixer.SetFloat("effectsVolume", volume);
    }

    /// <summary>
    /// Функция управления громкостью фоновой музыки
    /// </summary>
    /// <param name="volume">громкость</param>
    public void SetMusicVolume(float volume)
    {
        Setting.musicVolume = volume;
        ConvertVolume(ref volume);
        _ = audioMixer.SetFloat("musicVolume", volume);
    }

    /// <summary>
    /// Функция управления громкостью звуков интерфейса
    /// </summary>
    /// <param name="volume">громкость</param>
    public void SetInterfaceVolume(float volume)
    {
        Setting.interfaceVolume = volume;
        ConvertVolume(ref volume);
        _ = audioMixer.SetFloat("interfaceVolume", volume);
    }

    public void SetSettingVolume()
    {
        SetMasterVolume(Setting.mainVolume);
        SetEffectsVolume(Setting.effectsVolume);
        SetMusicVolume(Setting.musicVolume);
        SetInterfaceVolume(Setting.interfaceVolume);
    }

    #endregion Methods
}