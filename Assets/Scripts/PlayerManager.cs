using UnityEngine;

public class PlayerManager : CombatantManager
{
    public delegate float MovementMultiplierDelegate(WorldLocation worldLocation);

    public MovementMultiplierDelegate MovementMultiplier;
    public float MoveSpeed = Configuration.DEFAULT_MOVE_SPEED;
    public PlayerMonoBehaviour MonoBehaviour;

    private Cooldown SlashCooldown;

    public PlayerManager(WorldLocation spawnLocation, MovementMultiplierDelegate movementMultiplier)
    {
        SlashCooldown = new Cooldown(1f);
        MovementMultiplier = movementMultiplier;

        GameObject player = GameObject.Instantiate(Prefabs.PLAYER_PREFAB, new Vector3(spawnLocation.X, spawnLocation.Y, 0), Quaternion.identity);
        MonoBehaviour = player.GetComponent<PlayerMonoBehaviour>();
        MonoBehaviour.AssignManager(this);
    }

    public Vector2 GetPlayerPosition()
    {
        return MonoBehaviour.transform.position;
    }

    public void SlashAttack(Vector2 position, Quaternion angle)
    {
        if (SlashCooldown.Use())
        {
            SlashManager slash = new SlashManager(this, position, angle, 2);
        }
    }

    public override void RecieveHit()
    {

    }
}
