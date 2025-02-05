using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform alertViewport;
    [SerializeField] private GameObject alertPrefab;
    private LookAndMove playerMovement;

    void Start()
    {
        playerMovement = GetComponent<LookAndMove>();
    }

    [PunRPC]
    public void CreateAlertRpc(string alertText)
    {
        foreach(Player player in FindObjectsOfType<Player>())
        {
            player.CreateAlertLocal(alertText);
        }
    }

    private void CreateAlertLocal(string alertText)
    {
        if(playerMovement == null || !playerMovement.IsLocalPlayer) return;
        Alert alert = Instantiate(alertPrefab, alertViewport).GetComponent<Alert>();
        alert.Init(alertText);
    }
}
