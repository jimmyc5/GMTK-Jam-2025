using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;

    private void Awake()
    {
        // this is the code to ensure there's only one Transition Manager in a scene at a time (it's a Singleton)
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //This is the code that carries the TM over between scenes
        DontDestroyOnLoad(gameObject);

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
        }
    }

    public static string GetSceneNameByBuildIndex(int buildIndex)
    {
        return GetSceneNameFromScenePath(SceneUtility.GetScenePathByBuildIndex(buildIndex));
    }

    private static string GetSceneNameFromScenePath(string scenePath)
    {
        // Unity's asset paths always use '/' as a path separator
        var sceneNameStart = scenePath.LastIndexOf("/") + 1;
        var sceneNameEnd = scenePath.LastIndexOf(".");
        var sceneNameLength = sceneNameEnd - sceneNameStart;
        return scenePath.Substring(sceneNameStart, sceneNameLength);
    }

    public void LoadLevelSimple(string sceneName)
    {
        LoadLevel(sceneName);
    }

    public void LoadLevel(string sceneName, float transitionTime = 2f)
    {
        // screen wipe out
        TransitionScreenWipe wipe = FindObjectOfType<TransitionScreenWipe>();
        if (wipe)
        {
            wipe.WipeOut();
        }
        // load level
        StartCoroutine(LoadLevelFromName(sceneName, transitionTime));
    }
    public void RestartScene()
    {
        // screen wipe out
        TransitionScreenWipe wipe = FindObjectOfType<TransitionScreenWipe>();
        if (wipe)
        {
            wipe.WipeOut();
        }
        // restart level, make sure to reset the liquid (water/lava) levels to what they were when the scene started
        StartCoroutine(LoadLevelFromName(SceneManager.GetActiveScene().name, 0.5f));
    }

    IEnumerator LoadLevelFromName(string sceneName, float waitTimeBeforeTransition = 0f)
    {
        yield return new WaitForSeconds(waitTimeBeforeTransition);

        SceneManager.LoadScene(sceneName);
    }

    public void RestartSceneAfterDelay(float delayTime)
    {
        StartCoroutine(DelayedRestart(delayTime));
    }

    IEnumerator DelayedRestart(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        RestartScene();
    }
}
