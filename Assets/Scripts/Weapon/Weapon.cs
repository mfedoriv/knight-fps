using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapon
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        private PlayerInputHandler _inputHandler;
        
        // Shooting
        [SerializeField] private bool isShooting;
        [SerializeField] private bool isReadyToShoot;
        private bool allowReset = true;
        [SerializeField] private float shootingDelay = 2f;
        
        // Burst
        [SerializeField] private int bulletsPerBurst = 3;
        [FormerlySerializedAs("currentBurst")] [SerializeField] private int burstBulletsLeft;
        
        // Spread
        [SerializeField] private float spreadIntencity;
        
        // Bullet
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform bulletSpawn;
        [SerializeField] private float bulletVelocity = 30f;
        [SerializeField] private float bulletPrefabLifetime = 3f;
        
        public enum ShootingMode
        {
            Single,
            Auto,
            Burst
        }

        [SerializeField] private ShootingMode currentShootingMode;

        private void Awake()
        {
            isReadyToShoot = true;
            burstBulletsLeft = bulletsPerBurst;
        }

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
            switch (currentShootingMode)
            {
                case ShootingMode.Auto:
                    isShooting = _inputHandler.ShootHeld; // Работает при удержании
                    break;

                case ShootingMode.Single:
                    isShooting = _inputHandler.ShootPressed && isReadyToShoot; // Один раз за нажатие
                    break;

                case ShootingMode.Burst:
                    isShooting = _inputHandler.ShootPressed && isReadyToShoot; // Один раз за нажатие
                    break;
            }

            if (isReadyToShoot && isShooting)
            {
                burstBulletsLeft = bulletsPerBurst;
                HandleShooting();
            }
        }

        private void HandleShooting()
        {
            isReadyToShoot = false;

            Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation * Quaternion.Euler(-90, 0, 0));

            // Pointing bullet to face the shooting direction
            bullet.transform.forward = shootingDirection;
            
            bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
            StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifetime));
            
            // Checking if we are done shooting
            if (allowReset)
            {
                Invoke(nameof(ResetShot), shootingDelay);
                allowReset = false;
            }
            
            // Burst mode
            if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
            {
                burstBulletsLeft--;
                Invoke(nameof(HandleShooting), shootingDelay);
            }
        }

        private void ResetShot()
        {
            isReadyToShoot = true;
            allowReset = true;
        }

        private Vector3 CalculateDirectionAndSpread()
        {
            // Shooting from the middle of the screen to check where are we pointing at
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
            {
                // Hitting something
                targetPoint = hit.point;
            }
            else
            {
                // Shooting at the air
                targetPoint = ray.GetPoint(100);
            }

            Vector3 direction = targetPoint - bulletSpawn.position;
            float x = UnityEngine.Random.Range(-spreadIntencity, spreadIntencity);
            float y = UnityEngine.Random.Range(-spreadIntencity, spreadIntencity);
            
            return direction + new Vector3(x, y, 0);
        }

        private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(bullet);
        }
    }
}
