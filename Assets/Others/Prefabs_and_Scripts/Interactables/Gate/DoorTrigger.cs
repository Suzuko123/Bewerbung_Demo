using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Dennis Deindörfer
// Only for debugging and early build testing

public class DoorTrigger : MonoBehaviour {

    [SerializeField] Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            anim.SetBool("Opened", true);
            Destroy(this);
        }
            
        
    }
}
