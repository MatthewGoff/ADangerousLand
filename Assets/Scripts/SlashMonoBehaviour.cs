using UnityEngine;
using System.Collections;

public class SlashMonoBehaviour : MonoBehaviour
{
    private SlashManager Manager;

    private SpriteRenderer SpriteRenderer;
    private float Lifetime;

    void Start()
    {
        Lifetime = 1f;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine("Fade");
    }

    private IEnumerator Fade()
    {
        for (float i=0; i<Lifetime; i+=Time.deltaTime)
        {
            if (i >= 3*Time.deltaTime)
            {
                Destroy(GetComponent<PolygonCollider2D>());
            }
            SpriteRenderer.color = new Color(255, 255, 255, 1 - i / Lifetime);
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

    public void AssignManager(SlashManager manager)
    {
        Manager = manager;
    }
}
