using UnityEngine;
using System.Collections;

/// <summary>
/// DEPRECATED. An ObjectTrigger that enables the hint. Now done through the player's action controller.
/// </summary>
public class HintTrigger : ObjectTrigger<HintHandler> {

    void OnTriggerEnter(Collider other)
    {
        target.SetHintEnabled(true);
    }

    void OnTriggerExit(Collider other)
    {
        target.SetHintEnabled(false);
    }

}
