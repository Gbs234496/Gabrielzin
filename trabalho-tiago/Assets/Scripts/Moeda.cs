using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(Collider))]
public class Moeda : MonoBehaviour
{
    [Header("Efeitos")]
    public float velocidadeRotacao = 100f;

    private void Update()
    {
        // Efeito simples de rotação contínua da moeda
        transform.Rotate(Vector3.up, velocidadeRotacao * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Procura pelo script do robô que colidiu
        ThirdPersonController player = other.GetComponent<ThirdPersonController>();
        if (player == null) player = other.GetComponentInParent<ThirdPersonController>();

        if (player != null)
        {
            // Aplica os bônus no jogador e notifica a coleta
            player.ColetarMoeda();

            // Destrói a moeda coletada
            Destroy(gameObject);
        }
    }
}