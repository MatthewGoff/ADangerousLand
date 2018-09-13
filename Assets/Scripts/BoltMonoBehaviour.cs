using UnityEngine;
using System.Collections;

public class BoltMonoBehaviour : MonoBehaviour
{
    private BoltManager Manager;

    private float Speed = 10;
    private float Distance = 10;
    private Vector2 VelocityVector;

    void Start()
    {
        VelocityVector = Quaternion.Euler(0,0,transform.eulerAngles.z) * Vector2.right;
        StartCoroutine("Fly");
    }

    private IEnumerator Fly()
    {
        for (float i=0f; i<Distance; i += Speed * Time.deltaTime)
        {
            transform.position = (Vector2)transform.position + VelocityVector * Speed * Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Combatant")
        {
            ICombatantMonoBehaviour MonoBehaviour = other.gameObject.GetComponent<ICombatantMonoBehaviour>();
            if (MonoBehaviour != null)
            {
                Manager.ResolveCollision(MonoBehaviour.GetCombatantManager());
            }
        }
    }

    public void AssignManager(BoltManager manager)
    {
        Manager = manager;
    }
}
