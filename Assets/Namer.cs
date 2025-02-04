using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Namer : MonoBehaviour
{
    public GameObject localPlayerRef;
    public string currentName { get; private set;}

    public void SetName(string newName)
    {
        currentName = newName;
    }
}
