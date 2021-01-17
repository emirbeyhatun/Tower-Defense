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
    private WaitForSeconds secsToDestroy;
    private bool enableCollision = false;
    private void Awake()
    {
        secsToDestroy = new WaitForSeconds(10);
    }
    public void PrepareBullet()
    {
        StartCoroutine(DestroyIfNoCollision());
        enableCollision = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if (damagable != null && enableCollision)
        {
            enableCollision = false;
            StopAllCoroutines();

            OnImpact?.Invoke(transform.position, damagable, targeter);
            OnBulletDisposed?.Invoke(this);
            OnExplotionCreated?.Invoke(transform.position);
            gameObject.SetActive(false);

        }
    }

    IEnumerator DestroyIfNoCollision()
    {
        yield return secsToDestroy;
        Destroy(gameObject);
    }
}
