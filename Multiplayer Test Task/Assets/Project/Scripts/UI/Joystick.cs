using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{

    #region Properties

    public Vector2 inputVector { get; private set; }

    #endregion Properties

    #region Fields

    [SerializeField]
    private Image joystick, joystickBackground, joystickArea;

    private Vector2 joystickBackgroundStartPosition;
    //[SerializeField] private Color inActiveJoystickColor;
    //[SerializeField] private Color activeJoystickColor;

    //private bool joystickIsActive = false;

    #endregion Fields

    #region Methods

    private void Start() =>
        //ClickEffect();
        joystickBackgroundStartPosition = joystickBackground.rectTransform.anchoredPosition;

    //private void ClickEffect()
    //{
    //    if (!joystickIsActive)
    //    {
    //        joystick.color = activeJoystickColor;
    //        joystickIsActive = true;
    //    }
    //    else
    //    {
    //        joystick.color = inActiveJoystickColor;
    //        joystickIsActive = false;
    //    }
    //}

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground.rectTransform, eventData.position, null, out Vector2 joystickPosition))
        {
            joystickPosition.x *= 2 / joystickBackground.rectTransform.sizeDelta.x;
            joystickPosition.y *= 2 / joystickBackground.rectTransform.sizeDelta.y;

            inputVector = joystickPosition.magnitude > 1f ? joystickPosition.normalized : joystickPosition;

            joystick.rectTransform.anchoredPosition = new Vector2(inputVector.x * joystickBackground.rectTransform.sizeDelta.x / 2, inputVector.y * joystickBackground.rectTransform.sizeDelta.y / 2);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //ClickEffect();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickArea.rectTransform, eventData.position, null, out Vector2 joystickBackgroundPosition))
            joystickBackground.rectTransform.anchoredPosition = joystickBackgroundPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickBackground.rectTransform.anchoredPosition = joystickBackgroundStartPosition;

        //ClickEffect();

        inputVector = Vector2.zero;
        joystick.rectTransform.anchoredPosition = Vector2.zero;
    }

    #endregion Methods

}