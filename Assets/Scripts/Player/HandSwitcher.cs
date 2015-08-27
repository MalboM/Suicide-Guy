//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Type of the hand to show: add more as needed!
/// </summary>
public enum HandType
{
    NONE,
    HINT_PICKUP,
    BOTH_RAISED,
    TEST_1,
    TEST_2,
	CAKE_1,
}

/// <summary>
/// Controls the switching of hands.
/// Will check for all Hand components in the children and add them to the available hands.
/// </summary>
public class HandSwitcher : MonoBehaviour {

    // Inner state
    private Dictionary<HandType, Hand> handsDictionary;
    private Hand currentHand;

    /// <summary>
    /// The hand that is currently used
    /// </summary>
    public Hand CurrentHand
    {
        get { return this.currentHand; }
    } 

    /// <summary>
    /// Initialisation
    /// </summary>
    void Awake()
    {
        handsDictionary = new Dictionary<HandType, Hand>();
        var hands = this.GetComponentsInChildren<Hand>();
        foreach (var h in hands) handsDictionary[h.type] = h;
        ShowHand(HandType.NONE, initialisation:true);
    }
	
    /// <summary>
    /// Changes hands at will
    /// </summary>
    /// <param name="handType">Hand type to switch to</param>
    /// <param name="initialisation">Is this called during initialisation? (leave this to false during the game)</param>
    public void ShowHand (HandType handType, bool initialisation = false) {
        if (!initialisation && currentHand == null && handType == HandType.NONE) return;
        if (currentHand != null && currentHand.type == handType) return;

        foreach (var h in handsDictionary.Values) h.Hide();

        if (handType != HandType.NONE)
        {
            if (!handsDictionary.ContainsKey(handType))
            {
                Debug.LogError("You did not create a hand of type " + handType + "!");
                return;
            }

            if (currentHand == null || currentHand.type == HandType.NONE)
                handsDictionary[handType].Show();
            else
                handsDictionary[handType].ShowLater();
            this.currentHand = handsDictionary[handType];
        }
        else
        {
            this.currentHand = null;
        }
	}
	
}
