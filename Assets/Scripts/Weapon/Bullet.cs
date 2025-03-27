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
        }
    }
}
