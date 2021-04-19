using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Sam Armstrong
public class PlayerSpawnPointTT : MonoBehaviour
{
    private void Awake()
    {

        PlayerSpawnSystem.AddSpawnPointTeamTwo(transform);


    }

    private void OnDestroy() => PlayerSpawnSystem.RemoveSpawnPointTeamTwo(transform);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5);
    }
}
