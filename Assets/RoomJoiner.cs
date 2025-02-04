using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RoomJoiner : MonoBehaviourPunCallbacks
{
    public Button Host, Client;
    public GameObject JoinUI;
    public GameObject playerPrefab;
    public Transform spawnPos;
    public GameObject localPlayer { get; private set; }

    void Start()
    {
        Host.onClick.AddListener(StartHost);
        Client.onClick.AddListener(StartClient);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    void OnDestroy()
    {
        Host.onClick.RemoveListener(StartHost);
        Client.onClick.RemoveListener(StartClient);
    }

    private void StartHost()
    {
        PhotonNetwork.JoinOrCreateRoom("test", null, null);
        JoinUI.SetActive(false);
    }

    private void StartClient()
    {
        PhotonNetwork.JoinRoom("test");
        JoinUI.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        GameObject _player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos.position, Quaternion.identity);
        localPlayer = _player;
        localPlayer.GetComponent<LookAndMove>().IsLocalPlayer = true;
    }
}
