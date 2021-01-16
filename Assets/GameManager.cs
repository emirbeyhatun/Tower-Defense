using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private static readonly int updateablesStartSize = 100;
    private UpdateableGameObject[] updateableObjects = new UpdateableGameObject[updateablesStartSize];
    private int updateableCurrentIndex = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        RefreshUpdateables();
    }

    private void RefreshUpdateables()
    {
        if (updateableObjects == null)
            return;

        int i = 0;
        while(i < updateableCurrentIndex)
        {
            UpdateableGameObject updateable = updateableObjects[i];
            if (updateable != null)//Enemy.
            {
                updateable.UpdateEveryFrame();
            }
            else
            {
                if (i < updateableCurrentIndex - 1)
                {
                    updateableObjects[i] = updateableObjects[updateableCurrentIndex - 1];
                    updateableCurrentIndex--;
                }
                else if (i == updateableCurrentIndex - 1)
                {
                    updateableCurrentIndex--;
                }
            }
            i++;
        }
    }
    public void AddUpdateableObject(UpdateableGameObject updateable)
    {
        if (updateable == null)
            return;

        if (updateableCurrentIndex >= updateableObjects.Length)
        {
            UpdateableGameObject[] newUpdArray = new UpdateableGameObject[updateableObjects.Length * 2];

            updateableObjects.CopyTo(newUpdArray, 0);
            updateableObjects = newUpdArray;
        }

        updateableObjects[updateableCurrentIndex] = updateable;
        updateableCurrentIndex++;
    }


}
