using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance = null;

    Dictionary<int, IdleState> enemySet;
    Dictionary<int, float> enemyHealth;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        enemySet = new Dictionary<int, IdleState>();
    }

    public int addEnemy(IdleState script)
    {
        int id = script.id;
        while (enemySet.Keys.Contains(id))
        {
            id = script.regenerateId();
        }

        enemySet.Add(id, script);
        enemyHealth.Add(id, 100);
        return id;
    }

    private void LateUpdate()
    {
        if (enemySet.Count == 0)
        {
            //YOU WIN
        }

        foreach (int key in enemyHealth.Keys)
        {
            if (enemyHealth[key] <= 0) {
                enemySet[key].setDeath();
            }
            enemySet.Remove(key);
            enemyHealth.Remove(key);
        }
    }
}
