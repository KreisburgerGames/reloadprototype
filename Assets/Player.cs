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
    public ThirdPersonWeaponManager manager;

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

    [PunRPC]
    public void ChangeWeaponPosRpc(string pos)
    {
        manager.ChangePos(pos);
    }

    [PunRPC]
    public void ChangeMagVisibleRpc(bool visible)
    {
        manager.ChangeMagVisible(visible);
    }

    private void CreateAlertLocal(string alertText)
    {
        if(playerMovement == null || !playerMovement.IsLocalPlayer) return;
        Alert alert = Instantiate(alertPrefab, alertViewport).GetComponent<Alert>();
        alert.Init(alertText);
    }

    void Update()
    {
        if(!playerMovement.IsLocalPlayer) return;

        if(Input.GetKeyDown(KeyCode.M))
        {
            Cursor.visible = !Cursor.visible;

            if(Cursor.visible) Cursor.lockState = CursorLockMode.None; else Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
