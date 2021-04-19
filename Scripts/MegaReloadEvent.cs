using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Sam Armstrong
public class MegaReloadEvent : MonoBehaviour
{
        public GameObject thisMegaReloadObj;
    public bool respawnStarted = false;
    //public bool isSpawned = true;

    IEnumerator MegaReloadSelfRespawn()
    {
        yield return new WaitForSeconds(30);
        thisMegaReloadObj.transform.Translate(0, 1000, 0);
        respawnStarted = false;
    }

    private void OnTriggerEnter(Collider other)
        {
            if (other.name == "Player(Clone)")
            {
            if (respawnStarted == false)
            {
                //other.gameObject.SendMessage("MegaReloadEvent");
                //thisReloadObj.transform.Translate(0, -30, 0);
                other.gameObject.GetComponent<PlayerMovementController>().WonAddMegaReloadEvent();
                
                thisMegaReloadObj.transform.Translate(0, -1000, 0);
                StartCoroutine(MegaReloadSelfRespawn());
                respawnStarted = true;
            }
           // other.gameObject.GetComponent<PlayerMovementController>().WonAddLivesEvent();
            //thisReloadObj.SetActive(false);
        }
        }
}
