using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RoomJoiner : MonoBehaviour
{
    public Button Host, Client;
    public GameObject JoinUI;

    void Start()
    {
        Host.onClick.AddListener(StartHost);
        Client.onClick.AddListener(StartClient);
    }

    void OnDestroy()
    {
        Host.onClick.RemoveListener(StartHost);
        Client.onClick.RemoveListener(StartClient);
    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        JoinUI.SetActive(false);
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        JoinUI.SetActive(false);
    }
}
