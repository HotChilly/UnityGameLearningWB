using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Sam Armstrong
public class NormalReload : MonoBehaviour
{
    public GameObject thisReloadObj;
    public int spawnTime = 20;
    public int respawnCount = 0;

    IEnumerator ReloadSelfRespawn()
    {
        if(respawnCount%2 == 0)
        {
            spawnTime += 5;
        }
        else
        {
            spawnTime -= 5;
        }

        yield return new WaitForSeconds(spawnTime);
        thisReloadObj.transform.Translate(0, 30, 0);
        respawnCount++;
        //thisReloadObj.SetActive(true);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player(Clone)")
        {
            other.gameObject.SendMessage("NormalReload");
            //ReloadSelfDestruct();
            //thisReloadObj.SetActive(false);
            thisReloadObj.transform.Translate(0, -30, 0);
            StartCoroutine(ReloadSelfRespawn());
        }
    }
}
