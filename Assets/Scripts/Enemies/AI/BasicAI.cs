using UnityEngine;
using MessagePack;

[MessagePackObject]
public class BasicAI : EnemyAI
{
    [Key(0)] public bool Agroed;
    [Key(1)] public float AgroCountdown;

    public BasicAI ()
    {
        Agroed = false;
        AgroCountdown = 0f;
    }

    [SerializationConstructor]
    public BasicAI (bool agroed, float agroCountdown)
    {
        Agroed = agroed;
        AgroCountdown = agroCountdown;
    }

    public Vector2 FixedUpdate(EnemyManager manager)
    {
        Vector2 playerPosition = GameManager.Singleton.World.PlayerManager.GetPlayerCenter();
        Vector2 myPosition = manager.GetCenter();
        float distance = (playerPosition - myPosition).magnitude;
        if (distance < manager.Aoe + 1f)
        {
            manager.SlashAttack(playerPosition);
        }

        if ((distance < manager.AgroDistance)
            || (distance < manager.DeAgroDistance && Agroed))
        {
            Agroed = true;
            AgroCountdown = manager.MinAgroDuration;
        }

        AgroCountdown -= Time.fixedDeltaTime;
        if (AgroCountdown <= 0f)
        {
            Agroed = false;
        }

        if (Agroed)
        {
            return playerPosition-myPosition;
        }
        else
        {
            return Vector2.zero;
        }
    }
}