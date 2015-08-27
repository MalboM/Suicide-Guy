//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Component that defines a Hand that can be shown.
/// This is controlled by the HandSwitcher
/// </summary>
public class Hand : MonoBehaviour {

    // parameters
    public HandType type;
    public MonoBehaviour targetComponentForActivation;  // This component will be enabled when the hand is activated
    
    // State
    float animationPeriod = 0.2f;    // Period of the switch animation
    Vector3 startLocalPos;

    void Awake()
    {
        startLocalPos = transform.localPosition;
    }

    /// <summary>
    /// This is called by a special effect
    /// </summary>
    public void Activate()
    {
        if (targetComponentForActivation != null) targetComponentForActivation.enabled = true;
    }

    /// <summary>
    /// Show the hand
    /// </summary>
	public void Show() 
    {
        //Debug.Log("SHOW " + this.name);
        StopAllCoroutines();
        StartCoroutine("ShowCO");
	}

    public void ShowLater()
    {
        Invoke("Show", animationPeriod);
    }

    /// <summary>
    /// Hide this hand
    /// </summary>
    public void Hide()
    {
        CancelInvoke("Show");
        //Debug.Log("HIDE " + this.name);
        StopAllCoroutines();
        StartCoroutine("HideCO");
    }

    #region Coroutines
    IEnumerator ShowCO()
    {
        float t = 0;
        Vector3 start = transform.localPosition;
        while (t < 1)
        {
            t += Time.deltaTime / animationPeriod;
            if (t >= 1)
                t = 1;

            transform.localPosition = Vector3.Lerp(start, startLocalPos, t);
            yield return null;
        }
    }

    IEnumerator HideCO()
    {
        float t = 0;
        Vector3 start = transform.localPosition;
        while (t < 1)
        {
            t += Time.deltaTime / animationPeriod;
            if (t >= 1)
                t = 1;

            transform.localPosition = Vector3.Lerp(start, startLocalPos + Vector3.down * 1, t);
            yield return null;
        }
    }
    #endregion
}
