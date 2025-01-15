using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject magPrefab;
    public Vector3 magEjectDirection;
    public float magEjectForce;
    public Transform magEjectPoint;
    public Transform magTakeoutPoint;
    public Transform magPickupPoint;
    public Transform magRevealPoint;
    public Transform magAlignPoint;
    public Transform magInsertPoint;
    public float magTakeoutSpeed;
    public float magTakeoutHeight;
    private bool isReloading = false;
    private ReloadState reloadState = ReloadState.Reloaded;
    public GameObject currentMag;
    public bool isAutomatic;
    public float range;
    public Transform head;
    public int magSize;
    private int currentAmmo;
    public int ammoReserve;
    public float fireRate;
    private float secondsPerShot;
    private bool isFiring = false;
    private Vector2 mouseInput;
    public float forceReleaseRate;
    public float magEjectTriggerForce;
    private Rigidbody magRb;
    public float gunErgonomics = 10f;
    private float magTakeoutInput;
    private float magTakeoutProgress;

    void Start()
    {
        currentAmmo = magSize;
        secondsPerShot = 60f / fireRate;
        magRb = currentMag.GetComponent<Rigidbody>();
        magRb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private IEnumerator AutomaticFire()
    {
        yield return new WaitForSeconds(secondsPerShot);
        if(isFiring)
        {
            Fire();
        }
    }

    private void Reload()
    {
        if(isReloading || ammoReserve <= 0 || currentAmmo == magSize)
        {
            return;
        }
        int ammoNeeded = magSize - currentAmmo;
        if(ammoNeeded <= ammoReserve)
        {
            ammoReserve -= ammoNeeded;
            currentAmmo = magSize;
        }
        else
        {
            currentAmmo += ammoReserve;
            ammoReserve = 0;
        }
        reloadState = ReloadState.EjectingMag;
    }

    private void Fire()
    {
        if(currentAmmo > 0)
        {
            currentAmmo--;
            RaycastHit hit;
            if(Physics.Raycast(head.position, head.forward, out hit, range))
            {
                if(isAutomatic)
                {
                    StartCoroutine(AutomaticFire());
                }
            }
        }
        else
        {
            Reload();
        }
    }

    void Update()
    {
        if(isReloading)
        {
            mouseInput += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
        else
        {
            mouseInput = Vector2.zero;
        }
        mouseInput = Vector2.Lerp(mouseInput, Vector2.zero, forceReleaseRate * Time.deltaTime);
        mouseInput.x = MathF.Round(mouseInput.x, 3);
        mouseInput.y = MathF.Round(mouseInput.y, 3);
        if(Input.GetMouseButtonDown(0) && !isReloading)
        {
            isFiring = true;
            Fire();
        }
        if(Input.GetMouseButtonUp(0))
        {
            isFiring = false;
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            Reload();
            isReloading = true;
        }
        if(Input.GetKeyUp(KeyCode.R))
        {
            isReloading = false;
        }
        if(isReloading)
        {
            switch(reloadState)
            { 
                case ReloadState.EjectingMag:
                    magTakeoutInput = 0f;
                    transform.position = Vector3.Lerp(transform.position, magEjectPoint.position, gunErgonomics * Time.deltaTime);
                    transform.rotation = Quaternion.Lerp(transform.rotation, magEjectPoint.rotation, gunErgonomics * Time.deltaTime);
                    if(mouseInput.x >= magEjectTriggerForce)
                    {
                        currentMag.transform.parent = null;
                        magRb.constraints = RigidbodyConstraints.None;
                        magRb.AddForce(magEjectDirection * magEjectForce, ForceMode.Impulse);
                        reloadState = ReloadState.GrabbingMag;
                    }
                break;

                case ReloadState.GrabbingMag:
                    if(mouseInput.y >= 3f)
                    {
                        currentMag = Instantiate(magPrefab, magPickupPoint.position, magPickupPoint.rotation);
                        magRb = currentMag.GetComponent<Rigidbody>();
                        magRb.constraints = RigidbodyConstraints.FreezeAll;
                        currentMag.transform.parent = transform;
                        reloadState = ReloadState.RevealingMag;
                    }
                break;

                case ReloadState.RevealingMag:
                    magTakeoutInput += Input.GetAxis("Mouse Y") * magTakeoutSpeed;
                    magTakeoutProgress = magTakeoutInput/magTakeoutHeight;
                    currentMag.transform.position = Vector3.Lerp(magPickupPoint.position, magTakeoutPoint.position, magTakeoutProgress);
                    if(currentMag.transform.position == magTakeoutPoint.position)
                    {
                        reloadState = ReloadState.AlignPrep;
                    }
                break;

                case ReloadState.AligningMag:

                break;

                case ReloadState.InsertingMag:
                
                break;
            }
        }
    }

    public enum ReloadState
    {
        InsertingMag,
        AligningMag,
        GrabbingMag,
        RevealingMag,
        AlignPrep,
        EjectingMag,
        EjectedMag,
        Reloaded
    }
}
