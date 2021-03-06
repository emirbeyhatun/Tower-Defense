﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : UpdateableGameObject
{
    public Transform spawnPosition;
    public Transform[] path;
    public Enemy enemyPrefab;
    public List<Enemy> activeEnemies = new List<Enemy>(100);
    public List<Enemy> disactiveEnemies = new List<Enemy>(100);

    public float enemySpawnInterval = 3;
    private float spawnTimerDecreaseAmount = 0.05f;
    private float enemySpawnTimer = 0;
    private int spawnedEnemyCount = 0;



    private void Start()
    {
        GameManager.instance.AddUpdateableObject(this);
    }
 
    public override void UpdateEveryFrame()
    {
        enemySpawnTimer += Time.deltaTime;
        enemySpawnTimer = Mathf.Min(enemySpawnInterval, enemySpawnTimer);

        if (enemySpawnTimer >= enemySpawnInterval)
        {
            SpawnEnemy();
            enemySpawnTimer = 0;

            enemySpawnInterval -= spawnTimerDecreaseAmount;
            enemySpawnInterval = Mathf.Max(enemySpawnInterval, 0.03f);

            if(enemySpawnInterval > 2)
            {
                spawnTimerDecreaseAmount = 0.04f;
            }
            else if (enemySpawnInterval > 1)
            {
                spawnTimerDecreaseAmount = 0.035f;
            }
            else if (enemySpawnInterval <= 1)
            {
                spawnTimerDecreaseAmount = 0.0015f;
            }
        }
    }

    public override void UpdateFixedFrame()
    {
        throw new System.NotImplementedException();
    }

    public int AmountOfEnemyLeft()
    {
        return Mathf.Max(GameManager.instance.KillCountLimit - spawnedEnemyCount, 0);
    }
    public void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPosition == null || AmountOfEnemyLeft() <= 0)
            return;

        Enemy spawnedEnemy = null;
        if (disactiveEnemies.Count <= 0)
        {
            spawnedEnemy = Instantiate(enemyPrefab, spawnPosition.transform.position, Quaternion.identity, null);
            activeEnemies.Add(spawnedEnemy);
            GameManager.instance.AddUpdateableObject(spawnedEnemy);
        }
        else
        {
            spawnedEnemy = disactiveEnemies[0];
            activeEnemies.Add(spawnedEnemy);
            disactiveEnemies.Remove(spawnedEnemy);
        }

        if (spawnedEnemy)
        {
            spawnedEnemy.PrepareEnemy(path, SwitchEnemyToDisabled);
            spawnedEnemy.transform.position = spawnPosition.transform.position;

            spawnedEnemyCount++;
        }
    }

    public void SwitchEnemyToDisabled(Enemy enemy)
    {
        if (enemy)
        {
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);

                if(disactiveEnemies.Contains(enemy) == false)
                {
                    disactiveEnemies.Add(enemy);
                }
            }
        }
    }
}
