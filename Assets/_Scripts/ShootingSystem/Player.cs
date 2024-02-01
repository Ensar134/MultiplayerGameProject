using Mirror;
using UnityEngine;



    public class Player : NetworkBehaviour
    {
        [SerializeField] float _speed = 1f;
        [SerializeField] Transform _ShootPoint ;
        [SerializeField] int _playerNumber = 1;
    
    
        public Transform ShootPoint => _ShootPoint;
        public int PlayerNumber => _playerNumber;
    
        Animator _animator = null;


        private void Awake() => _animator = GetComponent<Animator>();

   
    }


