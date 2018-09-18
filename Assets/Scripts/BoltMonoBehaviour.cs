using UnityEngine;
using System.Collections;

public class BoltMonoBehaviour : MonoBehaviour
{
    private SpriteRenderer Renderer;
    private BoltManager Manager;
    private float Speed = 10;
    private float Distance = 30;
    private Vector2 VelocityVector;

    void Start()
    {
        Renderer = GetComponent<SpriteRenderer>();
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

    public void Update()
    {
        Renderer.color = new Color(1, 1, 1, GameManager.Singleton.World.GetVisibilityLevel(transform.position));
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
