using UnityEngine;

[CreateAssetMenu(fileName = "NovoRobo", menuName = "GuerraDeRobos/RoboData")]
public class RoboData : ScriptableObject
{
    [Header("Identificação")]
    public string NomeRobo = "Robô Padrão";
    public GameObject PrefabRobo; // O seu único prefab de robô

    [Header("Diferenciação Visual & Esporte")]
    public Sprite IconeBola; // Imagem da bola para exibir na UI da Seleção
    public Texture2D TexturaBola; // Textura da bola para aplicar na malha 3D do robô
    public float EscalaTamanho = 1.0f; // Altera o tamanho visual (0.8 a 1.3)
    public Material MaterialBase; // Material padrão/base do robô
    
    [Tooltip("Cor aplicada ao robô do P2 caso ambos escolham o mesmo esporte.")]
    public Color CorDuplicado = new Color(1f, 0.45f, 0.45f, 1f); // Cor por defeito (vermelho)

    [Header("Atributos de Gameplay")]
    public float VelocidadeInicial = 4.0f;
    public float ForcaEmpurraoBase = 12.0f;
    public float RaioEmpurrao = 3.5f; // Distância máxima para atingir o inimigo
    public float TempoCooldownEmpurrao = 3.0f; // Tempo para carregar a barra
}