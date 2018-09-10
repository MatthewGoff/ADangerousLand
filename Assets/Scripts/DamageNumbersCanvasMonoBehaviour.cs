using UnityEngine;
using UnityEngine.UI;

public class DamageNumbersCanvasMonoBehaviour : MonoBehaviour
{
    private bool ToDestroy = false;
    private int CurrentNumbers = 0;
    private EnemyManager Manager;

    public void Destroy()
    {
        if (CurrentNumbers == 0)
        {
            GameManager.Singleton.GameObjectCount--;
            Destroy(gameObject);
        }
        ToDestroy = true;
    }

    public void Log(string message)
    {
        GameObject damageText = GameObject.Instantiate(Prefabs.DAMAGE_NUMBER_PREFAB, transform.position, Quaternion.identity);
        GameManager.Singleton.GameObjectCount++;
        damageText.transform.SetParent(transform);
        damageText.GetComponent<Text>().text = message;
        damageText.GetComponent<DamageNumberMonoBehaviour>().AssignSignalFinish(MessageEnd);
        damageText.GetComponent<DamageNumberMonoBehaviour>().AssignManager(Manager);
        CurrentNumbers++;
    }

    public void MessageEnd()
    {
        CurrentNumbers--;
        if (CurrentNumbers == 0 && ToDestroy)
        {
            GameManager.Singleton.GameObjectCount--;
            Destroy(gameObject);
        }
    }

    public void AssignManager(EnemyManager manager)
    {
        Manager = manager;
    }
}
