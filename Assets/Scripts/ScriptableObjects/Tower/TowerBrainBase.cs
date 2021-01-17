using UnityEngine;
using System.Collections.Generic;

public abstract class TowerBrainBase : ScriptableObject
{
    public int ID = 0;
    public int damage = 10;
    public int price = 25;
    public int killMoney = 5;
    public int killLimitForUpgrade = 10;


    public float shootIntervalSeconds = 0.3f;
    public float bulletSpeed = 10;
    public float range = 2;

    protected float bulletSpeedMult = 100;

    public Sprite defaultSprite;
    public AnimatorOverrideController animatorController;

    public GameObject bulletPrefab;
    public GameObject bulletExplotionPrefab;

    public Queue<Bullet> bulletPool;
    public Queue<GameObject> explotionPool;

    public TowerBrainBase upgradedVersion;

    private void Awake()
    {
        bulletPool = new Queue<Bullet>();
        explotionPool = new Queue<GameObject>();
    }

    public virtual Bullet ShootBullet(Vector2 startPos, Vector2 targetPos, Tower targeter, Animator animator)
    {
        if (bulletPrefab == null)
            return null;

        Bullet bullet = GetPooledBullet();

        if(bullet == null)
        {
            GameObject cloneBullet = Instantiate(bulletPrefab, startPos, Quaternion.identity, null);
            bullet = cloneBullet.GetComponent<Bullet>();
        }
        else
        {
            bullet.transform.position = startPos;
        }

       
        bullet.Damage = damage;
        bullet.explotionPrefab = bulletExplotionPrefab;
        bullet.Targeter = targeter;
        bullet.OnBulletDisposed = PoolBullet;
        bullet.OnExplotionCreated = GetPooledExplotionElseCreate;
        bullet.OnImpact = OnBulletImpact;

        bullet.PrepareBullet();

        if (animator)
        {
            animator.SetTrigger("Shoot");
        }



        return bullet;
    }

    public abstract void OnBulletImpact(Vector2 impactPos, IDamagable damagable, ITargeter targeter);
    

    public void PoolBullet(Bullet bullet)
    {
        if (bullet)
        {
            bulletPool.Enqueue(bullet);
        }
    }

    public Bullet GetPooledBullet()
    {
        if(bulletPool.Count > 0)
        {
            Bullet bullet = bulletPool.Dequeue();
            bullet.gameObject.SetActive(true);
            return bullet;
        }
        else
        {
            return null;
        }
    }

    public void PoolExplotion(GameObject explotion)
    {
        if (explotion)
        {
            explotionPool.Enqueue(explotion);
        }
    }
   

    public GameObject GetPooledExplotionElseCreate(Vector3 pos)
    {
        GameObject explotion = null;
        if (explotionPool.Count > 0)
        {
            explotion = explotionPool.Dequeue();
            explotion.gameObject.SetActive(true);
        }
        else
        {
            explotion = Instantiate(bulletExplotionPrefab, pos, Quaternion.identity, null);
        }

        if (explotion)
        {
            explotion.transform.position = pos;
        }

        GameManager.instance.PoolExplotion(this, explotion);

        return explotion;

    }




}
