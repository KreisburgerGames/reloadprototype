using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject magPrefab;
    public Vector3 magEjectDirection;
    public float magEjectForce;
    public Transform magEjectPoint;
    public Transform magTakeoutPoint;
    public Transform magPickupPoint;
    public Transform magInsertPoint;
    public Transform alignGoal;
    public Transform alignMovePoint;
    public float magTakeoutHeight;
    public bool isReloading = false;
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
    public Transform HipfirePos;
    public Transform ADSPos;
    private float magAlignInput;
    public float alignDist;
    private float insertInput;
    public float insertDistance;
    public float toHipfireTime;
    private float toHipfireTimer;
    public bool adjustingADS = false;
    private float ADSPercentage;
    public float ADSSpeed;
    public float ADSDist;
    private float ADSInput;
    public GameObject bolt;
    public Transform closedBolt;
    public Transform openBolt;
    public float boltPullDistance;
    public float boltSnapbackSpeed;
    private float boltPullInput;
    public Transform chamberingPoint;
    private bool pause = false;
    public float alignPrepDistance;
    private float alignPrepInput;
    private float grabbingInput;
    public float grabingSpeed;
    private bool isChambered = true;
    private Recoil recoil;

    void Start()
    {
        currentAmmo = magSize;
        secondsPerShot = 60f / fireRate;
        magRb = currentMag.GetComponent<Rigidbody>();
        magRb.constraints = RigidbodyConstraints.FreezeAll;
        recoil = GetComponent<Recoil>();
    }

    private IEnumerator AutomaticFire()
    {
        yield return new WaitForSeconds(secondsPerShot);
        print("check");
        if(isFiring)
        {
            Fire();
            print("fire");
        }
    }

    private void AddBullets()
    {
        int ammoNeeded = magSize - currentAmmo;
        print(ammoNeeded);
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
    }

    private void Reload()
    {
        if(isReloading || ammoReserve <= 0 || currentAmmo == magSize || reloadState != ReloadState.Reloaded)
        {
            return;
        }
        reloadState = ReloadState.EjectingMag;
    }

    private void Fire()
    {
        if(isChambered)
        {
            isChambered = false;
            if(currentAmmo > 0)
            {
                isChambered = true;
                currentAmmo--;
            }
            RaycastHit hit;
            if(Physics.Raycast(head.position, head.forward, out hit, range))
            {
                
            }
            recoil.RecoilStart();
            if(isAutomatic)
            {
                StartCoroutine(AutomaticFire());
            }
        }
    }

    private IEnumerator SwitchReloadState(ReloadState state, float delay)
    {
        pause = true;
        yield return new WaitForSeconds(delay);
        reloadState = state;
        pause = false;
    }

    void Update()
    {
        if(isReloading && Input.GetKey(KeyCode.E) || adjustingADS)
        {
            mouseInput += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
        else
        {
            mouseInput = Vector2.zero;
        }
        if(Input.GetKey(KeyCode.Q) && !isReloading)
        {
            adjustingADS = true;
        }
        else
        {
            adjustingADS = false;
        }
        if(adjustingADS)
        {
            ADSInput -= Input.GetAxis("Mouse X") * ADSSpeed;
            ADSInput = Mathf.Clamp(ADSInput, 0, ADSDist);
            ADSPercentage = ADSInput / ADSDist;
        }
        mouseInput = Vector2.Lerp(mouseInput, Vector2.zero, forceReleaseRate * Time.deltaTime);
        mouseInput.x = MathF.Round(mouseInput.x, 3);
        mouseInput.y = MathF.Round(mouseInput.y, 3);
        if(Input.GetMouseButtonDown(0) && (reloadState == ReloadState.Reloaded || reloadState == ReloadState.EjectingMag))
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
            if(isReloading)
            {
                isReloading = false;
            }
            else
            {
                Reload();
                isReloading = true;
            }
        }
        if(isReloading)
        {
            if(!pause)
            {
                if(reloadState != ReloadState.ToHipfire && reloadState != ReloadState.BoltOpening && reloadState != ReloadState.BoltClosing)
                {
                    transform.position = Vector3.Lerp(transform.position, magEjectPoint.position, gunErgonomics * Time.deltaTime);
                    transform.rotation = Quaternion.Lerp(transform.rotation, magEjectPoint.rotation, gunErgonomics * Time.deltaTime);
                }
                else if(reloadState == ReloadState.BoltOpening || reloadState == ReloadState.BoltClosing)
                {
                    transform.position = Vector3.Lerp(transform.position, chamberingPoint.position, gunErgonomics * Time.deltaTime);
                    transform.rotation = Quaternion.Lerp(transform.rotation, chamberingPoint.rotation, gunErgonomics * Time.deltaTime);
                }
                switch(reloadState)
                {
                    case ReloadState.EjectingMag:
                        magTakeoutInput = 0f;
                        magAlignInput = 0f;
                        magTakeoutProgress = 0f;
                        insertInput = 0f;
                        toHipfireTimer = 0f;
                        boltPullInput = 0f;
                        alignPrepInput = 0f;
                        grabbingInput = 0f;
                        
                        if(mouseInput.x >= magEjectTriggerForce)
                        {
                            currentMag.transform.parent = null;
                            magRb.constraints = RigidbodyConstraints.None;
                            magRb.AddForce(magEjectDirection * magEjectForce, ForceMode.Impulse);
                            currentAmmo = 0;
                            reloadState = ReloadState.GrabbingMag;
                        }
                    break;

                    case ReloadState.GrabbingMag:
                        if(Input.GetKey(KeyCode.G)) grabbingInput += Input.GetAxis("Mouse Y");
                        grabbingInput = Mathf.Lerp(grabbingInput, 0, forceReleaseRate * Time.deltaTime);
                        grabbingInput = MathF.Round(grabbingInput, 3);
                        if (grabbingInput >= grabingSpeed)
                        {
                            currentMag = Instantiate(magPrefab, magPickupPoint.position, magPickupPoint.rotation);
                            magRb = currentMag.GetComponent<Rigidbody>();
                            magRb.constraints = RigidbodyConstraints.FreezeAll;
                            currentMag.transform.parent = transform;
                            currentMag.transform.localScale = new Vector3(100f, 100f, 100f);
                            reloadState = ReloadState.RevealingMag;
                        }
                    break;

                    case ReloadState.RevealingMag:
                        if(Input.GetKey(KeyCode.Z))
                        {
                            magTakeoutInput += Input.GetAxis("Mouse Y");
                            print("e");
                        }
                        magTakeoutInput = Mathf.Clamp(magTakeoutInput, 0 , magTakeoutHeight);
                        magTakeoutProgress = magTakeoutInput/magTakeoutHeight;
                        currentMag.transform.position = Vector3.Lerp(magPickupPoint.position, magTakeoutPoint.position, magTakeoutProgress);
                        if(currentMag.transform.position == magTakeoutPoint.position)
                        {
                            reloadState = ReloadState.AlignPrep;
                        }
                    break;

                    case ReloadState.AlignPrep:
                        if(Input.GetKey(KeyCode.F))
                        {
                            alignPrepInput += Input.GetAxis("Mouse X");
                        }
                        alignPrepInput = Mathf.Clamp(alignPrepInput, 0, alignPrepDistance);
                        float alignPrepPercentage = alignPrepInput / alignPrepDistance;
                        currentMag.transform.position = Vector3.Lerp(magTakeoutPoint.position, alignGoal.position, alignPrepPercentage);
                        currentMag.transform.rotation = Quaternion.Lerp(magTakeoutPoint.rotation, alignGoal.rotation, alignPrepPercentage);
                        if(alignPrepPercentage == 1f)
                        {
                            reloadState = ReloadState.AligningMag;
                        }
                    break;

                    case ReloadState.AligningMag:
                        if(!Input.GetKey(KeyCode.F) && !Input.GetKey(KeyCode.X))
                        {
                            magAlignInput += Input.GetAxis("Mouse X");
                        }
                        magAlignInput = Mathf.Clamp(magAlignInput, 0, alignDist);
                        float magAlignPercentage = magAlignInput / alignDist;
                        currentMag.transform.position = Vector3.Lerp(alignGoal.position, alignMovePoint.position, magAlignPercentage);
                        if (magAlignPercentage == 1f) 
                        {
                            reloadState = ReloadState.InsertingMag;
                        }
                        break;
                    case ReloadState.InsertingMag:
                        if(Input.GetKey(KeyCode.X))
                        {
                            insertInput += Input.GetAxis("Mouse Y");
                        }
                        insertInput = Mathf.Clamp(insertInput, 0, insertDistance);
                        float insertPercentage = insertInput / insertDistance;
                        currentMag.transform.position = Vector3.Lerp(alignMovePoint.position, magInsertPoint.position, insertPercentage);
                        currentMag.transform.rotation = Quaternion.Lerp(currentMag.transform.rotation, magInsertPoint.rotation, insertPercentage);
                        if (insertPercentage == 1f)
                        {
                            if (isChambered) StartCoroutine(SwitchReloadState(ReloadState.ToHipfire, 0.25f));
                            else StartCoroutine(SwitchReloadState(ReloadState.BoltOpening, 0.25f));
                            AddBullets();
                        }
                    break;

                    case ReloadState.BoltOpening:
                        if(Input.GetMouseButton(0))
                        {
                            boltPullInput -= Input.GetAxis("Mouse Y");
                            boltPullInput = Mathf.Clamp(boltPullInput, 0, boltPullDistance);
                        }
                        else
                        {
                            boltPullInput = Mathf.MoveTowards(boltPullInput, 0, boltSnapbackSpeed * Time.deltaTime);
                        }
                        float boltPullPercentage = boltPullInput / boltPullDistance;
                        bolt.transform.position = Vector3.Lerp(closedBolt.position, openBolt.position, boltPullPercentage);
                        if(boltPullPercentage == 1f) reloadState = ReloadState.BoltClosing;
                        break;

                    case ReloadState.BoltClosing:
                        if(Input.GetMouseButton(0))
                        {
                            boltPullInput -= Input.GetAxis("Mouse Y");
                            boltPullInput = Mathf.Clamp(boltPullInput, 0, boltPullDistance);
                        }
                        else
                        {
                            boltPullInput = Mathf.MoveTowards(boltPullInput, 0, boltSnapbackSpeed * Time.deltaTime);
                        }
                        float boltClosePercentage = boltPullInput / boltPullDistance;
                        bolt.transform.position = Vector3.Lerp(closedBolt.position, openBolt.position, boltClosePercentage);
                        if(boltClosePercentage == 0f) 
                        {
                            isChambered = true;
                            currentAmmo --;
                            StartCoroutine(SwitchReloadState(ReloadState.ToHipfire, 0.1f));
                        }
                    break;
                }
            }
        }
        else if(!pause)
        {
            Vector3 targetADSPos = Vector3.Lerp(HipfirePos.position, ADSPos.position, ADSPercentage);
            Quaternion targetADSRot = Quaternion.Lerp(HipfirePos.rotation, ADSPos.rotation, ADSPercentage);
            transform.position = Vector3.Lerp(transform.position, targetADSPos, ADSSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetADSRot, ADSSpeed * Time.deltaTime);
        }
        if(reloadState == ReloadState.ToHipfire && !pause)
        {
            toHipfireTimer += Time.deltaTime;
            toHipfireTimer = Mathf.Clamp(toHipfireTimer, 0, toHipfireTime);
            float toHipPercentage = toHipfireTimer / toHipfireTime;
            transform.position = Vector3.Lerp(transform.position, HipfirePos.position, toHipPercentage);
            transform.rotation = Quaternion.Lerp(transform.rotation, HipfirePos.rotation, toHipPercentage);
            print(toHipPercentage);
            if(toHipPercentage == 1f)
            {
                isReloading = false;
                reloadState = ReloadState.Reloaded;
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
        BoltOpening,
        BoltClosing,
        ToHipfire,
        Reloaded
    }
}
