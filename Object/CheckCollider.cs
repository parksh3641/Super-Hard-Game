using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollider : MonoBehaviour
{
    public bool isCreate = true;


    private void Awake()
    {
        isCreate = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "SafeZone" || other.gameObject.tag == "Goal"
            || other.gameObject.tag == "Map")
        {
            isCreate = false;
        }
    }
}
