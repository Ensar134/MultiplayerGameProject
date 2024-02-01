using System.Collections.Generic;
using Mirror;
using UnityEngine;
using System.Collections;
using Mirror.Examples;


public class Bullet : NetworkBehaviour
    {
        private Gun _gun;
        public List<TrailRenderer> trails = new List<TrailRenderer>();

        public List<GameObject> BulletList;

      
        public void SetGun(GameObject gunObject)
        {
            _gun = gunObject.GetComponent<Gun>();
            
        }

        [ClientRpc]
        public void RpcSetGun(GameObject gunObject)
        {
            
            _gun = gunObject.GetComponent<Gun>();
        }
        

        private void OnCollisionEnter(Collision collision)
        {
            if (!isServer)
            {
                return;
            }
            
            DisableBullet();
           // Zombiye çarpışma kontrolü
        }

        public void DisableBullet()
        {
            NetworkServer.UnSpawn(gameObject);
            PrefabPool.singleton.Return(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Dummy"))
            {
                var damageable = other.gameObject.GetComponentInParent<IDamageableInterface>();
                if(damageable != null)
                {
                    
                        damageable.TakeDamage((int)_gun._gunData.damage);
                    
                   
                    
                }
                BulletList[_gun._gunData._bulletID].SetActive(false);
                DisableBullet();
            }
        }
        
        [ClientRpc]
        public void SetBullet()
        {
            
                foreach (var bullet in BulletList)
                {
                    bullet.SetActive(false);
                }
                BulletList[_gun._gunData._bulletID].SetActive(true);
            
            
            
        }   
        private float timer;
        public void Update()
        {
            if (!isServer) return;
          
            
        }
        private void TrailEnable(bool enable)
        {
            foreach (var trail in trails)
            {
                trail.enabled = enable;
            }
        }

        public void OnEnable()
        {
            if (!isServer) return;
            if (trails.Count == 0)
            {
                foreach (var trail in GetComponentsInChildren<TrailRenderer>())
                {
                    trails.Add(trail);
                }
            }
        } 

        [ClientRpc]
        public void Launch(Vector3 shootPointPosition, Quaternion shootPointRotation, Vector3 velocity)
        {
            transform.position = shootPointPosition;
            transform.rotation = shootPointRotation;
            TrailEnable(true);
            GetComponent<Rigidbody>().velocity =velocity;
            
        }
        public void InitializeBullet(GameObject gun, Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            SetGun(gun);
            RpcSetGun(gun);
            SetBullet();
            Launch(position, rotation, velocity);
        }
    }

