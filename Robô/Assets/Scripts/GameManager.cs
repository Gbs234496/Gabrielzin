using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.InputSystem; 
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    

    [SerializeField] private GameState currentState;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeState(GameState.Iniciando);
        RequestSceneLoad("Splash"); 
    }
    
    public void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log($"<color=cyan>Estado do Jogo alterado para: {currentState}</color>");
    }
    
    public void RequestSceneLoad(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    
    public void AssignPlayerInput(PlayerInput playerInput)
    {
        Debug.Log("Input alocado para o jogador.");
    }
    
    public void MenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal");
       Instance.ChangeState(GameState.MenuPrincipal);
    }
    /*
    public void Splash()
    {
        SceneManager.LoadScene("Splash");
       // Instance.ChangeState(GameState.ExibindoSplash);
    }*/

    public void GamePlay()
    {
        Instance.ChangeState(GameState.Gameplay);
        SceneManager.LoadScene("SampleScene");
    }
        
    
}