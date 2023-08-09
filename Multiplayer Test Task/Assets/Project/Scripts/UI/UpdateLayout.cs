using System.Collections;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ContentSizeFitter))]
[DisallowMultipleComponent]
public class UpdateLayout : MonoBehaviour
{
    #region Fields

    private ContentSizeFitter sizeFitter;
    private GameObject gameObj;
    private RectTransform[] rectTransforms;
    private HorizontalOrVerticalLayoutGroup layoutGroup;
    private ScrollRect scroll;
    private float verticalScroll, horizontalScroll;

    #endregion Fields

    #region Methods

    private void Awake()
    {
        sizeFitter = GetComponent<ContentSizeFitter>();
        if (transform.parent.parent.parent.GetComponent<ScrollRect>())
            scroll = transform.parent.parent.parent.GetComponent<ScrollRect>();
        if (transform.parent.GetComponent<HorizontalOrVerticalLayoutGroup>())
            layoutGroup = transform.parent.GetComponent<HorizontalOrVerticalLayoutGroup>();
        gameObj = gameObject;
        LayoutGroup[] Layouts = gameObj.GetComponentsInChildren<LayoutGroup>();
        rectTransforms = new RectTransform[Layouts.Length];
        for (int i = 0; i < Layouts.Length; i++)
            rectTransforms[i] = Layouts[i].GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Canvas.ForceUpdateCanvases();
        _ = StartCoroutine(RestartSizeFitter());
    }

    public IEnumerator RestartSizeFitter()
    {
        if (scroll)
        {
            if (scroll.horizontalScrollbar) horizontalScroll = scroll.horizontalScrollbar.value;
            if (scroll.verticalScrollbar) verticalScroll = scroll.verticalScrollbar.value;
        }
        sizeFitter.enabled = false;

        if (layoutGroup)
        {
            layoutGroup.enabled = false;
            yield return new WaitForEndOfFrame();
            layoutGroup.enabled = true;
        }

        yield return new WaitForEndOfFrame();
        sizeFitter.enabled = true;
        for (int i = 0; i < rectTransforms.Length; i++)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransforms[i]);
        if (scroll)
        {
            if (scroll.horizontalScrollbar) scroll.horizontalScrollbar.value = horizontalScroll;
            if (scroll.verticalScrollbar) scroll.verticalScrollbar.value = verticalScroll;
        }
    }

    #endregion Methods
}