using UnityEngine;

public class HealthBarMonoBehaviour : MonoBehaviour {

    private SpriteRenderer Renderer;

    public void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
    }

    public void Destroy()
    {
        GameManager.Singleton.GameObjectCount--;
        Destroy(gameObject);
    }

    public void ShowHealth(float health)
    {
        if (health == 1f)
        {
            Renderer.enabled = false;
        }
        else
        {
            Renderer.enabled = true;
            transform.localScale = new Vector3(health, 1, 1);
        }
    }
}