using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance;
    [SerializeField] GameObject bulletPrefab;

    IObjectPool<GameObject> bulletPool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        bulletPool = new ObjectPool<GameObject>(CreateBulletForPool,GetBulletFromPool,PutBulletToPool, DestroyBulletFromPool);
    }


    public void SpawnBullet(Vector3 spawnPosition, GameObject target, float damage)
    {
        GameObject obj = bulletPool.Get();
        obj.transform.position = spawnPosition;
        obj.GetComponent<Bullet>().ResetData(target, damage);
    }

    public void RemoveBullet(GameObject bullet)
    {
        bulletPool.Release(bullet);
    }


    GameObject CreateBulletForPool()
    {
        return Instantiate(bulletPrefab);
    }
    void GetBulletFromPool(GameObject obj)
    {
        obj.SetActive(true);
    }
    void PutBulletToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
    void DestroyBulletFromPool(GameObject obj)
    {
        Destroy(obj);
    }
}
