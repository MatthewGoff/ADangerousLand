using UnityEngine;

public class Treadmill
{
    public Vector2 Center { get; set; }
    public readonly float Width;
    public readonly float Height;

    public Treadmill (float width, float height)
    {
        Width = width;
        Height = height;
    }

    public bool OnTreadmill((float X, float Y) position)
    {
        bool withinX = (position.X > Center.x - Width / 2) && (position.X < Center.x + Width / 2);
        bool withinY = (position.Y > Center.y - Height / 2) && (position.Y < Center.y + Height / 2);
        return withinX && withinY;
    }

    public bool OnTreadmill(Vector2 position)
    {
        bool withinX = (position.x > Center.x - Width / 2) && (position.x < Center.x + Width / 2);
        bool withinY = (position.y > Center.y - Height / 2) && (position.y < Center.y + Height / 2);
        return withinX && withinY;
    }
}