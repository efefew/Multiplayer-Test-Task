using TMPro;

using UnityEngine;

public class PlayerLabel : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private Player player;

    [SerializeField]
    private TMP_Text label;

    #endregion Fields

    #region Methods

    private void UpdateLabel() => label.text = $"{player.nickname} : {player.coins}";

    public void Build(Player player)
    {
        this.player = player;
        UpdateLabel();
        player.ChangeCoinsHandler += UpdateLabel;
    }
    private void OnDestroy()
    {
        if (player != null)
            player.ChangeCoinsHandler -= UpdateLabel;
    }
    #endregion Methods
}