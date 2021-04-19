using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
//Sam Armstrong
public class CannonBallScript : NetworkBehaviour
{
    public GameObject thisCannonBall;
    public GameObject playerShooter;
    public GameObject playerHit;
    public AudioClip impactSound;
    public AudioClip woodBreak;
    public AudioClip woodClunk;
    public AudioClip hollowHit;
    public int alreadyPlayed = 0;
    public string playerHitName;
    public bool team;

    public void Start()
    {
        team = playerShooter.GetComponent<PlayerMovementController>().teamChoice;
        //thisAS = GetComponent<AudioSource>();
        StartCoroutine(BallSelfDestruct());
    }
    IEnumerator BallSelfDestruct()
    {
        yield return new WaitForSeconds(10f);
        Destroy(thisCannonBall);
    }
    
    void OnCollisionEnter(Collision collision)
    {

            if (collision.collider.name == "Player(Clone)" && collision.collider.gameObject.GetComponent<PlayerMovementController>().teamChoice != team)
            {
                Debug.Log("PlayerCloneHit");
                playerHitName = collision.collider.gameObject.GetComponent<PlayerMovementController>().displayName;
                collision.collider.gameObject.SendMessage("cmdPlayerRigid", thisCannonBall.GetComponent<Rigidbody>().velocity);
                collision.collider.gameObject.SendMessage("DisplayWhoShot", playerShooter);
                collision.collider.gameObject.SendMessage("CmdChangeScore");
                collision.collider.gameObject.SendMessage("setAlive", false);

            }
            else if (collision.collider.name == "Visuals_Player" && collision.collider.gameObject.GetComponent<PlayerMovementController>().teamChoice != team)
            {
                Debug.Log("PlayerVisualHit");
                playerHitName = collision.collider.gameObject.GetComponent<PlayerMovementController>().displayName;
                collision.collider.gameObject.SendMessageUpwards("cmdPlayerRigid", thisCannonBall.GetComponent<Rigidbody>().velocity);
                collision.collider.gameObject.SendMessageUpwards("DisplayWhoShot", playerShooter);
                collision.collider.gameObject.SendMessageUpwards("CmdChangeScore");
                collision.collider.gameObject.SendMessageUpwards("setAlive", false);
            }

        if (collision.collider.tag == "Obstacle")
        {
            if (alreadyPlayed < 4)
            {
                    GetComponent<AudioSource>().spatialBlend = 1.0f;
                    GetComponent<AudioSource>().priority = 128;
                    GetComponent<AudioSource>().PlayOneShot(impactSound, .4f);
                    alreadyPlayed++;
            }
        }
        if (collision.collider.tag == "WoodPlank")
        {
            if (alreadyPlayed < 4)
            {
                GetComponent<AudioSource>().spatialBlend = 1.0f;
                GetComponent<AudioSource>().priority = 128;
                GetComponent<AudioSource>().PlayOneShot(woodClunk, 1f);
                
                alreadyPlayed++;
            }
        }
        if (collision.collider.tag == "BigStone")
        {
            if (alreadyPlayed < 4)
            {
                GetComponent<AudioSource>().spatialBlend = 1.0f;
                GetComponent<AudioSource>().priority = 128;
                GetComponent<AudioSource>().PlayOneShot(hollowHit, 1f);
                
                alreadyPlayed++;
            }
        }

    }
}
