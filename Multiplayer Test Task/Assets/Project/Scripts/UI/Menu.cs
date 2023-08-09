using System;

using UnityEngine;
using UnityEngine.UI;

public static class Setting
{
    #region Fields

    public const int countLanguage = 2;
    public static string sceneLoad;
    public static float mainVolume, effectsVolume, musicVolume, interfaceVolume;

    #endregion Fields
}

[DisallowMultipleComponent]
public class Menu : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private Dropdown languageDropdown;

    [SerializeField]
    private Slider mainVolumeSlider, effectsVolumeSlider, musicVolumeSlider, interfaceVolumeSlider;

    [SerializeField]
    private Volume volume;

    public SaveSetting setting = new();

    #endregion Fields

    #region Methods

    private void Start() => LoadSetting();

    public void OnChangeLanguage() => Language.language = (Language.LanguageType)languageDropdown.value;

    public void SaveSetting()
    {
        setting.language = languageDropdown.value;
        setting.mainVolume = mainVolumeSlider.value;
        setting.effectsVolume = effectsVolumeSlider.value;
        setting.musicVolume = musicVolumeSlider.value;
        setting.interfaceVolume = interfaceVolumeSlider.value;
        PlayerPrefs.SetString("setting", JsonUtility.ToJson(setting));
    }

    public void LoadSetting()
    {
        if (PlayerPrefs.HasKey("setting"))
            setting = JsonUtility.FromJson<SaveSetting>(PlayerPrefs.GetString("setting"));
        else
            ResetSetting();
        SetSetting();
    }

    public void ResetSetting()
    {
        setting.language = 0;
        setting.mainVolume = 10;
        setting.effectsVolume = 10;
        setting.musicVolume = 10;
        setting.interfaceVolume = 10;
    }

    /// <summary>
    /// установка сохранёных значений на интерфейс настроек
    /// </summary>
    public void SetSetting()
    {
        Language.language = (Language.LanguageType)setting.language;
        Setting.mainVolume = setting.mainVolume;
        Setting.effectsVolume = setting.effectsVolume;
        Setting.musicVolume = setting.musicVolume;
        Setting.interfaceVolume = setting.interfaceVolume;

        languageDropdown.value = setting.language;
        mainVolumeSlider.value = setting.mainVolume;
        effectsVolumeSlider.value = setting.effectsVolume;
        musicVolumeSlider.value = setting.musicVolume;
        interfaceVolumeSlider.value = setting.interfaceVolume;

        volume.SetSettingVolume();
    }

    /// <summary>
    /// Выход из игры
    /// </summary>
    public void Quit() => Application.Quit();

    #endregion Methods
}

[Serializable]
public class SaveSetting
{
    #region Fields

    public int language;
    public float mainVolume, effectsVolume, musicVolume, interfaceVolume;

    #endregion Fields
}