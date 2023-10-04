using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum Coordinate
{

}

public class LodingSceneController : MonoBehaviour
{
    static string nextScene;
    Coordinate coordinate;

    [SerializeField]
    Image progressBar;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }


    private void Start()
    {
        StartCoroutine(LoadSceneProcess());
        
    }

    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);

        op.allowSceneActivation = false; // 씬 로딩이 끝나면 자동으로 씬을 불러올 것인가?

        float timer = 0.0f;
        while(!op.isDone)
        {
            yield return null;

            if(op.progress < 0.7f)
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.7f, 1f, timer);

                if(progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }

    }

}
