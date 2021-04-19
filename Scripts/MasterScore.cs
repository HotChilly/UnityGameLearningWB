using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
//Sam Armstrong
public class MasterScore : NetworkBehaviour
{
    [SyncVar]
    public int blueLives = 3;
    [SyncVar]
    public int redLives = 3;
    
}
