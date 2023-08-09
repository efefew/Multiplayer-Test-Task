using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextRepeater : MonoBehaviour
{
    #region Fields

    private Text repeater;
    public Text original;

    #endregion Fields

    #region Methods

    private void Awake() => repeater = GetComponent<Text>();

    private void OnEnable() => RepeatText();

    public void RepeatText() => repeater.text = original.text;

    public void InverseSetActive(bool on) => gameObject.SetActive(!on);

    #endregion Methods
}