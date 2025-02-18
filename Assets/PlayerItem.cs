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
    public bool isLocalPlayer;
    private SpawnPointsHandler spawnPointsHandler;

    void Start()
    {
        network = GetComponent<PhotonView>();
        RefreshConnNumber();
    }

    [PunRPC]
    public void SpawnPlayer()
    {
        if(!isLocalPlayer) return;
        StartCoroutine(SyncedSpawn());
    }

    [PunRPC]
    public void DDOL()
    {
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator SyncedSpawn()
    {
        DontDestroyOnLoad(this.gameObject);
        while (SceneManager.GetSceneAt(0).name != "Game") yield return null;
        GameObject[] roots = SceneManager.GetSceneAt(0).GetRootGameObjects();
        
        bool ready = false;
        while(!ready)
        {
            foreach(GameObject root in roots)
            {
                if(root.GetComponentInChildren<SpawnPointsHandler>() != null)
                {
                    ready = true;
                    spawnPointsHandler = root.GetComponentInChildren<SpawnPointsHandler>();
                    MoveToGame();
                    print("Moving to game");
                    break;
                }
            }
            yield return null;
        }
        
        Vector3 spawnPos = spawnPointsHandler.spawnPoints[connectionNumber];
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
        LookAndMove playerRef = player.GetComponent<LookAndMove>();
        playerRef.IsLocalPlayer = true;
        FindFirstObjectByType<Namer>().localPlayerRef = player;
        print("e");
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
        print("moving namer");
        transform.parent = SceneManager.GetSceneAt(0).GetRootGameObjects()[0].transform;
        transform.parent = null;
        print("moved self");
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
