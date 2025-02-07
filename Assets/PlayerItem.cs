using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerItem : MonoBehaviour
{
    public string username {get; private set;}
    public TMP_Text userText;
    public GameObject readyButton;
    public TMP_Text readyText;
    public bool isReady;
    private PhotonView network;
    public GameObject playerPrefab;
    private int connectionNumber;
    private bool spawned;
    public bool isLocalPlayer;
    private bool moved;
    private SpawnPointsHandler spawnPointsHandler;

    void Start()
    {
        network = GetComponent<PhotonView>();
        RefreshConnNumber();
    }

    void Update()
    {
        if(!isLocalPlayer) return;
        Scene gameScene = SceneManager.GetSceneAt(0);
        if(gameScene.name != "Game") return;
        GameObject[] roots = gameScene.GetRootGameObjects();

        if(!moved)
        {
            foreach(GameObject root in roots)
            {
                if(root.GetComponent<SpawnPointsHandler>() != null)
                {
                    spawnPointsHandler = root.GetComponent<SpawnPointsHandler>();
                    MoveToGame();
                    moved = true;
                    break;
                }
            }
        }
        if(moved && spawnPointsHandler != null && !spawned && spawnPointsHandler.isReady)
        {
            spawned = true;
            Vector3 spawnPos = spawnPointsHandler.spawnPoints[connectionNumber];
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
            LookAndMove playerRef = player.GetComponent<LookAndMove>();
            playerRef.IsLocalPlayer = true;
            FindFirstObjectByType<Namer>().localPlayerRef = player;
            print("e");
            //PhotonNetwork.Destroy(gameObject);
        }
    }

    private void RefreshConnNumber()
    {
        connectionNumber = 0;
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if(PhotonNetwork.LocalPlayer != player) connectionNumber++;
            else break;
        }
    }

    private void MoveToGame()
    {
        FindAnyObjectByType<Namer>().MoveToGame();
        transform.parent = SceneManager.GetSceneAt(0).GetRootGameObjects()[0].transform;
        transform.parent = null;
    }

    [PunRPC]
    public void SpawnPlayer(Vector3 spawnPoint)
    {
        if(!network.IsMine) return;
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity);
    }

    [PunRPC]
    public void SetUsername(string newUsername)
    {
        username = newUsername;
        userText.text = username;
    }

    [PunRPC]
    public void PushPlayerToUI()
    {
        transform.SetParent(GameObject.FindWithTag("Server Player List").transform, false);
    }

    [PunRPC]
    public void ToggleReadyRpc(bool newIsReady)
    {
        isReady = newIsReady;
        if(isReady)
        {
            readyText.text = "Ready";
        }
        else readyText.text = "Not Ready";
    }

    public void LocalPlayerInit()
    {
        readyButton.SetActive(true);
        readyButton.GetComponent<Button>().onClick.AddListener(ToggleReadyLocal);
        isLocalPlayer = true;
    }

    private void ToggleReadyLocal()
    {
        network.RPC("ToggleReadyRpc", RpcTarget.AllBuffered, !isReady);
    }
}
