using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage = 10;
    public int Damage { get => damage; set => damage = value; }

    private ITargeter targeter = null;
    public ITargeter Targeter { get => targeter; set => targeter = value; }

    public delegate void BulletPoolHandler(Bullet bullet);
    public BulletPoolHandler OnBulletDisposed;
    private WaitForSeconds secsToDestroy;
    private void Start()
    {
        secsToDestroy = new WaitForSeconds(10);
        StartCoroutine(DestroyIfNoCollision());
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if (damagable != null)
        {
            StopAllCoroutines();
            damagable.Damage(Damage, Targeter);
            gameObject.SetActive(false);
            OnBulletDisposed(this);
        }
    }

    IEnumerator DestroyIfNoCollision()
    {
        yield return secsToDestroy;
        Destroy(gameObject);
    }
}
