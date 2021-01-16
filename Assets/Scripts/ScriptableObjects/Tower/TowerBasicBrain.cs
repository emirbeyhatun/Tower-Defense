

using UnityEngine;

[CreateAssetMenu(fileName = "TowerBrain", menuName = "ScriptableObjects/TowerBasicBrain", order = 1)]
public class TowerBasicBrain : TowerBrainBase
{
    public override Bullet ShootBullet(Vector2 startPos, Vector2 targetPos)
    {
        Bullet bullet = base.ShootBullet(startPos, targetPos);
        
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        
        if (rb)
        {
            Vector2 dir = targetPos - startPos;
            rb.AddForce((bulletSpeed * bulletSpeedMult) * dir.normalized);
        }
        

        return bullet;
    }
}
