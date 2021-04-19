using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Sam Armstrong
public class FallingPlatform : MonoBehaviour
{ 
    public Rigidbody thisFlyingSupport;
    public Transform startPos;
    public ParticleSystem spawnParticles;

    IEnumerator PlatformSelfRespawn()
    {


        yield return new WaitForSeconds(30);
        
        thisFlyingSupport.isKinematic = true;
        //thisReloadObj.SetActive(true);
        thisFlyingSupport.transform.SetPositionAndRotation(startPos.position, startPos.rotation);
        spawnParticles.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player(Clone)")
        {
            thisFlyingSupport.isKinematic = false;
            StartCoroutine(PlatformSelfRespawn());
        }
        if (other.name == "HotCannonBallPrefab(Clone)")
        {
            thisFlyingSupport.isKinematic = false;
            StartCoroutine(PlatformSelfRespawn());
        }
        if (other.name == "cannonBallPrefab(Clone)")
        {
            thisFlyingSupport.isKinematic = false;
            StartCoroutine(PlatformSelfRespawn());
        }
    }
}