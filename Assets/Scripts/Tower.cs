using System.Collections.Generic;
using UnityEngine;

public class Tower  : UpdateableGameObject, ITargeter
{
    public TowerBrainBase brain;
    public Transform bulletSpawnPosition;
    public GameObject warningIndicator;
    public int killedEnemy = 0;
    //pool
    //ACtion delegate

    private float shootTimer = 0;
    private float shootInterval = 0;

    private float lookAtTimer = 0;
    private float lookAtInterval = 0.1f;

    private Enemy target;
    private List<Enemy> enemiesInsideRange;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circCollider;

    private void Start()
    {
        brain = GameManager.instance.TryToGetBrain(brain);
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        circCollider = GetComponent<CircleCollider2D>();
        RefreshWithBrain();
    }

    private void RefreshWithBrain()
    {
        if (brain == null)
            return;
        
        shootInterval = brain.shootIntervalSeconds;

        if(circCollider)
            circCollider.radius = brain.range;

        if(spriteRenderer)
            spriteRenderer.sprite = brain.defaultSprite;

        if (animator)
        {
            animator.runtimeAnimatorController = brain.animatorController;
            animator.Rebind();
        }
    }

    public override void UpdateEveryFrame()
    {
        if (brain == null)
            return;


        float deltaTime = Time.deltaTime;

        lookAtTimer += deltaTime;
        lookAtTimer = Mathf.Min(lookAtInterval, lookAtTimer);

        if (target && lookAtTimer >= lookAtInterval)
        {
            Vector2 lookDir = target.transform.position - transform.position;
            transform.up = lookDir.normalized;
            lookAtTimer = 0;
        }

        shootTimer += deltaTime;
        shootTimer = Mathf.Min(shootInterval, shootTimer);

        if(shootTimer >= shootInterval)
        {
            Shoot();
            shootTimer = 0;
        }
    }

    public override void UpdateFixedFrame()
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(target != null)
        {
            Enemy collidedEnemy = other.GetComponent<Enemy>();

            if (target.IsDead() == true)
            {
                SetTarget(collidedEnemy);
            }
            else
            {
                if (collidedEnemy)
                {
                    if (enemiesInsideRange == null)
                        enemiesInsideRange = new List<Enemy>();

                    enemiesInsideRange.Add(collidedEnemy);
                }
            }
        }
        else
        {
            Enemy collidedEnemy = other.GetComponent<Enemy>();
            SetTarget(collidedEnemy);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (target != null)
        {
            Enemy collidedEnemy = other.GetComponent<Enemy>();

            if (target == collidedEnemy)
            {
                collidedEnemy.RemoveSubscribedTargeter(this);
                StopTargettingCurrent();
            }
            else if (enemiesInsideRange != null)
            {
                if (enemiesInsideRange.Contains(collidedEnemy))
                {
                    enemiesInsideRange.Remove(collidedEnemy);
                }
            }
        }
    }

    public void SetBrain(TowerBrainBase newBrain)
    {
        brain = GameManager.instance.TryToGetBrain(newBrain);
        RefreshWithBrain();
    }
    private void SetTarget(Enemy enemy)
    {
        if (enemy != null)
        {
            target = enemy;
            enemy.SubscribeTargeter(this);
        }
        else
        {
            target = null;
            TryToFindAndSetEnemyInRange();
        }
    }

    public void TryToFindAndSetEnemyInRange()
    {
        if (enemiesInsideRange != null)
        {
            if (enemiesInsideRange.Count > 0)
            {
                SetTarget(enemiesInsideRange[0]);
                enemiesInsideRange.Remove(enemiesInsideRange[0]);
            }
        }
    }

    public void Shoot()
    {
        if (brain == null || bulletSpawnPosition == null || target == null || animator == null)
            return;

        Bullet spawnedBullet = brain.ShootBullet(bulletSpawnPosition.position, target.transform.position, this, animator);
    }

   
    public void StopTargettingCurrent()
    {
        SetTarget(null);
    }

    public void OnTargetUnavailable()
    {
        StopTargettingCurrent();
    }

    public void OnTargetKilled()
    {
        killedEnemy++;
        if (brain)
        {
            if (brain.upgradedVersion)
            {
                if (killedEnemy >= brain.upgradedVersion.killLimitForUpgrade)
                {
                    if (warningIndicator)
                    {
                        warningIndicator.gameObject.SetActive(true);
                    }
                }
            }

            GameManager.instance.EarnMoney(brain.killMoney);
        }
    }

    public void ResetWarning()
    {
        if (warningIndicator)
        {
            warningIndicator.gameObject.SetActive(false);
        }
    }

    public bool IsUpgradeAvailable()
    {
        if (brain)
        {
            if (killedEnemy >= brain.killLimitForUpgrade)
            {
                return true;
            }
        }

        return false;
    }
}
