using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Alert : MonoBehaviour
{
    [SerializeField] private TMP_Text alertText;
    public float lifetime = 5f;
    private float lifetimeTimer = 0f;

    public void Init(string text)
    {
        alertText.text = text;
    }

    void Update()
    {
        lifetimeTimer += Time.deltaTime;

        if(lifetimeTimer >= lifetime) Destroy(gameObject);
    }
}
