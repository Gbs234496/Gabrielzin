using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Ugui : MonoBehaviour
{
    [Header("Pontos do Round (Por Jogador)")]
    public TextMeshProUGUI textoPontosP1;
    public TextMeshProUGUI textoPontosP2;

    [Header("Rounds Vencidos (Por Jogador)")]
    public TextMeshProUGUI textoRoundsP1;
    public TextMeshProUGUI textoRoundsP2;

    [Header("Textos Centrais Separados (Opcional)")]
    public TextMeshProUGUI textoPontosCentral;
    public TextMeshProUGUI textoRoundsCentral;

    [Header("Barras de Cooldown (Sliders)")]
    public Slider sliderCooldownP1;
    public Slider sliderCooldownP2;

    [Header("Barras de Cooldown (Image Fill)")]
    public Image barraCooldownP1;
    public Image barraCooldownP2;

    private void OnEnable()
    {
        PlayerOM.OnScoreUpdated += AtualizarPlacar;
        PlayerOM.OnCooldownUpdated += AtualizarCooldown;
    }

    private void OnDisable()
    {
        PlayerOM.OnScoreUpdated -= AtualizarPlacar;
        PlayerOM.OnCooldownUpdated -= AtualizarCooldown;
    }

    private void Start()
    {
        AtualizarPlacar();
    }

    public void AtualizarPlacar()
    {
        // 1. Textos individuais por Jogador (ex: P1 de um lado da tela, P2 do outro)
        if (textoPontosP1 != null) textoPontosP1.text = $"Pontos: {PlayerOM.P1PontosRound}";
        if (textoPontosP2 != null) textoPontosP2.text = $"Pontos: {PlayerOM.P2PontosRound}";

        if (textoRoundsP1 != null) textoRoundsP1.text = $"Rounds: {PlayerOM.P1RoundsVencidos}";
        if (textoRoundsP2 != null) textoRoundsP2.text = $"Rounds: {PlayerOM.P2RoundsVencidos}";

        // 2. Textos centrais divididos em 2 linhas/caixas distintas (se usar placar central)
        if (textoPontosCentral != null) 
            textoPontosCentral.text = $"PONTOS: {PlayerOM.P1PontosRound} x {PlayerOM.P2PontosRound}";

        if (textoRoundsCentral != null) 
            textoRoundsCentral.text = $"ROUNDS: {PlayerOM.P1RoundsVencidos} x {PlayerOM.P2RoundsVencidos}";
    }

    private void AtualizarCooldown(int playerID, float progresso)
    {
        if (playerID == 1)
        {
            if (sliderCooldownP1 != null) sliderCooldownP1.value = progresso;
            if (barraCooldownP1 != null) barraCooldownP1.fillAmount = progresso;
        }
        else if (playerID == 2)
        {
            if (sliderCooldownP2 != null) sliderCooldownP2.value = progresso;
            if (barraCooldownP2 != null) barraCooldownP2.fillAmount = progresso;
        }
    }
}