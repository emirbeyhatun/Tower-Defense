using UnityEngine;
using System.Collections.Generic;

public abstract class TowerBrainBase : ScriptableObject
{
    public int damage = 10;
    public float shootIntervalSeconds = 0.3f;
    public float bulletSpeed = 10;
    public float range = 2;

    protected float bulletSpeedMult = 100;

    public Sprite defaultSprite;
    public GameObject bulletPrefab;
    public AnimationClip shootAnimClip;

    public Queue<Bullet> bulletPool = new Queue<Bullet>();


    public virtual Bullet ShootBullet(Vector2 startPos, Vector2 targetPos)
    {
        if (bulletPrefab == null)
            return null;

        Bullet bullet = GetBulletFromPool();

        if(bullet == null)
        {
            GameObject cloneBullet = Instantiate(bulletPrefab, startPos, Quaternion.identity, null);
            bullet = cloneBullet.GetComponent<Bullet>();
        }
        else
        {
            bullet.transform.position = startPos;
            bullet.gameObject.SetActive(true);
        }

        bullet.Damage = damage;

        return bullet;
    }

    public void AddBulletToPool(Bullet bullet)
    {
        if (bullet)
        {
            bulletPool.Enqueue(bullet);
        }
    }

    public Bullet GetBulletFromPool()
    {
        if(bulletPool.Count > 0)
        {
            return bulletPool.Dequeue();
        }
        else
        {
            return null;
        }
    }

}
