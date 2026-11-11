using UnityEngine;
using StarterAssets;

public class ArenaManager : MonoBehaviour
{
    [Header("Pontos de Spawn")]
    public Transform spawnP1;
    public Transform spawnP2;

    [Header("Robô Padrão (Fallback caso rode direto a cena da Arena)")]
    public RoboData roboPadrao;

    private void Start()
    {
        SpawnarJogadores();
    }

    private void SpawnarJogadores()
    {
        // Pega os dados do PlayerOM ou usa o padrão se estiver testando direto na Arena
        RoboData dataP1 = PlayerOM.RoboP1 != null ? PlayerOM.RoboP1 : roboPadrao;
        RoboData dataP2 = PlayerOM.RoboP2 != null ? PlayerOM.RoboP2 : roboPadrao;

        if (dataP1 == null || dataP1.PrefabRobo == null || dataP2 == null || dataP2.PrefabRobo == null)
        {
            Debug.LogError("Erro no Spawn: PrefabRobo não atribuído no RoboData!");
            return;
        }

        // --- SPAWN JOGADOR 1 ---
        GameObject p1Obj = Instantiate(dataP1.PrefabRobo, spawnP1.position, spawnP1.rotation);
        ThirdPersonController controllerP1 = p1Obj.GetComponent<ThirdPersonController>();
        if (controllerP1 != null)
        {
            controllerP1.PlayerID = 1;
            controllerP1.InicializarComRoboData(dataP1, false);
        }

        // --- SPAWN JOGADOR 2 ---
        GameObject p2Obj = Instantiate(dataP2.PrefabRobo, spawnP2.position, spawnP2.rotation);
        ThirdPersonController controllerP2 = p2Obj.GetComponent<ThirdPersonController>();
        if (controllerP2 != null)
        {
            controllerP2.PlayerID = 2;
            controllerP2.InicializarComRoboData(dataP2, PlayerOM.P2UsarCorSecundaria);
        }
    }
}