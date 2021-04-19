using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Sam Armstrong
public class flyingCastleSupport : MonoBehaviour
{
    public Rigidbody thisFlyingSupport;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player(Clone)")
        {
            thisFlyingSupport.isKinematic = false;
        }
        if (other.name == "HotCannonBallPrefab(Clone)")
        {
            thisFlyingSupport.isKinematic = false;
        }
        if (other.name == "cannonBallPrefab(Clone)")
        {
            thisFlyingSupport.isKinematic = false;
        }
    }
}
