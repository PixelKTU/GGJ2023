using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuilding : Building
{
    [SerializeField] float towerRange = 3;
    [SerializeField] float towerDamage = 1;
    [SerializeField] float shootingCooldown = 1;


    bool isShooting = false;
    bool onCooldown = false;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, towerRange);
    }


    IEnumerator CooldownWait()
    {
        yield return new WaitForSeconds(shootingCooldown);
        onCooldown = false;
    }

    void Shoot(GameObject enemy)
    {

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
            GameObject enem = EnemySpawningSystem.Instance.GetNearestEnemy(transform.position);
            Vector3 enemPosition = enem.transform.position;
            if ((enemPosition-transform.position).sqrMagnitude <= towerRange * towerRange)
            {
                Shoot(enem);
                onCooldown = true;
                StartCoroutine(CooldownWait());
            }
        }
    }
}
