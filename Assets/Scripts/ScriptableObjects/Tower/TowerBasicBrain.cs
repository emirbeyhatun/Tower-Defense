using UnityEngine;

[CreateAssetMenu(fileName = "TowerBrain", menuName = "ScriptableObjects/TowerBasicBrain", order = 1)]
public class TowerBasicBrain : TowerBrainBase
{
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
            damagable.Damage(damage, targeter);
        }
    }
}
