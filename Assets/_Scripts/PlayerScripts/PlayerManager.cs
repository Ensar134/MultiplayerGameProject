using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    public static List<Transform> ClientTransforms = new List<Transform>();

    void Update()
    {
        if (isServer)
        {
            ClientTransforms.Clear();
            foreach (var player in NetworkServer.connections)
            {
                if (player.Value != null && player.Value.identity != null)
                {
                    ClientTransforms.Add(player.Value.identity.transform);
                }
            }
        }
    }
}