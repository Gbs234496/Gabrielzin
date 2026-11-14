using UnityEngine;
using StarterAssets;

public class MatchInitializer : MonoBehaviour
{
    private void Start()
    {
        ConfigurarJogadores();
        IniciarSistemaMoedas();
    }

    private void ConfigurarJogadores()
    {
        ThirdPersonController[] players = FindObjectsByType<ThirdPersonController>(FindObjectsSortMode.None);

        for (int i = 0; i < players.Length; i++)
        {
            ThirdPersonController player = players[i];
            AplicarDadosDoRobo(player);
        }
    }

    private void AplicarDadosDoRobo(ThirdPersonController player)
    {
        switch (player.PlayerID)
        {
            case 1:
                if (PlayerOM.RoboP1 != null)
                    player.InicializarComRoboData(PlayerOM.RoboP1, PlayerOM.P1UsarCorSecundaria);
                break;

            case 2:
                if (PlayerOM.RoboP2 != null)
                    player.InicializarComRoboData(PlayerOM.RoboP2, PlayerOM.P2UsarCorSecundaria);
                break;
        }
    }

    private void IniciarSistemaMoedas()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.IniciarRotinaMoedas();
    }
}