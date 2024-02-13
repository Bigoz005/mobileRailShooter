using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{
    private static EnemySoundManager enemyManagerInstance;
    void Awake()
    {
        if (enemyManagerInstance == null)
        {
            enemyManagerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (enemyManagerInstance != this)
        {
            Destroy(gameObject);
        }
    }
}
