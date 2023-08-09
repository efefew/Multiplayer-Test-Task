/// <summary>
/// у меня несколько языков
/// </summary>
public interface IMultipleLanguage
{
    #region Methods

    /// <summary>
    /// При изменении языка
    /// </summary>
    /// <param name="language">выбранный язык</param>
    void OnChangeLanguage(Language.LanguageType language);

    #endregion Methods
}

public static class Language
{
    #region Events

    public static event DelegateChangeLanguage eventChangeLanguage;

    #endregion Events

    #region Delegates

    public delegate void DelegateChangeLanguage(LanguageType language);

    #endregion Delegates

    #region Enums

    public enum LanguageType
    {
        russian,
        english
    }

    #endregion Enums

    #region Properties

    public static LanguageType language
    {
        get => languageValue;    // возвращаем значение свойства
        set
        {
            OnChangeLanguage(value);
            languageValue = value;   // устанавливаем новое значение свойства
        }
    }

    #endregion Properties

    #region Fields

    private static LanguageType languageValue;
    public const int countLanguage = 2;

    #endregion Fields

    #region Methods

    /// <summary>
    /// При изменении языка
    /// </summary>
    private static void OnChangeLanguage(LanguageType newLanguage)
    {
        if (newLanguage == language)
            return;
        eventChangeLanguage?.Invoke(newLanguage);
    }

    #endregion Methods
}