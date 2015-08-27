//Copyright 2015 Michele Pirovano

using UnityEngine;
using System.Collections;

namespace Cat.Debugging
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Add this to any GameObject to use a shortcut key to forcefully reset the level.
    /// </summary>
    public class ResetLevelDebug : MonoBehaviour
    {
        public KeyCode resetKey = KeyCode.R;

        [System.Diagnostics.Conditional("DEBUG")]
        void Update()
        {
            if (Input.GetKeyDown(resetKey)) Application.LoadLevel(Application.loadedLevel);
        }

    }
}
