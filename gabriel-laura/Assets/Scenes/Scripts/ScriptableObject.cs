using UnityEngine;

[CreateAssetMenu(fileName = "NovoRobo", menuName = "GuerraDeRobos/RoboData")]
public class RoboData : ScriptableObject
{
    public string NomeRobo = "Robô Padrão";
    public GameObject PrefabRobo; // O seu único prefab de robô

    public Sprite IconeBola; 
        public Texture2D TexturaBola;
    public float EscalaTamanho = 1.0f; 
    public Material MaterialBase; 
    public Color CorDuplicado = new Color(1f, 0.45f, 0.45f, 1f); 
    public float VelocidadeInicial = 4.0f;
    public float ForcaEmpurraoBase = 12.0f;
    public float RaioEmpurrao = 3.5f; 
    public float TempoCooldownEmpurrao = 3.0f; 
}