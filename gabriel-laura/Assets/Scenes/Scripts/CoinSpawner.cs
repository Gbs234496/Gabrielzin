using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Configuração do Item")]
    public GameObject objetoMoeda;

    [Header("Superfície de Geração")]
    public Transform pisoReferencia;
    public float elevacaoVertical = 0.8f;
    public float recuoLimite = 2.5f;

    [Header("Controle de Quantidade e Tempo")]
    public int limiteMoedasAtivas = 3;
    public float intervaloGeracao = 3f;

    private Collider _colisorPiso;
    private float _cronometro;

    private void Start()
    {
        if (pisoReferencia != null)
        {
            _colisorPiso = pisoReferencia.GetComponent<Collider>();
            if (_colisorPiso == null)
            {
                Debug.LogError("CoinSpawner: O objeto de piso precisa ter um Collider anexado!");
            }
        }
    }

    private void Update()
    {
        if (objetoMoeda == null || _colisorPiso == null) return;

        _cronometro += Time.deltaTime;
        if (_cronometro >= intervaloGeracao)
        {
            _cronometro = 0f;
            ProcessarGeracaoMoeda();
        }
    }

    private void ProcessarGeracaoMoeda()
    {
        int moedasEmCena = FindObjectsByType<Moeda>(FindObjectsSortMode.None).Length;

        if (moedasEmCena < limiteMoedasAtivas)
        {
            Bounds limitesPiso = _colisorPiso.bounds;

            float limiteMinX = limitesPiso.min.x + recuoLimite;
            float limiteMaxX = limitesPiso.max.x - recuoLimite;
            float limiteMinZ = limitesPiso.min.z + recuoLimite;
            float limiteMaxZ = limitesPiso.max.z - recuoLimite;

            if (limiteMinX >= limiteMaxX) { limiteMinX = limitesPiso.min.x; limiteMaxX = limitesPiso.max.x; }
            if (limiteMinZ >= limiteMaxZ) { limiteMinZ = limitesPiso.min.z; limiteMaxZ = limitesPiso.max.z; }

            float coordX = Random.Range(limiteMinX, limiteMaxX);
            float coordZ = Random.Range(limiteMinZ, limiteMaxZ);

            Vector3 pontoDeCriacao = new Vector3(coordX, limitesPiso.max.y + elevacaoVertical, coordZ);

            Instantiate(objetoMoeda, pontoDeCriacao, objetoMoeda.transform.rotation);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pisoReferencia == null) return;
        Collider colisorVisual = pisoReferencia.GetComponent<Collider>();
        if (colisorVisual == null) return;

        Bounds dimensoesArea = colisorVisual.bounds;
        Gizmos.color = Color.yellow;

        Vector3 pontoCentral = new Vector3(dimensoesArea.center.x, dimensoesArea.max.y + elevacaoVertical, dimensoesArea.center.z);
        Vector3 escalaCaixa = new Vector3(
            Mathf.Max(0.1f, dimensoesArea.size.x - (recuoLimite * 2f)),
            0.2f,
            Mathf.Max(0.1f, dimensoesArea.size.z - (recuoLimite * 2f))
        );

        Gizmos.DrawWireCube(pontoCentral, escalaCaixa);
    }
}