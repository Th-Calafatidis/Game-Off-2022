// /* --------------------
// -----------------------
// Creation date: 06/11/2022
// Author: Alex
// Description: Collection of general purpose utility methods.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utility
{
    /// <summary>
    /// Loads the next scene in the build index.
    /// </summary>
    public static void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Restarts the current scene.
    /// </summary>
    public static void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Delays the execution of a method by a given amount of time. The reason for using this over built in Invoke()
    /// is that this allows for passing methods directly, not as a string, and it accepts lambda expressions as well.
    /// </summary>
    public static void Invoke(this MonoBehaviour mono, System.Action action, float delay)
    {
        mono.StartCoroutine(InvokeCoroutine(action, delay));
    }

    /// <summary>
    /// Internal coroutine for the Invoke method. Waits the specified amount of time before
    /// running the action.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    private static IEnumerator InvokeCoroutine(System.Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
}
