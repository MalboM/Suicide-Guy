//Copyright 2015 Michele Pirovano
using UnityEngine;
using System.Collections;

/// <summary>
/// Abstract object trigger. Extend this for areas that will activate or otherwise interact with objects.
/// </summary>
public abstract class ObjectTrigger<T> : MonoBehaviour where T: MonoBehaviour
{
    protected T target;

    void Awake()
    {
        target = GetComponentInParent<T>();
    }

}
