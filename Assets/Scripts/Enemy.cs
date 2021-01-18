using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : UpdateableGameObject, IDamagable
{
    public float speed = 1f;
    public int hp = 100;
    public int baseHp = 100;

    public delegate void EnemyDeathHandler(Enemy enemy);
    private EnemyDeathHandler OnEnemyDeath;

    private Transform[] path;
    private int pathIndex = 0;
    private List<ITargeter> subscribedTargeters = new List<ITargeter>();

    public void Damage(int damage, ITargeter targeter)
    {
        if (!IsDead())
        {
            hp -= damage;
            hp = Math.Max(0, hp);

            if (IsDead())
            {
                GameManager.instance.IncreaseKillCount();
                if (targeter != null)
                {
                    targeter.OnTargetKilled();
                }
                WarnAndClearSubscribers();

                gameObject.SetActive(false);

                OnEnemyDeath?.Invoke(this);
            }
        }
    }

    public bool IsDead()
    {
        return hp <= 0;
    }
    public override void UpdateEveryFrame()
    {
        if (IsDead())
            return;

        if (path != null)
        {
            if(pathIndex < path.Length)
            {
                Vector2 dir = path[pathIndex].transform.position - transform.position;
                if (dir.magnitude > 0.3f)
                {
                    transform.Translate(dir.normalized * speed * Time.deltaTime);
                }
                else
                {
                    if(pathIndex == path.Length - 1)
                    {
                        UiManager.instance.OnLevelLost();
                    }
                    pathIndex++;
                }
            }
        }
    }

    public override void UpdateFixedFrame()
    {
        throw new System.NotImplementedException();
    }

    public void RemoveSubscribedTargeter(ITargeter targeter)
    {
        if (targeter != null)
        {
            subscribedTargeters.Remove(targeter);
        }
    }

    public void SubscribeTargeter(ITargeter targeter)
    {
        if (targeter != null)
        {
            subscribedTargeters.Add(targeter);
        }
    }

    public void WarnAndClearSubscribers()
    {
        for (int i = 0; i < subscribedTargeters.Count; i++)
        {
            subscribedTargeters[i].OnTargetUnavailable();
        }
        subscribedTargeters.Clear();
    }

    public void PrepareEnemy(Transform[] path, EnemyDeathHandler call)
    {
        if (path != null)
        {
            this.path = path;
            pathIndex = 0;
        }
        OnEnemyDeath = call;
        hp = baseHp;
        gameObject.SetActive(true);
    }
}

