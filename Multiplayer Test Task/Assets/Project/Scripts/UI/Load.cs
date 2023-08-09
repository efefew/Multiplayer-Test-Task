using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Load : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// значение прогресс-бара
    /// </summary>
    [SerializeField]
    private Slider progressBar;

    [SerializeField]
    private TMP_Text label;

    public static string sceneName;
    public bool loadSceneOnAwake;

    #endregion Fields

    #region Methods

    private void Start()
    {
        if (loadSceneOnAwake)
            LoadScene();
    }

    private IEnumerator ILoadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            if (progressBar)
                progressBar.value = operation.progress;
            if (label)
                label.text = $"{operation.progress * 100}%";
            yield return null;
        }
    }

    public void SetStaticLoadScene(string sceneName) => Load.sceneName = sceneName;

    /// <summary>
    /// Загрузка сцены
    /// </summary>
    public void LoadScene(string sceneName) => StartCoroutine(ILoadScene(sceneName));

    /// <summary>
    /// Загрузка сцены, имя которой статическое
    /// </summary>
    public void LoadScene() => StartCoroutine(ILoadScene(sceneName));

    #endregion Methods
}