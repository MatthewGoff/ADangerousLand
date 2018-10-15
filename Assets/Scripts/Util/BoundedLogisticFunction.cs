using UnityEngine;

namespace ADL.Util
{
    public struct BoundedLogisticFunction
    {
        private readonly float I;
        private readonly float K;
        private readonly float X0;

        /*
         * Initialize a logistic function with points (0,a), (b,c*(b+i)) and
         * carrying capacity of linear bound y=x+i.
         * (c is a percentage of carying capacity)
         */
        public BoundedLogisticFunction(float i, float a, float b, float c)
        {
            I = i;
            K = (Mathf.Log((i / a) - 1) - Mathf.Log((1 / c) - 1)) / b;
            X0 = b / (1 - (Mathf.Log((1 / c) - 1) / Mathf.Log((i / a) - 1)));
        }

        public float Evaluate(float x)
        {
            return ((x + I) / (1 + Mathf.Exp(-K * (x - X0))));
        }
    }
}