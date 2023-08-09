using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ColorToggle : MonoBehaviour
{
    [SerializeField]
    private Color color;
    [SerializeField]
    private Network network;
    public void Awake() =>
        GetComponent<Toggle>().onValueChanged.AddListener
        ((bool on) =>
        {
            if (on)
                network.colorPlayer = color;
        });
}
