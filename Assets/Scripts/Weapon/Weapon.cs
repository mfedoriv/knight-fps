using System.Collections;
using Player;
using UnityEngine;

namespace Weapon
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform bulletSpawn;
        [SerializeField] private float bulletVelocity = 30f;
        [SerializeField] private float bulletPrefabLifetime = 3f;
        
        private PlayerInputHandler _inputHandler;
        
        private void Start()
        {
            _inputHandler = PlayerInputHandler.Instance;
            if (_inputHandler == null)
            {
                Debug.LogError("PlayerInputHandler instance is not found.");
            }
        }

        // Update is called once per frame
        void Update()
        {
            HandleShooting();
        }

        private void HandleShooting()
        {
            if (_inputHandler.ShootTriggered)
            {
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation * Quaternion.Euler(-90, 0, 0));
                bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);
                StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifetime));
            }
        }

        private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(bullet);
        }
    }
}
