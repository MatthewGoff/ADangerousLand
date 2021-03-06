﻿using UnityEngine;
using System;
using ADL.Core;

namespace ADL.Util
{
    public static class Helpers
    {

        public static void Populate2D<T>(ref T[,] array, T value)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = value;
                }
            }
        }

        public static Vector2 ClockwiseNormalVector(Vector2 vector)
        {
            return new Vector2(vector.y, -vector.x);
        }

        public static Vector2 CClockwiseNormalVector(Vector2 vector)
        {
            return new Vector2(-vector.y, vector.x);
        }

        public static bool WithinArrayBounds2D<T>(ref T[,] array, int x, int y)
        {
            return (x >= 0 && x < array.GetLength(0) && y >= 0 && y < array.GetLength(1));
        }

        public static (int X, int Y) RoundVector2(Vector2 vector)
        {
            return ((int)Mathf.Round(vector.x), (int)Mathf.Round(vector.y));
        }

        public static int RandomInt(int min, int max)
        {
            int var = Mathf.FloorToInt(UnityEngine.Random.Range(min, max + 1));
            return Mathf.Clamp(var, min, max);
        }

        public static float ArrayMinimum(float[] array)
        {
            if (array == null || array.GetLength(0) == 0)
            {
                return 0;
            }
            float minimum = array[0];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                if (array[i] < minimum)
                {
                    minimum = array[i];
                }
            }
            return minimum;
        }

        public static float ArrayMaximum(float[] array)
        {
            if (array == null || array.GetLength(0) == 0)
            {
                return 0;
            }
            float maximum = array[0];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                if (array[i] > maximum)
                {
                    maximum = array[i];
                }
            }
            return maximum;
        }

        public static float GetPerlinNoise(float seed, float[] periods, (float X, float Y) position)
        {
            float offset = 10000.0f / seed;
            float accumulator = 0.0f;
            for (int i = 0; i < periods.GetLength(0); i++)
            {
                accumulator += Mathf.PerlinNoise(position.X / periods[i] + offset, position.Y / periods[i] + offset);
            }
            return accumulator / periods.GetLength(0);
        }

        public static Vector2 GetPerlinNoiseGradient(float seed, float[] periods, (float X, float Y) position)
        {
            float center = GetPerlinNoise(seed, periods, position);
            float h = ArrayMinimum(periods) / 10.0f;
            float up = GetPerlinNoise(seed, periods, (position.X, position.Y + h)) / h;
            float right = GetPerlinNoise(seed, periods, (position.X + h, position.Y)) / h;
            return new Vector2(center - right, center - up);
        }

        public static float NormalRandom(float min, float max)
        {
            float mean = (min + max) / 2.0f;
            float sigma = (max - mean) / 3.0f;
            float rand = NextGaussianFloat() * sigma + mean;
            if (rand < min)
            {
                rand = min;
            }
            else if (rand > max)
            {
                rand = max;
            }
            return rand;
        }

        public static float EuclidianDistance((float X, float Y) point1, (float X, float Y) point2)
        {
            return (float)Math.Sqrt(Math.Pow((point1.X - point2.X), 2) + Math.Pow((point1.Y - point2.Y), 2));
        }

        public static float EuclidianDistance(Vector2 point1, (float X, float Y) point2)
        {
            return (float)Math.Sqrt(Math.Pow((point1.x - point2.X), 2) + Math.Pow((point1.y - point2.Y), 2));
        }

        public static float NextGaussianFloat()
        {
            float u, v, S;
            do
            {
                u = 2.0f * UnityEngine.Random.value - 1.0f;
                v = 2.0f * UnityEngine.Random.value - 1.0f;
                S = u * u + v * v;
            } while (S >= 1.0f);

            float fac = Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);
            return u * fac;
        }

        public static Vector2 RoundToPixel(Vector2 vector, int pixelsPerUnit)
        {
            vector *= pixelsPerUnit;
            vector = new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
            vector /= pixelsPerUnit;
            return vector;
        }

        public static float Round(float value, float place)
        {
            value /= place;
            value = Mathf.Round(value);
            value *= place;
            return value;
        }

        public static string Truncate(string value, int length)
        {
            if (value.Length > length)
            {
                return value.Substring(0, length);
            }
            else
            {
                return value;
            }
        }

        public static int SortingOrder(Vector2 position)
        {
            return Mathf.RoundToInt(-position.y * Configuration.PIXELS_PER_UNIT);
        }

        /*
         * Indend the given message by inserting <tabs> tabs after every newline
         * where each tabs is <spaces> spaces.
         */
        public static string Indent(string message, int spaces, int tabs)
        {
            string returnMessage = "";
            string indent = Tabs(spaces, tabs);
            for (int i = 0; i < message.Length; i++)
            {
                if (message[i] == '\n')
                {
                    returnMessage += '\n' + indent;
                }
                else
                {
                    returnMessage += message[i];
                }
            }
            return returnMessage;
        }

        public static string Tabs(int spaces, int tabs)
        {
            string returnString = "";
            for (int i = 0; i < tabs * spaces; i++)
            {
                returnString += ' ';
            }
            return returnString;
        }
    }
}