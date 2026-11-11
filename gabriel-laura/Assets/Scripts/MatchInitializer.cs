using UnityEngine;
using StarterAssets;

public class MatchInitializer : MonoBehaviour
{
    private void Start()
    {
        ThirdPersonController[] players = FindObjectsByType<ThirdPersonController>(FindObjectsSortMode.None);

        foreach (ThirdPersonController player in players)
        {
            if (player.PlayerID == 1 && PlayerOM.RoboP1 != null)
            {
                player.InicializarComRoboData(PlayerOM.RoboP1, PlayerOM.P1UsarCorSecundaria);
            }
            else if (player.PlayerID == 2 && PlayerOM.RoboP2 != null)
            {
                player.InicializarComRoboData(PlayerOM.RoboP2, PlayerOM.P2UsarCorSecundaria);
            }
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.IniciarRotinaMoedas();
        }
    }
}