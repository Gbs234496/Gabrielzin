using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Nomes das Cenas")]
    public string SelectionSceneName = "SelecaoRobos";
    public string GameplaySceneName = "Gameplay";
    public string GUISceneName = "GUI";
    public string VictorySceneName = "Vitoria";

    [Header("Spawn de Moedas")]
    public GameObject PrefabMoeda;
    public BoxCollider AreaDeSpawnMoedas;
    public float IntervaloSpawnMoeda = 4.0f;

    private const int VITORIAS_PARA_GANHAR = 2; // Melhor de 3 (quem fizer 2 pontos vence)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        string cenaAtual = SceneManager.GetActiveScene().name;

        if (cenaAtual == "_Boot")
        {
            SceneManager.LoadScene(SelectionSceneName);
        }
        else if (cenaAtual == GameplaySceneName)
        {
            IniciarRotinaMoedas();
        }
    }

    public void CarregarJogo()
    {
        PararRotinaMoedas();
        PlayerOM.ResetScores();
        SceneManager.LoadScene(GameplaySceneName, LoadSceneMode.Single);
        SceneManager.LoadScene(GUISceneName, LoadSceneMode.Additive);
    }

    public void IniciarRotinaMoedas()
    {
        StopAllCoroutines();
        StartCoroutine(RoutineSpawnMoedas());
    }

    public void PararRotinaMoedas()
    {
        StopAllCoroutines();
    }

    private IEnumerator RoutineSpawnMoedas()
    {
        while (true)
        {
            yield return new WaitForSeconds(IntervaloSpawnMoeda);
            SpawnarMoeda();
        }
    }

    private void SpawnarMoeda()
    {
        // Se a área não existe mais (ex: mudou de cena), encerra a rotina sem gerar erro no console
        if (AreaDeSpawnMoedas == null)
        {
            PararRotinaMoedas();
            return;
        }

        if (PrefabMoeda == null)
        {
            Debug.LogWarning("[Moedas] PrefabMoeda não está atribuído no GameManager!");
            return;
        }

        Bounds bounds = AreaDeSpawnMoedas.bounds;
        
        // Posição aleatória ligeiramente elevada (+0.5f) para não ficar enterrada no chão
        Vector3 pontoAleatorio = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.center.y + 0.5f,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        Instantiate(PrefabMoeda, pontoAleatorio, Quaternion.identity);
    }

   public void RegistrarQuedaForaDaArena(int playerIDQueCaiu)
{
    int vencedorDoPonto = (playerIDQueCaiu == 1) ? 2 : 1;

    PararRotinaMoedas();

    // 1. Atribui o ponto da queda ao adversário
    PlayerOM.AddPonto(vencedorDoPonto);

    int pontosVencedor = (vencedorDoPonto == 1) ? PlayerOM.P1PontosRound : PlayerOM.P2PontosRound;

    // 2. Verifica se fechou o Round (Melhor de 3 pontos -> 2 pontos ganham o round)
    if (pontosVencedor >= 2)
    {
        PlayerOM.AddRoundWin(vencedorDoPonto);
        int roundsVencedor = (vencedorDoPonto == 1) ? PlayerOM.P1RoundsVencidos : PlayerOM.P2RoundsVencidos;

        // 3. Verifica se fechou a Partida (Melhor de 3 rounds -> 2 rounds ganham o jogo)
        if (roundsVencedor >= 2)
        {
            RoboData roboVencedor = (vencedorDoPonto == 1) ? PlayerOM.RoboP1 : PlayerOM.RoboP2;
            string nomeRobo = (roboVencedor != null) ? roboVencedor.NomeRobo : "Robô";

            PlayerOM.TriggerMatchWin(vencedorDoPonto, nomeRobo);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneManager.LoadScene(VictorySceneName);
            return;
        }
    }

    // Se a partida não acabou, recarrega a arena para a próxima disputa
    SceneManager.LoadScene(GameplaySceneName, LoadSceneMode.Single);
    SceneManager.LoadScene(GUISceneName, LoadSceneMode.Additive);
}

    public void ReiniciarPartidaCompleta()
    {
        PararRotinaMoedas();

        // Garante visibilidade e desbloqueio do mouse na Seleção
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerOM.ResetScores();

        SceneManager.LoadScene(SelectionSceneName);
    }
}