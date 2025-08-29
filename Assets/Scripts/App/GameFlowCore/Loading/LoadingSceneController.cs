using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    private float waitTime = 3f;

    private void Start()
    {
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
