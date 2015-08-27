using UnityEngine;
using System.Collections;

namespace Cat.Debugging
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Add this to any GameObject to use numbers on the keyboard to forcefully change the timescale.
    /// </summary>
    public class TimescaleDebug : MonoBehaviour
    {
        public int startTimeScale = 1;

        [System.Diagnostics.Conditional("DEBUG")]
        void Update()
        {
            if (Input.GetKeyDown("\\")) Time.timeScale = 0.25f;
            if (Input.GetKeyDown("1")) Time.timeScale = 1.0f;
            if (Input.GetKeyDown("2")) Time.timeScale = 2.0f;
            if (Input.GetKeyDown("3")) Time.timeScale = 4.0f;
            if (Input.GetKeyDown("4")) Time.timeScale = 8.0f;
            if (Input.GetKeyDown("5")) Time.timeScale = 16.0f;
        }
    }
}