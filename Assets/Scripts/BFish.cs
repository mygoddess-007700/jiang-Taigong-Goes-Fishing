using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFish : MonoBehaviour
{
    public bool isProtected = false;
    public float protectedDuration = 1f;

    private float protectedDone;

    void Update()
    {
        if (isProtected)
        {
            if (Time.time > protectedDone)
            {
                isProtected = false;
            }
        }    
    }

    void OnMouseDown()
    {
        if (!isProtected)
        {
            protectedDone = Time.time + protectedDuration;
            isProtected = true;

            GameController gC = GameController.instance;
            if (gC.options[gC.number-1] == "B")
            {
                gC.ShootRight(2);
            }
            else
            {
                gC.ShootError();
            }   
        }  
    }
}
