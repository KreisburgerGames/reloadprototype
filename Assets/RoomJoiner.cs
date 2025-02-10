using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomJoiner : MonoBehaviourPunCallbacks
{
    public Button Host, Client;
    public GameObject JoinUI;
    public GameObject serverPrefab;
    public GameObject serverPlayersPrefab;
    public GameObject playerItemPrefab;
    public GameObject localPlayer { get; private set; }
    private bool isHost;
    public GameObject connected;

    void Start()
    {
        Host.onClick.AddListener(StartHost);
        Client.onClick.AddListener(StartClient);

        PhotonNetwork.ConnectUsingSettings();
    }

    private void Awake() 
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        print("Connected");
        connected.SetActive(true);
    }

    void OnDestroy()
    {
        Host.onClick.RemoveListener(StartHost);
        Client.onClick.RemoveListener(StartClient);
    }

    private void StartHost()
    {
        PhotonNetwork.CreateRoom("test", null, null);
        isHost = true;
        JoinUI.SetActive(false);
    }

    private void StartClient()
    {
        PhotonNetwork.JoinRoom("test");
        isHost = false;
        JoinUI.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if(isHost)
        {
            GameObject _server = PhotonNetwork.Instantiate(serverPrefab.name, Vector3.zero, Quaternion.identity);
            _server.GetComponent<Server>().serverCam = GameObject.FindWithTag("Server Player List");
            _server.GetComponent<Server>().isOwner = true;
            PhotonNetwork.Instantiate(serverPlayersPrefab.name, Vector3.zero, Quaternion.identity);
        }

        StartCoroutine(InitializeName(FindFirstObjectByType<Namer>().CheckName()));
    }

    private IEnumerator InitializeName(string username)
    {
        while(GameObject.FindWithTag("Server Player List") == null) yield return null;

        GameObject _player = PhotonNetwork.Instantiate(playerItemPrefab.name, Vector3.zero, Quaternion.identity);
        FindAnyObjectByType<Namer>().currentName = FindAnyObjectByType<Namer>().CheckName();
        _player.GetComponent<PhotonView>().RPC("SetUsername", RpcTarget.AllBuffered, FindAnyObjectByType<Namer>().currentName);
        _player.GetComponent<PhotonView>().RPC("PushPlayerToUI", RpcTarget.AllBuffered);
        _player.GetComponent<PlayerItem>().LocalPlayerInit();
        localPlayer = _player;
    }
}
