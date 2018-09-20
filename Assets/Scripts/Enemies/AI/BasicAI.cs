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
        Vector2 playerPosition = GameManager.Singleton.World.PlayerManager.GetPlayerPosition();
        Vector2 myPosition = manager.MonoBehaviour.transform.position;
        float distance = (playerPosition - myPosition).magnitude;
        if (distance < manager.Aoe)
        {
            //manager.SlashAttack(playerPosition);
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
            return GameManager.Singleton.World.PlayerManager.GetPlayerPosition();
        }
        else
        {
            return manager.MonoBehaviour.transform.position;
        }
    }
}