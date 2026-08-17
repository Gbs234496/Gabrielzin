using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro; // 1. Obrigatório para manipular os textos de UI na tela

public class GerenciadorDeCenas : MonoBehaviour
{
    [Header("Sistema de Contagem de Moedas")]
    public int contadorDeMoedas = 0; // 2. Variável para contabilizar os objetos
    public TextMeshProUGUI textoDaTela; // 3. Variável para mudar o número na cena

    public void CarregarCena(string nomeDaCena)
    {
        SceneManager.LoadScene(nomeDaCena);
        Debug.Log("Carregando a cena: " + nomeDaCena);
    }

    // 4. Método criado para somar a moeda e atualizar o texto
    public void AtualizarMoedasNaTela()
    {
        contadorDeMoedas++;

        // Verifica se você lembrou de arrastar o texto no Inspetor para evitar erros
        if (textoDaTela != null) 
        {
            textoDaTela.text = contadorDeMoedas.ToString();
        }
        else
        {
            Debug.LogWarning("[UI] O Texto da Tela não foi associado no Inspetor!");
        }
    }
}