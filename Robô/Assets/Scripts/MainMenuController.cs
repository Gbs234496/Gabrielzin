using UnityEngine;
using UnityEngine.UI; // Necessário para interagir com Componentes do Canvas

public class MainMenuController : MonoBehaviour
{
    [Header("Configuração dos Botões")]
    [SerializeField] private Button btnIniciar;
    [SerializeField] private Button btnSair;

    private void Start()
    {
        // Adiciona as funções aos botões via código (mais seguro que o Inspector)
        if (btnIniciar != null)
            btnIniciar.onClick.AddListener(IniciarJogo);

        if (btnSair != null)
            btnSair.onClick.AddListener(SairDoJogo);
    }

    public void IniciarJogo()
    {
        // Usando os nomes das funções que você tem no seu GameManager
        GameManager.Instance.GamePlay();
        
    }

    public void SairDoJogo()
    {
        Debug.Log("O jogo fecharia agora (Application.Quit)");
        Application.Quit();
    }
}