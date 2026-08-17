using UnityEngine;
using UnityEngine.SceneManagement; // Obrigatório para carregar cenas

public class GerenciadorDoJogo : MonoBehaviour
{
    [SerializeField] private string nomeDaCenaUI = "Cena_UI_Moedas";

    void Start()
    {
        CarregarInterfaceAditiva();
    }

    private void CarregarInterfaceAditiva()
    {
        // Verifica se a cena de UI já não está aberta (evita carregar a mesma UI duas vezes)
        if (!SceneManager.GetSceneByName(nomeDaCenaUI).isLoaded)
        {
            // Carrega a cena de UI por cima da cena atual de Gameplay
            SceneManager.LoadSceneAsync(nomeDaCenaUI, LoadSceneMode.Additive);
            
            Debug.Log($"<color=cyan>[GerenciadorDoJogo] Carregando a cena {nomeDaCenaUI} de forma aditiva.</color>");
        }
    }
}