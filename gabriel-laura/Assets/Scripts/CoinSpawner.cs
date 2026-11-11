using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Prefab da Moeda")]
    public GameObject prefabMoeda;

    [Header("Objeto do Chão")]
    public Transform transformChao;
    public float alturaAcimaDoChao = 0.8f;

    [Header("Recuo de Segurança")]
    [Tooltip("Distância (em metros) longe das bordas do chão para evitar locais inalcançáveis.")]
    public float margemBorda = 2.5f;

    [Header("Controle de Moedas")]
    public int maxMoedasNaArena = 5;
    public float tempoEntreSpawns = 3f;

    private Collider _colliderChao;
    private float _timer;

    private void Start()
    {
        if (transformChao != null)
        {
            _colliderChao = transformChao.GetComponent<Collider>();
            if (_colliderChao == null)
            {
                Debug.LogError("CoinSpawner: O objeto do Chão precisa ter um Collider anexado!");
            }
        }
    }

    private void Update()
    {
        if (prefabMoeda == null || _colliderChao == null) return;

        _timer += Time.deltaTime;
        if (_timer >= tempoEntreSpawns)
        {
            _timer = 0f;
            ChecarESpawnarMoeda();
        }
    }

    private void ChecarESpawnarMoeda()
    {
        int quantidadeAtual = FindObjectsByType<Moeda>(FindObjectsSortMode.None).Length;

        if (quantidadeAtual < maxMoedasNaArena)
        {
            Bounds bounds = _colliderChao.bounds;

            // Recua os limites das bordas conforme a margem configurada
            float minX = bounds.min.x + margemBorda;
            float maxX = bounds.max.x - margemBorda;
            float minZ = bounds.min.z + margemBorda;
            float maxZ = bounds.max.z - margemBorda;

            // Proteção caso a margem seja maior que a própria área
            if (minX >= maxX) { minX = bounds.min.x; maxX = bounds.max.x; }
            if (minZ >= maxZ) { minZ = bounds.min.z; maxZ = bounds.max.z; }

            float xAleatorio = Random.Range(minX, maxX);
            float zAleatorio = Random.Range(minZ, maxZ);

            Vector3 posicaoSpawn = new Vector3(xAleatorio, bounds.max.y + alturaAcimaDoChao, zAleatorio);

            Instantiate(prefabMoeda, posicaoSpawn, prefabMoeda.transform.rotation);
        }
    }

    // Desenha a área útil amarela na aba Scene quando o Spawner estiver selecionado
    private void OnDrawGizmosSelected()
    {
        if (transformChao == null) return;
        Collider col = transformChao.GetComponent<Collider>();
        if (col == null) return;

        Bounds bounds = col.bounds;
        Gizmos.color = Color.yellow;

        Vector3 centro = new Vector3(bounds.center.x, bounds.max.y + alturaAcimaDoChao, bounds.center.z);
        Vector3 tamanho = new Vector3(
            Mathf.Max(0.1f, bounds.size.x - (margemBorda * 2f)),
            0.2f,
            Mathf.Max(0.1f, bounds.size.z - (margemBorda * 2f))
        );

        Gizmos.DrawWireCube(centro, tamanho);
    }
}