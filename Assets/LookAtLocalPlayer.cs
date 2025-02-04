using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtLocalPlayer : MonoBehaviour
{
    private Namer namer;

    void Start()
    {
        namer = FindObjectOfType<Namer>();
    }

    void Update()
    {
        transform.LookAt(namer.localPlayerRef.transform);
    }
}
