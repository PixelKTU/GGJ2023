using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuilding : Building
{
    [SerializeField] float towerRange = 3;
    [SerializeField] float towerDamage = 1;
    [SerializeField] float shootingCooldown = 1;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPosition;


    bool isShooting = false;
    bool onCooldown = false;


    private void OnDrawGizmosSelected()
    {
        if (bulletSpawnPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(bulletSpawnPosition.position, towerRange);
        }
    }


    IEnumerator CooldownWait()
    {
        yield return new WaitForSeconds(shootingCooldown);
        onCooldown = false;
    }

    void Shoot(GameObject enemy)
    {
        enemy.GetComponent<Enemy>().TakeImaginaryDamage(towerDamage);
        BulletManager.Instance.SpawnBullet(bulletSpawnPosition.position, enemy, towerDamage);
    }

    protected override void OnRoundStarted()
    {
        isShooting = true;
    }

    protected override void OnRoundEnded()
    {
        isShooting = false;
    }

    void Update()
    {
        
        if (reachedByRoots && !onCooldown && isShooting)
        {
            GameObject enem = EnemySpawningSystem.Instance.GetNearestAliveEnemy(bulletSpawnPosition.position);
            if (enem != null)
            {
                Vector3 enemPosition = enem.transform.position;
                if ((enemPosition - bulletSpawnPosition.position).sqrMagnitude <= towerRange * towerRange)
                {

                    Shoot(enem);
                    onCooldown = true;
                    StartCoroutine(CooldownWait());
                }
            }
        }
    }
}
