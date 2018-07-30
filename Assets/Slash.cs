using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour {

    private SpriteRenderer SpriteRenderer;
    private float Lifetime;
    private float Alpha;

	void Start () {
        Alpha = 1f;
        Lifetime = 1f;
        SpriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Update () {
        Destroy(GetComponent<PolygonCollider2D>());
        Lifetime -= Time.deltaTime;
        SpriteRenderer.color = new Color(255, 255, 255, Lifetime/1f);
        if (Lifetime < 0)
        {
            Destroy(gameObject);
        }
    }
}
