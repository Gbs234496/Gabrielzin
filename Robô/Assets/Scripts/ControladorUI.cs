using UnityEngine;
using TMPro;

public class ControladorUI : MonoBehaviour
{
    #region Inspector Fields

    [Header("UI - Placar de Estrelas")]
    [SerializeField] private TextMeshProUGUI textoEstrelasP1;
    [SerializeField] private string prefixoEstrelasP1 = "P1 Estrelas: ";

    [SerializeField] private TextMeshProUGUI textoEstrelasP2;
    [SerializeField] private string prefixoEstrelasP2 = "P2 Estrelas: ";

    [Header("UI - Placar de Moedas")]
    [SerializeField] private TextMeshProUGUI textoMoedasP1;
    [SerializeField] private string prefixoMoedasP1 = "P1 Moedas: ";

    [SerializeField] private TextMeshProUGUI textoMoedasP2;
    [SerializeField] private string prefixoMoedasP2 = "P2 Moedas: ";

    [Header("UI - Painel de Vitória")]
    [SerializeField] private GameObject painelVencedor;
    [SerializeField] private TextMeshProUGUI textoVencedor;
    [SerializeField] private string mensagemVitoriaP1 = "JOGADOR 1 VENCEU!";
    [SerializeField] private string mensagemVitoriaP2 = "JOGADOR 2 VENCEU!";

    [Header("Regras do Jogo")]
    [SerializeField] private int estrelasParaVencer = 5;

    #endregion

    #region Private Fields

    private bool _jogoFinalizado;

    #endregion

    #region Unity LifeCycle & Subscriptions

    private void OnEnable()
    {
        PlayerOM.OnStarCountChanged += OnEstrelasAlteradas;
        PlayerOM.OnCoinCountChanged += OnMoedasAlteradas;
        PlayerOM.OnPlayerWon += OnVitoriaGatilho;
    }

    private void OnDisable()
    {
        PlayerOM.OnStarCountChanged -= OnEstrelasAlteradas;
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

        // Atualiza os 4 contadores no início da partida
        AtualizarTextoEstrelasUI(1, PlayerOM.GetStars(1));
        AtualizarTextoEstrelasUI(2, PlayerOM.GetStars(2));

        AtualizarTextoMoedasUI(1, PlayerOM.GetCoins(1));
        AtualizarTextoMoedasUI(2, PlayerOM.GetCoins(2));
    }

    #endregion

    #region Event Callbacks

    private void OnEstrelasAlteradas(int playerID, int novaQuantidade)
    {
        if (_jogoFinalizado) return;

        AtualizarTextoEstrelasUI(playerID, novaQuantidade);

        if (novaQuantidade >= estrelasParaVencer)
        {
            _jogoFinalizado = true;
            PlayerOM.TriggerWin(playerID);
        }
    }

    private void OnMoedasAlteradas(int playerID, int novaQuantidade)
    {
        if (_jogoFinalizado) return;

        AtualizarTextoMoedasUI(playerID, novaQuantidade);
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

    #endregion

    #region UI Methods

    private void AtualizarTextoEstrelasUI(int playerID, int quantidade)
    {
        if (playerID == 1 && textoEstrelasP1 != null)
        {
            textoEstrelasP1.text = $"{prefixoEstrelasP1}{quantidade}";
        }
        else if (playerID == 2 && textoEstrelasP2 != null)
        {
            textoEstrelasP2.text = $"{prefixoEstrelasP2}{quantidade}";
        }
    }

    private void AtualizarTextoMoedasUI(int playerID, int quantidade)
    {
        if (playerID == 1 && textoMoedasP1 != null)
        {
            textoMoedasP1.text = $"{prefixoMoedasP1}{quantidade}";
        }
        else if (playerID == 2 && textoMoedasP2 != null)
        {
            textoMoedasP2.text = $"{prefixoMoedasP2}{quantidade}";
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

    #endregion
}