using UnityEngine;
using TMPro;

public class GerenciadorTextoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoMoedas; 

    // O OnEnable roda assim que a UI é ativada/carregada
    private void OnEnable()
    {
        PlayerObserverManager.OnMoedasAlteradas += AtualizarTextoMoedas;
    }

    // O OnDisable roda se a UI for desligada ou destruída (previne vazamento de memória)
    private void OnDisable()
    {
        PlayerObserverManager.OnMoedasAlteradas -= AtualizarTextoMoedas;
    }
    

    private void AtualizarTextoMoedas(int quantidade) 
    {
        if (textoMoedas != null)
        {
            textoMoedas.text = "Moedas: " + quantidade;
            Debug.Log($"[UI VISUAL] O texto na tela mudou fisicamente para: Moedas: " + quantidade);
        }
        else
        {
            Debug.LogError("[UI] Erro Crítico: O campo 'textoMoedas' está vazio no Inspector!");
        }
    }
}