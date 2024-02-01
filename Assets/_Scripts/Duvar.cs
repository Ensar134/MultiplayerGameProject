using UnityEngine;
using Mirror;

public class Duvar : NetworkBehaviour
{
    private const float CheckInterval = 1.0f;
    private const float CheckDistanceThreshold = 5.0f; 
    public Odun[] odunlar;
    public Gun gun; private float _nextCheckTime;
    public bool isPlayerNear;

    private void Update()
    {
        if (!isServer)
        {
            return;
        }

        if (Time.time > _nextCheckTime)
        {
            CheckClosestPlayer();
            _nextCheckTime = Time.time + CheckInterval;
        }

        AllWallsDropped();
    }

    [Server]
    public void OdunlariResetle() // Odunları resetler esi posizyona gelir 
    {
        foreach (Odun odun in odunlar)
        {
            odun.OdunuResetle();
            gameObject.layer = 12;
            gameObject.GetComponent<BoxCollider>().isTrigger = false;
        }
    }
    
    [Server]
    public void Test()
    {
        foreach (Odun odun in odunlar)
        {
            if (!odun.IsDropped) 
            {
                odun.ServerOdunuDusur();
                break;
            }
        }
    }

    private void AllWallsDropped()
    {
        if (odunlar[0].IsDropped && odunlar[1].IsDropped && odunlar[2].IsDropped && odunlar[3].IsDropped)
        {
            gameObject.layer = 0;
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
        }
    }

    #region setWallScript
    private Transform FindClosestPlayer()
    {
        var closestDistance = float.MaxValue;
        Transform closestPlayer = null;

        foreach (var playerTransform in PlayerManager.ClientTransforms)
        {
            var distance = Vector3.Distance(transform.position, playerTransform.position);
            if (!(distance < closestDistance)) continue;
            closestDistance = distance;
            closestPlayer = playerTransform;
        }

        return closestPlayer;
    }
    private void CheckClosestPlayer()
    {
        var closestPlayer = FindClosestPlayer();

        if (closestPlayer == null) return;
        var distanceSqr = (transform.position - closestPlayer.position).sqrMagnitude;

        if (distanceSqr < CheckDistanceThreshold)
        {
            isPlayerNear = true;
            gun = closestPlayer.gameObject.GetComponent<Gun>();
            // Eğer Gun bileşeninin bir alt bileşeni veya bir fonksiyonu bu bileşene erişmek istiyorsa:
            if (gun != null)
            {
                gun._playerAction.duvar= this;
            }
        }
        else
        {
            isPlayerNear = false;
            gun = null;
        }

    }
    
    #endregion

}