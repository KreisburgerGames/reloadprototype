using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    public AnimationCurve recoilCurve;
    public float recoilX;
    public Vector2 recoilY;
    public float recoilZ;
    private float recoilTimer;
    private bool isRecoiling;
    public GameObject head;
    public Vector3 currentRecoil;
    private Vector3 randomizedRecoil;
    private float recoilCurveLength;
    public float recoilRecoverySpeed;
    public float recoilSnappiness;
    private Vector3 targetRecoil;
    [Header("Weapon model recoil")]
    public GameObject weaponGameObject;
    public float WMRecoilTime;
    public float WMRecoilOffset;
    public float WMRecoilSpeed;
    public float WMRecoilRecoverySpeed;
    private float WMRecoilTimer;
    private float WMOriginalPos;
    private float WMCurrentRecoil;
    private bool WMRecoiling;

    private void Start()
    {
        recoilCurveLength = recoilCurve[recoilCurve.length - 1].time;
        WMOriginalPos = weaponGameObject.transform.localPosition.z;
        recoilX /= 10f;
        recoilY /= 10f;
        recoilZ /= 10f;
    }

    void Update()
    {
        if(isRecoiling)
        {
            recoilTimer += Time.deltaTime;
            recoilTimer = Mathf.Clamp(recoilTimer, 0, recoilCurveLength);
            targetRecoil += randomizedRecoil * recoilCurve.Evaluate(recoilTimer) * Time.deltaTime;
            currentRecoil = Vector3.Lerp(currentRecoil, targetRecoil, recoilSnappiness * Time.deltaTime);
            if (recoilTimer == recoilCurveLength) isRecoiling = false;
        }
        else
        {
            targetRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, recoilRecoverySpeed * Time.deltaTime);
            currentRecoil = targetRecoil;
        }
        
        if(WMRecoiling)
        {
            WMRecoilTimer += Time.deltaTime * WMRecoilSpeed;
            WMRecoilTimer = Mathf.Clamp(WMRecoilTimer, 0, WMRecoilTime);
            WMCurrentRecoil = Mathf.Lerp(WMOriginalPos, WMOriginalPos + WMRecoilOffset, WMRecoilTimer / WMRecoilTime);
            if(WMRecoilTimer == WMRecoilTime)
            {
                WMRecoiling = false;
                WMRecoilTimer = 0f;
            }
        }
        else
        {
            WMCurrentRecoil = Mathf.Lerp(WMCurrentRecoil, WMOriginalPos, recoilRecoverySpeed * Time.deltaTime);
        }
        weaponGameObject.transform.localPosition = new Vector3(weaponGameObject.transform.localPosition.x, weaponGameObject.transform.localPosition.y, WMCurrentRecoil);
    }

    public void RecoilStart()
    {
        randomizedRecoil = new Vector3(Random.Range(-recoilX, recoilX), Random.Range(recoilY.x, recoilY.y), Random.Range(-recoilZ, recoilZ));
        recoilTimer = 0f;
        isRecoiling = true;
        WMRecoiling = true;
    }
}
