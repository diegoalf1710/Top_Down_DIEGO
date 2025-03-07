using UnityEngine;

public class HealthItem : InteractiveItem
{
    [Header("Health Settings")]
    public int healthAmount = 25;
    public ParticleSystem healEffect;
    public bool autoHeal = true; // Si es true, cura automáticamente al tocar

    protected override void Start()
    {
        base.Start();
        requireKeyPress = !autoHeal; // Si es autoHeal, no requiere tecla
    }

    public override void OnInteract(GameObject player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            bool wasHealed = playerHealth.Heal(healthAmount);
            if (wasHealed)
            {
                PlayHealEffect();
                base.OnInteract(player);
            }
        }
    }

    private void PlayHealEffect()
    {
        if (healEffect != null)
        {
            ParticleSystem effect = Instantiate(healEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, effect.main.duration);
        }
    }
}
