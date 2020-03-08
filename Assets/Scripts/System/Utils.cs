using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class Utils : MonoBehaviour
{

    public static T FindObjectOfTypeOrThrow<T>() where T : Object
    {
        var go = FindObjectOfType<T>();
        if (!go)
        {
            throw new Exception(
                $"Could not get the {typeof(T)}. Make sure you are using an object that uses {typeof(T)} " +
                "in your scene and it is enabled.");
        }

        return go;
    }

    public static T FindObjectOfTypeOrThrow<T>(string message) where T : Object
    {
        var go = FindObjectOfType<T>();
        if (!go)
        {
            throw new Exception(message);

        }

        return go;
    }
    
}