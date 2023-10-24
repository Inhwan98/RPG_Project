using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;
using System.Text;
using TMPro;
public class LodingSceneController : MonoBehaviour
{
    static string nextScene;
    static int nextSceneindex;
    static Sprite loadingSprite;


    [SerializeField] Image progressBar;
    [SerializeField] Image backGroundImage;
    [SerializeField] TMP_Text loadingSceneName;


    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        loadingSprite = Resources.Load<Sprite>(nextScene);
        SceneManager.LoadScene("Loading");
    }

    public static void LoadScene(int sceneindex)
    {
        nextSceneindex = sceneindex;

        string path = SceneUtility.GetScenePathByBuildIndex(nextSceneindex);

        nextScene = path.Substring(0, path.Length - 6).Substring(path.LastIndexOf('/') + 1);
        loadingSprite = Resources.Load<Sprite>(nextScene);
        SceneManager.LoadScene("Loading");
    }


    private void Start()
    {
        //Set SceneName
        loadingSceneName.text = nextScene;
        //Set BG Srptie
        backGroundImage.sprite = loadingSprite;
        backGroundImage.color = Color.white;

        StartCoroutine(LoadSceneProcess());
    }

    
    private void ResetStaticFields()
    {
        nextScene = null;
        nextSceneindex = 0;
        loadingSprite = null;
    }


    IEnumerator LoadSceneProcess()
    {
        Resources.UnloadUnusedAssets();
        
        AsyncOperation op;

        if (nextScene == null)
        {
            op = SceneManager.LoadSceneAsync(nextSceneindex);
        }
        else
        {
            op = SceneManager.LoadSceneAsync(nextScene);
        }
       

        op.allowSceneActivation = false; // 씬 로딩이 끝나면 자동으로 씬을 불러올 것인가?

        float timer1 = 0.0f;
        float timer2 = 0.0f;
        while(!op.isDone)
        {
            yield return null;

            if(op.progress < 0.7f)
            {
                timer1 += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0, 0.7f, timer1);
            }
            else
            {
                timer2 += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.7f, 1f, timer2);

                if(progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    ResetStaticFields();
                    yield break;
                }
            }
        }

    }

}
