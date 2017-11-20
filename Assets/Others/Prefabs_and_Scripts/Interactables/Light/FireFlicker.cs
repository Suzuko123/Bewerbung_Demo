using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/*
 * Author: Dennis Deindörfer
 * Last edited: 26.10.2017
 * This is only an placeholder used till we got real flickering
 */

public class FireFlicker : MonoBehaviour {

    new Light light;
    float baseRange, baseIntensity;
    public float flickerIntensity = 0.25f, flickerRange = 0.5f;
    float sineCycle = 0;
    public float sineSpeed = 40f;

	void Start () {
        light = GetComponent<Light>();
        baseRange = light.range;
        baseIntensity = light.intensity;
	}

    void Update(){
            sineCycle = (sineCycle + Random.Range(0,sineSpeed)) % 360;
            light.intensity = baseIntensity + ((Mathf.Sin(sineCycle * Mathf.Deg2Rad) * (flickerIntensity / 4.0f)) + (flickerIntensity / 2.0f));
            light.range = baseRange + ((Mathf.Sin(sineCycle * Mathf.Deg2Rad) * (flickerRange / 2.0f)) + (flickerRange / 2.0f));
    }
}
