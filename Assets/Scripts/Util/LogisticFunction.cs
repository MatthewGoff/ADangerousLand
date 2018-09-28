using UnityEngine;

public struct LogisticFunction
{
    private readonly float L;
    private readonly float K;
    private readonly float X0;

    /*
     * Initialize a logistic function with points (0,a), (b,c*l) and
     * carrying capacity l.
     * (c is a percentage of carying capacity)
     */
    public LogisticFunction(float l, float a, float b, float c)
    {
        L = l;
        K = (Mathf.Log((l / a) - 1) - Mathf.Log((1 / c) - 1)) / b;
        X0 = b / (1 - (Mathf.Log((1 / c) - 1) / Mathf.Log((l / a) - 1)));
    }

    public float Evaluate(float x)
    {
        return (L/(1+Mathf.Exp(-K*(x-X0))));
    }
}
