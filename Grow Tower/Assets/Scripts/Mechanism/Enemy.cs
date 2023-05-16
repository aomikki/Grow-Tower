using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{

    public Power power;
    
    public float Strength { get { return power.Strength; } private set { power.Strength = value; } }

    TextMeshPro powerTxt = null;

    bool isInit = false;

    public void Init()
    {
        if (isInit) return;

        powerTxt = gameObject.GetComponentInChildren<TextMeshPro>();
        power = new Power();
        power.OnVariableChange += TextUpdate;

        isInit = true;
    }

    void TextUpdate()
    {
        powerTxt.text = power.Strength_str;
    }

    void Update()
    {
        
    }
}
