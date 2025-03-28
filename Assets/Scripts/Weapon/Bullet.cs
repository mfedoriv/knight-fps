using UnityEngine;

namespace Weapon
{
    public class Bullet : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Target"))
            {
                print("Hit " + other.gameObject.name);
                Destroy(gameObject);
            }
            
            if (other.gameObject.CompareTag("Wall"))
            {
                print("Hit a wall");
                Destroy(gameObject);
            }
        }
    }
}
