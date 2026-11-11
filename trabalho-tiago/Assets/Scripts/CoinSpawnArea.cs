using UnityEngine;

public class CoinSpawnArea : MonoBehaviour
{
    [Header("Arraste o Prefab da Moeda aqui")]
    public GameObject PrefabMoeda;

    private void Start()
    {
        BoxCollider box = GetComponent<BoxCollider>();

        if (GameManager.Instance != null)
        {
            if (box != null) GameManager.Instance.AreaDeSpawnMoedas = box;
            if (PrefabMoeda != null) GameManager.Instance.PrefabMoeda = PrefabMoeda;
        }
        else
        {
            Debug.LogWarning("Não foi possível registrar a Área de Spawn no GameManager.");
        }
    }
}