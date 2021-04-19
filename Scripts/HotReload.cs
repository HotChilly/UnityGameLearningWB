using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Sam Armstrong
public class HotReload : MonoBehaviour
{
    public GameObject thisReloadObj;
    public int spawnTime = 45;
    public int respawnCount = 0;

    IEnumerator ReloadSelfRespawn()
    {
        if (respawnCount % 2 == 0)
        {
            spawnTime += 15;
        }
        else
        {
            spawnTime -= 15; 
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
            other.gameObject.SendMessage("HotReload");
            thisReloadObj.transform.Translate(0, -30, 0);
            StartCoroutine(ReloadSelfRespawn());

        }
    }
}
