using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Sam Armstrong
public class MoreLives : MonoBehaviour
{
    public GameObject thisMoveLivesObj;
    public GameObject parentOfLifeOrb;
    public int spawnTime = 45;

    IEnumerator AddLivesSelfRespawn()
    {

        yield return new WaitForSeconds(spawnTime);
        parentOfLifeOrb.transform.Translate(0, 30, 300);

        //thisReloadObj.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player(Clone)")
        {
            other.gameObject.GetComponent<PlayerMovementController>().WonAddLivesEvent();
            parentOfLifeOrb.transform.Translate(0, -30, -300);
            StartCoroutine(AddLivesSelfRespawn());
        }
    }
}
