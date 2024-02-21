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
            RaycastHit hit;
            IsGrounded = Physics.SphereCast(transform.position, groundDistance, Vector3.down, out hit, groundDistance, groundLayers);
            Debug.Log(hit.collider.gameObject.name);
        }
        
        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, groundDistance);
            
            if (IsGrounded) {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDistance);
            }
        }
    }
}