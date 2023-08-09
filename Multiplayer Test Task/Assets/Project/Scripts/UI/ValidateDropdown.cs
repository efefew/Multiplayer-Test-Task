using System.Collections;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class ValidateDropdown : MonoBehaviour
{
    #region Fields

    private Dropdown dropdown;
    public int frames, minCountOptions;

    #endregion Fields

    #region Methods

    private void Awake() => dropdown = GetComponent<Dropdown>();

    private void OnEnable() => StartCoroutine(NextFrame(frames));

    private IEnumerator NextFrame(int countFrame)
    {
        for (int i = 0; i < countFrame; i++)
            yield return new WaitForFixedUpdate();
        dropdown.interactable = dropdown.options.Count >= minCountOptions;
    }

    #endregion Methods
}