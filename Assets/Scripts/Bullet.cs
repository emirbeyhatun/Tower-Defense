using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage = 10;
    public int Damage { get => damage; set => damage = value; }

    private ITargeter targeter = null;
    public ITargeter Targeter { get => targeter; set => targeter = value; }

    public delegate void BulletPoolHandler(Bullet bullet);
    public BulletPoolHandler OnBulletDisposed;
    public delegate GameObject PooledExplotionReceiver(Vector3 pos);
    public PooledExplotionReceiver OnExplotionCreated;

    public delegate void BulletImpactHandler(Vector2 impactPos, IDamagable damagable, ITargeter targeter);
    public BulletImpactHandler OnImpact;

    [HideInInspector]public GameObject explotionPrefab;
    private bool enableCollision = false;
    
    public void PrepareBullet()
    {
        Invoke(nameof(DestroyIfNoCollision), 10);
        enableCollision = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if (damagable != null && enableCollision)
        {
            enableCollision = false;
            CancelInvoke(nameof(DestroyIfNoCollision));

            OnImpact?.Invoke(transform.position, damagable, targeter);
            OnBulletDisposed?.Invoke(this);
            OnExplotionCreated?.Invoke(transform.position);

            OnImpact = null;
            OnBulletDisposed = null;
            OnExplotionCreated = null;

            gameObject.SetActive(false);

        }
    }

    void DestroyIfNoCollision()
    {
        Destroy(gameObject);
    }
}
