using UnityEngine;
using StarterAssets;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Tenta encontrar o ThirdPersonController no objeto colidido ou nos seus pais
        ThirdPersonController player = other.GetComponentInParent<ThirdPersonController>();

        if (player != null)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegistrarQuedaForaDaArena(player.PlayerID);
            }
        }
    }
}