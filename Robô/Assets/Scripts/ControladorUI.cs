using UnityEngine;
using TMPro; // Se estiver usando UI tradicional, troque por usando UnityEngine.UI;

public class ControladorUI : MonoBehaviour
{
    [Header("UI - Placar de Moedas")]
    [SerializeField] private TextMeshProUGUI textoMoedasP1;
    [SerializeField] private string prefixoP1 = "P1 Moedas: ";

    [SerializeField] private TextMeshProUGUI textoMoedasP2;
    [SerializeField] private string prefixoP2 = "P2 Moedas: ";

    [Header("UI - Painel de Vitória")]
    [SerializeField] private GameObject painelVencedor;
    [SerializeField] private TextMeshProUGUI textoVencedor;
    [SerializeField] private string mensagemVitoriaP1 = "JOGADOR 1 VENCEU!";
    [SerializeField] private string mensagemVitoriaP2 = "JOGADOR 2 VENCEU!";

    [Header("Regras do Jogo")]
    [SerializeField] private int moedasParaVencer = 5;

    private bool _jogoFinalizado;

    private void OnEnable()
    {
        PlayerOM.OnCoinCountChanged += OnMoedasAlteradas;
        PlayerOM.OnPlayerWon += OnVitoriaGatilho;
        Debug.Log("[ControladorUI] Inscrito com sucesso nos eventos do PlayerOM.");
    }

    private void OnDisable()
    {
        PlayerOM.OnCoinCountChanged -= OnMoedasAlteradas;
        PlayerOM.OnPlayerWon -= OnVitoriaGatilho;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        _jogoFinalizado = false;

        if (painelVencedor != null)
        {
            painelVencedor.SetActive(false);
        }

        PlayerOM.ResetScores();
        AtualizarTextoUI(1, PlayerOM.GetCoins(1));
        AtualizarTextoUI(2, PlayerOM.GetCoins(2));
    }

    private void OnMoedasAlteradas(int playerID, int novaQuantidade)
    {
        Debug.Log($"[ControladorUI] Evento recebido! Player {playerID} pegou moeda. Total: {novaQuantidade}");

        if (_jogoFinalizado) return;

        AtualizarTextoUI(playerID, novaQuantidade);

        if (novaQuantidade >= moedasParaVencer)
        {
            _jogoFinalizado = true;
            PlayerOM.TriggerWin(playerID);
        }
    }

    private void OnVitoriaGatilho(int playerID)
    {
        _jogoFinalizado = true;

        if (painelVencedor != null)
        {
            painelVencedor.SetActive(true);
        }

        if (textoVencedor != null)
        {
            textoVencedor.text = (playerID == 1) ? mensagemVitoriaP1 : mensagemVitoriaP2;
        }

        Time.timeScale = 0f;
    }

   private void AtualizarTextoUI(int playerID, int quantidade)
{
    // Aceita tanto PlayerID 1 quanto 0 para o primeiro jogador
    if ((playerID == 1 || playerID == 0) && textoMoedasP1 != null)
    {
        textoMoedasP1.text = $"{prefixoP1}{quantidade}";
    }
    else if (playerID == 2 && textoMoedasP2 != null)
    {
        textoMoedasP2.text = $"{prefixoP2}{quantidade}";
    }
}

    public void ReiniciarPartida()
    {
        Time.timeScale = 1f;
        PlayerOM.ResetScores();
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}