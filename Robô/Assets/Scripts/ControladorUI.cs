using UnityEngine;
using TMPro;

public class ControladorUI : MonoBehaviour
{
    #region Inspector Fields

    [Header("UI - Placar de Estrelas")]
    [SerializeField] private TextMeshProUGUI textoEstrelasP1;
    [SerializeField] private string prefixoP1 = "P1 Estrelas: ";

    [SerializeField] private TextMeshProUGUI textoEstrelasP2;
    [SerializeField] private string prefixoP2 = "P2 Estrelas: ";

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
        PlayerOM.OnPlayerWon += OnVitoriaGatilho;
        Debug.Log("[ControladorUI] Inscrito com sucesso nos eventos de Estrelas do PlayerOM.");
    }

    private void OnDisable()
    {
        PlayerOM.OnStarCountChanged -= OnEstrelasAlteradas;
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
        AtualizarTextoUI(1, PlayerOM.GetStars(1));
        AtualizarTextoUI(2, PlayerOM.GetStars(2));
    }

    #endregion

    #region UI & Gameplay Logic

    private void OnEstrelasAlteradas(int playerID, int novaQuantidade)
    {
        Debug.Log($"[ControladorUI] Evento recebido! Player {playerID} pegou estrela. Total: {novaQuantidade}");

        if (_jogoFinalizado) return;

        AtualizarTextoUI(playerID, novaQuantidade);

        if (novaQuantidade >= estrelasParaVencer)
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
        if ((playerID == 1 || playerID == 0) && textoEstrelasP1 != null)
        {
            textoEstrelasP1.text = $"{prefixoP1}{quantidade}";
        }
        else if (playerID == 2 && textoEstrelasP2 != null)
        {
            textoEstrelasP2.text = $"{prefixoP2}{quantidade}";
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