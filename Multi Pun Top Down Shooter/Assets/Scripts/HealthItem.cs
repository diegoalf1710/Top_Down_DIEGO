using UnityEngine;

/// <summary>
/// Controla el comportamiento de los objetos que restauran salud al jugador.
/// Hereda de InteractiveItem para la funcionalidad base de objetos interactivos.
/// </summary>
public class HealthItem : InteractiveItem
{
    [Header("Health Settings")]
    /// <summary>
    /// Cantidad de salud que restaura al jugador
    /// </summary>
    public int healthAmount = 25;

    /// <summary>
    /// Sistema de partículas que se reproduce al curar
    /// </summary>
    public ParticleSystem healEffect;

    /// <summary>
    /// Si es true, cura automáticamente al tocar sin requerir interacción manual
    /// </summary>
    public bool autoHeal = true;

    /// <summary>
    /// Se ejecuta al iniciar. Configura si se requiere presionar una tecla para activar el objeto
    /// según el valor de autoHeal.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        requireKeyPress = !autoHeal;
    }

    /// <summary>
    /// Se ejecuta cuando el jugador interactúa con el objeto.
    /// Intenta curar al jugador y reproduce el efecto visual si la curación fue exitosa.
    /// </summary>
    /// <param name="player">Objeto del jugador que interactúa con el item</param>
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

    /// <summary>
    /// Reproduce el efecto de partículas de curación y lo destruye después de completarse.
    /// </summary>
    private void PlayHealEffect()
    {
        if (healEffect != null)
        {
            ParticleSystem effect = Instantiate(healEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, effect.main.duration);
        }
    }
}
