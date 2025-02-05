using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ThirdPersonWeaponManager : MonoBehaviour
{
    public Transform hipfire;
    public Transform reload;
    public GameObject mag;
    public GameObject thirdPersonWeapon;

    public void ChangePos(string pos)
    {
        switch(pos)
        {
            case "hip":
                thirdPersonWeapon.transform.position = hipfire.position;
                thirdPersonWeapon.transform.rotation = hipfire.rotation;
                break;
            case "reload":
                thirdPersonWeapon.transform.position = reload.position;
                thirdPersonWeapon.transform.rotation = reload.rotation;
                break;
        }
    }

    public void ChangeMagVisible(bool visible)
    {
        if(visible) mag.SetActive(true); else mag.SetActive(false);
    }
}
