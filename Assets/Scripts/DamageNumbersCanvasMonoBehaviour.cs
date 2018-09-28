using UnityEngine;
using UnityEngine.UI;

public class DamageNumbersCanvasMonoBehaviour : MonoBehaviour
{
    private bool ToDestroy = false;
    private int CurrentNumbers = 0;

    public void Destroy()
    {
        if (CurrentNumbers == 0)
        {
            Destroy(gameObject);
        }
        ToDestroy = true;
    }

    public void Log(string message)
    {
        GameObject highlight = GameObject.Instantiate(Prefabs.DAMAGE_NUMBER_HIGHLIGHT_PREFAB, (Vector2) transform.position - new Vector2(-0.03125f, 0.03125f), Quaternion.identity);
        highlight.transform.SetParent(transform);
        highlight.GetComponent<Text>().text = message;
        highlight.GetComponent<DamageNumberMonoBehaviour>().AssignSignalFinish(MessageEnd);

        GameObject damageText = GameObject.Instantiate(Prefabs.DAMAGE_NUMBER_PREFAB, transform.position, Quaternion.identity);
        damageText.transform.SetParent(transform);
        damageText.GetComponent<Text>().text = message;
        damageText.GetComponent<DamageNumberMonoBehaviour>().AssignSignalFinish(MessageEnd);
        CurrentNumbers++;
    }

    public void Log(float damage)
    {
        float displayDamage = Mathf.Floor(damage * 10) / 10;
        Log(displayDamage.ToString());
    }

    public void MessageEnd()
    {
        CurrentNumbers--;
        if (CurrentNumbers == 0 && ToDestroy)
        {
            Destroy(gameObject);
        }
    }
}
