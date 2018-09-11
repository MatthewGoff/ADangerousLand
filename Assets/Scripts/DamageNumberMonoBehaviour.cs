﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageNumberMonoBehaviour : MonoBehaviour {

    public delegate void SignalFinishDelegate();

    private SignalFinishDelegate SignalFinish;
    private Text Text;
    private float Alpha = 1f;
    private EnemyManager Manager;

	void Start () {
        Text = gameObject.GetComponent<Text>();
        StartCoroutine("Fade");
    }


    private IEnumerator Fade()
    {
        for (int i=0; i<Configuration.DAMAGE_NUMBERS_DURATION*60; i++)
        {
            float distance = Configuration.DAMAGE_NUMBERS_HEIGHT / (Configuration.DAMAGE_NUMBERS_DURATION * 60 * 32);
            transform.position = transform.position + new Vector3(0, distance, 0);
            Color color = Text.color;
            float fadeAlpha = 1 - ( i / (Configuration.DAMAGE_NUMBERS_DURATION * 60f));
            float fogAlpha = Manager.World.GetVisibilityLevel(transform.position);
            color.a = fadeAlpha * fogAlpha;
            Text.color = color;
            yield return new WaitForSeconds(1f/60);
        }
        SignalFinish();
        Destroy(gameObject);
        GameManager.Singleton.GameObjectCount--;
    }

    public void AssignSignalFinish(SignalFinishDelegate signalFinish)
    {
        SignalFinish = signalFinish;
    }

    public void AssignManager(EnemyManager manager)
    {
        Manager = manager;
    }
}
