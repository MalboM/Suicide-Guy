using UnityEngine;
using System.Collections;

/// <summary>
/// Effect that sends the player to a scene when activated
/// </summary>
public class GoToSceneEffect : InteractiveEffect{

    // The name of the scene we'll go to
    public string sceneName;

    public override void ActivateLogic()
    {
        Loader.SetNextScene(sceneName);
        Application.LoadLevel("LoaderScene");
    }

    public override void DeactivateLogic()
    {
        // Nothing
    }
	
}
