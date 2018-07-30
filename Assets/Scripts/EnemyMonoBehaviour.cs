using UnityEngine;

public class EnemyMonoBehaviour : MonoBehaviour {

    private EnemyManager Manager;

    public void Destory()
    {
        GameManager.Singleton.GameObjectCount--;
        Destroy(gameObject);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Manager.Die();
    }

    public void AssignManager(EnemyManager manager)
    {
        Manager = manager;
    }

}
