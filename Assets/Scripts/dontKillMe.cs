using UnityEngine;
using System.Collections;

public class dontKillMe : MonoBehaviour
{

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}

