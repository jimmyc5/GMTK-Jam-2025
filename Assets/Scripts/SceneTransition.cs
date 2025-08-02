using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public AudioClip entrySound;
    public GameObject particles;
    private string sceneToTransitionTo = "";
    public bool lastScene = false;
    public GameObject exitCamera;
    // Start is called before the first frame update
    void Start()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        if(!lastScene)
            sceneToTransitionTo = GetSceneNameByBuildIndex(currentIndex + 1);
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

    // collect the star if the player walks into the collider trigger
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(SoundManager.instance && entrySound)
            SoundManager.instance.PlaySoundClip(entrySound, transform.position, 0.25f);
            if (particles)
            {
                GameObject obj = GameObject.Instantiate(particles, transform.position, Quaternion.identity);
                ParticleSystem collectionPsComponent = obj.GetComponent<ParticleSystem>();
            }
            exitCamera.SetActive(true);
            TransitionManager.instance.LoadLevel(sceneToTransitionTo, 0.5f);
            gameObject.SetActive(false);
        }
    }
}
