using System.Collections;
using Photon.Pun;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Server : MonoBehaviour
{
    public GameObject serverCam;
    public bool isOwner;
    private ServerState serverState = ServerState.Lobby;
    private PhotonView photonView;
    public List<Transform> spawnPoints = new List<Transform>();

    static T[] ShuffleArray<T>(T[] array)
    {
        System.Random random = new System.Random();
        return array.OrderBy(x => random.Next()).ToArray();
    }

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if(!isOwner) return;

        switch(serverState)
        {
            case ServerState.Lobby:
                LobbyHandler();
            break;

            case ServerState.InGame:
                GameLoopServer();
            break;

            case ServerState.AfterGame:
                AfterGameHandler();
            break;
        }
    }

    [PunRPC]
    public void MovePlayerItems()
    {
        foreach(PlayerItem item in FindObjectsOfType<PlayerItem>())
        {
            item.gameObject.transform.SetParent(SceneManager.GetSceneAt(0).GetRootGameObjects()[0].transform, false);
            item.gameObject.transform.SetParent(null, false);
            DontDestroyOnLoad(item.gameObject);
        }
    }

    private void LobbyHandler()
    {
        PlayerItem[] players = FindObjectsOfType<PlayerItem>();
        if(players.Length > 1)
        {
            bool ready = true;
            for(int i = 0; i < players.Length; i++)
            {
                if(!players[i].isReady)
                {
                    ready = false;
                    break;
                }
            }
            if(ready)
            {
                photonView.RPC("MovePlayerItems", RpcTarget.All);
                PhotonNetwork.LoadLevel(1);
                StartCoroutine(ShuffleSpawnPoints());
                serverState = ServerState.InGame;
            }
        }
    }

    [PunRPC]
    public void MoveToGame()
    {
        transform.parent = SceneManager.GetSceneAt(0).GetRootGameObjects()[0].transform;
        transform.parent = null;
        print("server to game");

        if(PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SetSpawnPoints());
        }
    }

    private IEnumerator SetSpawnPoints()
    {
        while(FindFirstObjectByType<SpawnPointsHandler>() == null)
        {
            print("searching");
            yield return null;
        }

        foreach(Transform spawnPoint in FindFirstObjectByType<SpawnPointsHandler>().spawnPointsRaw) spawnPoints.Add(spawnPoint.transform);

        Transform[] points = ShuffleArray(spawnPoints.ToArray());
        spawnPoints = points.ToList();

        Vector3[] spawnPositions = new Vector3[spawnPoints.Count];
        for(int i = 0; i < spawnPoints.Count; i++)
        {
            spawnPositions[i] = spawnPoints[i].position;
        }

        print("Finalizing");
        FindFirstObjectByType<SpawnPointsHandler>().gameObject.GetComponent<PhotonView>().RPC("SetSpawnPoints", RpcTarget.All, spawnPositions);

        while(gameObject.scene.name != "Game" && FindObjectsOfType<PlayerItem>().Length != PhotonNetwork.PlayerList.Length) yield return null;

        foreach(PlayerItem player in FindObjectsOfType<PlayerItem>())
        {
            player.gameObject.GetComponent<PhotonView>().RPC("SpawnPlayer", RpcTarget.All);
        }

        serverState = ServerState.InGame;
        print("start");
    }

    private IEnumerator ShuffleSpawnPoints()
    {
        while(SceneManager.GetSceneAt(0).name != "Game") yield return null;

        photonView.RPC("MoveToGame", RpcTarget.All);
    }

    private void GameLoopServer()
    {
        bool isOver = false;
        foreach(Player player in FindObjectsOfType<Player>())
        {
            if(player.health <= 0)
            {
                isOver = true;
            }
        }
        if(isOver)
        {
            string winner = "";
            foreach(Player player in FindObjectsOfType<Player>())
            {
                if(player.health > 0)
                {
                    winner = player.gameObject.GetComponent<LookAndMove>().nametag.text;
                    break;
                }
            }
            print(winner);
            serverState = ServerState.AfterGame;
        }
    }

    private void AfterGameHandler()
    {

    }

    public enum ServerState
    {
        Lobby,
        InGame,
        AfterGame
    }
}
