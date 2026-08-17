using UnityEngine;
using UnityEngine.SceneManagement; // Obrigatório para gerenciamento de cenas

public class GerenciadorDeInterface : MonoBehaviour
{
    // Digite o nome exato da sua cena de UI no Inspetor da Unity
    [SerializeField] private string nomeDaCenaUI = "Cena_UI_Moedas";

    void Start()
    {
        CarregarInterfaceAditiva();
    }

    private void CarregarInterfaceAditiva()
    {
        // Verifica se a cena de UI já não está carregada para evitar duplicatas
        Scene cenaUI = SceneManager.GetSceneByName(nomeDaCenaUI);
        
        if (!cenaUI.isLoaded)
        {
            // O segredo está no LoadSceneMode.Additive
            SceneManager.LoadSceneAsync(nomeDaCenaUI, LoadSceneMode.Additive);
            Debug.Log($"<color=cyan>Carregando {nomeDaCenaUI} de forma aditiva.</color>");
        }
    }
    
    
}