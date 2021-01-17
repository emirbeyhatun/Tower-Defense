using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerBrain", menuName = "ScriptableObjects/TowerDestructorBrain", order = 2)]
public class TowerDestructorBrain : TowerBrainBase
{
    [Header("Destructor")]
    public float blastRadius = 3;
    public LayerMask blastLayer;
    public override Bullet ShootBullet(Vector2 startPos, Vector2 targetPos, Tower targeter, Animator animator)
    {
        Bullet bullet = base.ShootBullet(startPos, targetPos, targeter, animator);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb)
        {
            Vector2 dir = targetPos - startPos;
            rb.AddForce((bulletSpeed * bulletSpeedMult) * dir.normalized);
            bullet.transform.up = dir.normalized;
        }

        return bullet;
    }

    public override void OnBulletImpact(Vector2 startPos, IDamagable damagable, ITargeter targeter)
    {
        if (damagable != null)
        {
            Collider2D[] collidedObjects = Physics2D.OverlapCircleAll(startPos, blastRadius, blastLayer);

            for (int i = 0; i < collidedObjects.Length; i++)
            {
                if (collidedObjects[i])
                {
                    IDamagable collidedDamagable = collidedObjects[i].GetComponent<IDamagable>();
                    if (collidedDamagable != null)
                    {
                        collidedDamagable.Damage(damage, targeter);
                    }
                }
            }
        }
    }
}
