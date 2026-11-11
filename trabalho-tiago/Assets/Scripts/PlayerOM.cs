using UnityEngine;
using System;

public static class PlayerOM
{
    // Dados dos Robôs
    public static RoboData RoboP1;
    public static RoboData RoboP2;
    public static bool P1UsarCorSecundaria;
    public static bool P2UsarCorSecundaria;

    // Moedas / Estrelas Coletadas
    public static int MoedasP1 { get; private set; }
    public static int MoedasP2 { get; private set; }

    // Pontos do Round Atual (0 a 2)
    public static int P1PontosRound { get; private set; }
    public static int P2PontosRound { get; private set; }

    // Rounds Vencidos na Partida (0 a 2)
    public static int P1RoundsVencidos { get; private set; }
    public static int P2RoundsVencidos { get; private set; }

    // Vencedor Final
    public static int UltimoVencedorID { get; private set; }
    public static string UltimoRoboVencedorNome { get; private set; }

    // Eventos para atualização da UI
    public static event Action OnScoreUpdated;
    public static event Action<int, float> OnCooldownUpdated;

    // --- MÉTODOS DE COLETA (MOEDAS E ESTRELAS) ---
    public static void AddCoin(int playerID, int quantidade = 1)
    {
        if (playerID == 1) MoedasP1 += quantidade;
        else if (playerID == 2) MoedasP2 += quantidade;

        OnScoreUpdated?.Invoke();
    }

    // Suporte para o script Pickup.cs
    public static void AddStar(int playerID, int quantidade = 1)
    {
        AddCoin(playerID, quantidade);
    }

    public static int GetCoins(int playerID)
    {
        return playerID == 1 ? MoedasP1 : MoedasP2;
    }

    // --- MÉTODOS DE PONTUAÇÃO E ROUNDS ---
    public static void AddPonto(int playerID)
    {
        if (playerID == 1) P1PontosRound++;
        else if (playerID == 2) P2PontosRound++;

        OnScoreUpdated?.Invoke();
    }

    public static void AddRoundWin(int playerID)
    {
        if (playerID == 1) P1RoundsVencidos++;
        else if (playerID == 2) P2RoundsVencidos++;

        P1PontosRound = 0;
        P2PontosRound = 0;
        MoedasP1 = 0;
        MoedasP2 = 0;

        OnScoreUpdated?.Invoke();
    }

    public static int GetRoundWins(int playerID)
    {
        return playerID == 1 ? P1RoundsVencidos : P2RoundsVencidos;
    }

    // --- RESET DE DADOS ---
    public static void ResetCoins()
    {
        MoedasP1 = 0;
        MoedasP2 = 0;
        OnScoreUpdated?.Invoke();
    }

    public static void ResetScores()
    {
        P1PontosRound = 0;
        P2PontosRound = 0;
        P1RoundsVencidos = 0;
        P2RoundsVencidos = 0;
        MoedasP1 = 0;
        MoedasP2 = 0;

        OnScoreUpdated?.Invoke();
    }

    public static void ResetGameData()
    {
        P1UsarCorSecundaria = false;
        P2UsarCorSecundaria = false;
        ResetScores();
    }

    // --- VITORIA DA PARTIDA ---
    public static void TriggerMatchWin(int vencedorID, string nomeRobo)
    {
        UltimoVencedorID = vencedorID;
        UltimoRoboVencedorNome = nomeRobo;
    }

    // --- NOTIFICAÇÃO DE COOLDOWN ---
    public static void NotifyCooldown(int playerID, float progresso)
    {
        OnCooldownUpdated?.Invoke(playerID, progresso);
    }
}