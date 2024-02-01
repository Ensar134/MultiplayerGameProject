using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.ShootingSystem
{
   public class Enemy : NetworkBehaviour
   {
      [SerializeField] private GameObject _hitPrefab;
      [SerializeField] private GameObject _explosionPrefab;
      [SerializeField] private int _health = 3;
      [SerializeField] private int _pointValue = 100;

      private AudioSource _audioSource=null;
      private int _currentHealth;

      private void OnEnable()
      {
         _audioSource = GetComponent<AudioSource>();
         _currentHealth= _health;
      
      }


      private void Update()
      {
         var Player =FindObjectOfType<Player>();
         //GetComponent<NavMeshAgent>().SetDestination(Player.transform.position);
      
      } 
   

      public void TakeDamage(Vector3 impactPoint, int playerNumber, int amount = 1)
      {
         if (!isServer)
         {
            return;
         }
         _currentHealth -= amount;
         TakeHitOnClient(impactPoint);
         if (_currentHealth <= 0)
         {
            DieOnClient(impactPoint, playerNumber);
         }
      
      }

      [ClientRpc]
      public void DieOnClient(Vector3 impactPoint, int playerNumber)
      {
         Instantiate(_explosionPrefab, impactPoint, transform.rotation);
         gameObject.SetActive(false);
         //Score system 
      }


      public void OnCollisionEnter(Collision collision)
      {
         var Player = collision.gameObject.GetComponent<Player>();
         if (Player != null)
         {
            SceneManager.LoadScene("GameOver");
         }
      }
   
      [ClientRpc]
      public void TakeHitOnClient(Vector3 impactPoint)
      {
         Instantiate(_hitPrefab, impactPoint, transform.rotation);
         if (_audioSource != null)
         {
            _audioSource.Play();
         }
      
      }
   }
}
