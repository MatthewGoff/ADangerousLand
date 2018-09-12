using UnityEngine;

public class SlashMonoBehaviour : MonoBehaviour
{
    private SlashManager Manager;

    private SpriteRenderer SpriteRenderer;
    private float LifeTimeRemaining;
    private float LifeTimeTotal;

    void Start()
    {
        LifeTimeTotal = 1f;
        LifeTimeRemaining = LifeTimeTotal;
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Destroy(GetComponent<PolygonCollider2D>());
        LifeTimeRemaining -= Time.deltaTime;
        SpriteRenderer.color = new Color(255, 255, 255, LifeTimeRemaining / LifeTimeTotal);
        if (LifeTimeRemaining < 0)
        {
            Destroy(gameObject);
        }
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

    public void AssignManager(SlashManager manager)
    {
        Manager = manager;
    }
}
