using UnityEngine;
using System.Collections;

/// <summary>
/// Loads the next scene
/// </summary>
public class Loader : MonoBehaviour {

    private static string nextScene = "";

    public static void SetNextScene(string s)
    {
        nextScene = s;
	}

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);
        Application.LoadLevel(nextScene);
    }
}
