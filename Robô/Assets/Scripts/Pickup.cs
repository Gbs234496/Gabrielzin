using UnityEngine;

public class Pickup : MonoBehaviour
{
    #region Inspector Fields

    [Header("Effects")]
    public GameObject particleEffectPrefab;

    [Header("Motion Settings")]
    public float rotationSpeed = 100f;
    public float bobbingAmount = 0.1f;
    public float bobbingSpeed = 1f;

    #endregion

    #region Private Fields

    private Vector3 startPosition;
    private float timer;

    #endregion

    #region Unity LifeCycle

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Rotação da estrela
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Movimento de flutuação (bobbing)
        timer += Time.deltaTime * bobbingSpeed;
        float newY = startPosition.y + Mathf.Sin(timer) * bobbingAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Tenta obter o script do jogador para saber quem pegou a estrela
            var player = other.GetComponent<StarterAssets.ThirdPersonController>();
            if (player == null)
            {
                player = other.GetComponentInParent<StarterAssets.ThirdPersonController>();
            }

            if (player != null)
            {
                // Pontua para o jogador correto (certifique-se de que o PlayerOM possui o método correspondente)
                PlayerOM.AddStar(player.PlayerID); 
            }

            if (particleEffectPrefab != null)
            {
                Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    #endregion
}