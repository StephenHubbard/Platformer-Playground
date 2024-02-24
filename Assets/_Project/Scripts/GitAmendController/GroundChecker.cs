using UnityEngine;
using UnityEngine.Serialization;

namespace Platformer
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] private float groundDistance = 0.1f;
        [SerializeField] private LayerMask groundLayers;
        
        public bool IsGrounded {get; private set;}
        
        void Update()
        {
            IsGrounded = Physics.SphereCast(transform.position, groundDistance, Vector3.down, out _, groundDistance, groundLayers);
            
            // IsGrounded = Physics.CheckSphere(transform.position, groundDistance, groundLayers);
        }
       
    }
}