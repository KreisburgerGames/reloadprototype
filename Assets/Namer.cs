using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Namer : MonoBehaviour
{
    public GameObject localPlayerRef;
    public string currentName;

    public void SetName(string newName)
    {
        currentName = newName;
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void MoveToGame()
    {
        transform.parent = SceneManager.GetSceneAt(0).GetRootGameObjects()[0].transform;
        transform.parent = null;
        print("moved namer");
    }

    public string CheckName()
    {
        if(currentName == "" || currentName == null)
        {
            return "Player " + PhotonNetwork.LocalPlayer.UserId[0];
        }
        return currentName;
    }
}
