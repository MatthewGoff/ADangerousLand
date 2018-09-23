using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageNumberMonoBehaviour : MonoBehaviour
{

    public delegate void SignalFinishDelegate();

    private SignalFinishDelegate SignalFinish;
    private Text Text;

    void Start()
    {
        Text = gameObject.GetComponent<Text>();
        StartCoroutine("Fade");
    }


    private IEnumerator Fade()
    {
        for (int i = 0; i < Configuration.DAMAGE_NUMBERS_DURATION * 60; i++)
        {
            float distance = Configuration.DAMAGE_NUMBERS_HEIGHT / (Configuration.DAMAGE_NUMBERS_DURATION * 60 * 32);
            transform.position = transform.position + new Vector3(0, distance, 0);
            Color color = Text.color;
            float fadeAlpha = 1 - (i / (Configuration.DAMAGE_NUMBERS_DURATION * 60f));
            float fogAlpha = GameManager.Singleton.World.GetVisibilityLevel(transform.position);
            color.a = fadeAlpha * fogAlpha;
            Text.color = color;
            yield return null;
        }
        SignalFinish();
        Destroy(gameObject);
    }

    public void AssignSignalFinish(SignalFinishDelegate signalFinish)
    {
        SignalFinish = signalFinish;
    }
}
