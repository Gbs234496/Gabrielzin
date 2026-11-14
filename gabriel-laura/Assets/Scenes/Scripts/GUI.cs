using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Ugui : MonoBehaviour
{
    [Header("Placar Centralizado")]
    public TextMeshProUGUI textoRoundsCentral;

    [Header("Contador de Moedas")]
    public TextMeshProUGUI textoMoedasP1;
    public TextMeshProUGUI textoMoedasP2;

    [Header("Cooldown do Golpe")]
    public Slider sliderCooldownP1;
    public Slider sliderCooldownP2;

    private void OnEnable()
    {
        PlayerOM.OnScoreUpdated += AtualizarUI;
        PlayerOM.OnCooldownUpdated += AtualizarCooldown;
    }

    private void OnDisable()
    {
        PlayerOM.OnScoreUpdated -= AtualizarUI;
        PlayerOM.OnCooldownUpdated -= AtualizarCooldown;
    }

    private void Start()
    {
        AtualizarUI();
    }

    public void AtualizarUI()
    {
        AtualizarPlacar();
        AtualizarMoedas();
    }

    public void AtualizarPlacar()
    {
        if (textoRoundsCentral != null)
            textoRoundsCentral.text = $"ROUNDS: {PlayerOM.P1RoundsVencidos} x {PlayerOM.P2RoundsVencidos}";
    }

    public void AtualizarMoedas()
    {
        if (textoMoedasP1 != null)
            textoMoedasP1.text = $"Moedas: {PlayerOM.MoedasP1}";

        if (textoMoedasP2 != null)
            textoMoedasP2.text = $"Moedas: {PlayerOM.MoedasP2}";
    }

    private void AtualizarCooldown(int playerID, float progresso)
    {
        if (playerID == 1 && sliderCooldownP1 != null)
            sliderCooldownP1.value = progresso;
        else if (playerID == 2 && sliderCooldownP2 != null)
            sliderCooldownP2.value = progresso;
    }
}