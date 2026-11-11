using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class VictoryUIController : MonoBehaviour
{
    public TextMeshProUGUI textoVencedor;
    public TextMeshProUGUI textoRoboVencedor;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        int vencedorID = PlayerOM.UltimoVencedorID;
        string nomeRobo = PlayerOM.UltimoRoboVencedorNome;

        if (textoVencedor != null)
            textoVencedor.text = $"Bola {vencedorID} levará o cinturão";

        if (textoRoboVencedor != null)
            textoRoboVencedor.text = $"Bola escolhida: {nomeRobo}";
    }

    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.None || !Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
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
            SceneManager.LoadScene("SelecaoRobos");
        }
    }
}