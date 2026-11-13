using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryUIController : MonoBehaviour
{
    [Header("Referências de UI")]
    public TextMeshProUGUI textoVencedor;
    public TextMeshProUGUI textoRoboVencedor;

    [Header("Personalização dos Textos")]
    [Tooltip("Use {0} onde deve aparecer o número do jogador vencedor.")]
    public string formatoVencedor = "JOGADOR {0} VENCEU!";

    [Tooltip("Use {0} onde deve aparecer o nome da joia escolhida.")]
    public string formatoJoia = "Joia escolhida: {0}";

    [Header("Configuração de Cena")]
    public string nomeCenaSelecao = "SelecaoRobos";

    private void OnEnable()
    {
        LiberarCursor();
    }

    private void Start()
    {
        LiberarCursor();
        ExibirDadosDaVitoria();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) LiberarCursor();
    }

    private void ExibirDadosDaVitoria()
    {
        int vencedorID = PlayerOM.UltimoVencedorID;
        string nomeJoia = ObterNomeDaJoia(vencedorID);

        if (textoVencedor != null)
            textoVencedor.text = string.Format(formatoVencedor, vencedorID);

        if (textoRoboVencedor != null)
            textoRoboVencedor.text = string.Format(formatoJoia, nomeJoia);
    }

    private string ObterNomeDaJoia(int vencedorID)
    {
        if (!string.IsNullOrEmpty(PlayerOM.UltimoRoboVencedorNome))
            return PlayerOM.UltimoRoboVencedorNome;

        if (vencedorID == 1 && PlayerOM.RoboP1 != null)
            return PlayerOM.RoboP1.NomeRobo;

        if (vencedorID == 2 && PlayerOM.RoboP2 != null)
            return PlayerOM.RoboP2.NomeRobo;

        return "Desconhecida";
    }

    public void VoltarParaSelecao()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReiniciarPartidaCompleta();
        }
        else
        {
            PlayerOM.ResetScores();
            SceneManager.LoadScene(nomeCenaSelecao);
        }
    }

    private void LiberarCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}