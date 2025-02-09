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
    }

    private int GetPlayerNumber()
    {
        int num = 1;
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if(player == PhotonNetwork.LocalPlayer)
            {
                break;
            }
            num++;
        }

        return num;
    }

    public string CheckName()
    {
        if(currentName == "" || currentName == null)
        {
            return "Player " + GetPlayerNumber();
        }
        return currentName;
    }
}
