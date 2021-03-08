using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    public static LoadScene instance;

    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private Image black;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(canvas);
        }
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneOpen;
    }

    private void Start()
    {
    }

    private void SceneOpen(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        StartCoroutine(IEnumSceneOpen());
    }

    private IEnumerator IEnumSceneOpen()
    {
        float time = 0;
        yield return new WaitForSeconds(1.0f);
        while (time < 5)
        {
            black.color = Color.Lerp(black.color, Color.clear, 1.5f * Time.deltaTime);

            time += Time.deltaTime;
            yield return null;
        }
    }

    private void SceneClose()
    {
        StopAllCoroutines();
        StartCoroutine(IEnumSceneClose());
    }

    private IEnumerator IEnumSceneClose()
    {
        float time = 0;
        while (time < 5)
        {
            black.color = Color.Lerp(black.color, Color.white, 1.5f * Time.deltaTime);

            time += Time.deltaTime;
            yield return null;
        }
    }

    public void LoadSceneAsync(string sceneName)
    {
        SceneClose();
        StartCoroutine(SceneLoad(sceneName));
    }

    private IEnumerator SceneLoad(string sceneName)
    {
        yield return new WaitForSeconds(2.5f);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {

            if (op.progress >= 0.9f)
            {


                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
