using UnityEngine;

public class EnemyMonoBehaviour : MonoBehaviour, ICombatantMonoBehaviour
{
    EnemyManager Manager;

    public void Destory()
    {
        GameManager.Singleton.GameObjectCount--;
        Destroy(gameObject);
    }

    public void AssignManager(EnemyManager manager)
    {
        Manager = manager;
    }

    public CombatantManager GetCombatantManager()
    {
        return Manager;
    }
}
