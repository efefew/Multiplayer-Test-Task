using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MultipleLanguageLabel : MonoBehaviour, IMultipleLanguage
{
    #region Fields

    private Text label;
    public string[] textOnTargetLanguage = new string[Language.countLanguage];

    #endregion Fields

    #region Methods

    private void Awake() => label = GetComponent<Text>();

    private void OnEnable()
    {
        OnChangeLanguage(Language.language);
        Language.eventChangeLanguage -= OnChangeLanguage;
        Language.eventChangeLanguage += OnChangeLanguage;
    }

    private void OnDisable() => Language.eventChangeLanguage -= OnChangeLanguage;

    public void OnChangeLanguage(Language.LanguageType language)
    {
        if (textOnTargetLanguage.Length <= (int)language)
            return;
        label.text = textOnTargetLanguage[(int)language];
    }

    #endregion Methods
}