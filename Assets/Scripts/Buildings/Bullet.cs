using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float collisionDistance = 0.1f;
    [SerializeField] float bulletSpeed = 1f;
    GameObject target;
    float damage;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, collisionDistance);
    }

    public void ResetData(GameObject target, float damage)
    {
        this.target = target;
        this.damage = damage;
    }

    private void Update()
    {
        float dist = (target.transform.position - transform.position).magnitude;
        float step = Mathf.Min(dist, bulletSpeed * Time.deltaTime);

        if (dist - step <= collisionDistance)
        {
            target.GetComponent<Enemy>().TakeDamage(damage);
            BulletManager.Instance.RemoveBullet(gameObject);
            return;
        }
        Vector3 dir = Vector3.zero;
        if (dist > 0)
        {
            dir = (target.transform.position - transform.position) / dist;
        }
        transform.position += dir * step;
    }

}
