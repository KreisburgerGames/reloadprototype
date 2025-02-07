using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPointsHandler : MonoBehaviour
{
    public Vector3[] spawnPoints;
    public bool isReady = false;
    public List<Transform> spawnPointsRaw = new List<Transform>();

    [PunRPC]
    public void SetSpawnPoints(Vector3[] newSpawnPoints)
    {
        print("set");
        spawnPoints = newSpawnPoints;
        foreach(Vector3 pos in spawnPoints) print(pos);
        isReady = true;
    }
}
