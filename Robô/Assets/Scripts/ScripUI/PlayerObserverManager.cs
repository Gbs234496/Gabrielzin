using System;
using UnityEngine;

public static class PlayerObserverManager
{
    public static event Action<int> OnMoedasAlteradas;
    

    public static void AdicionarMoeda(int coin)
    {
        
        Debug.Log($"[GERENCIADOR] Moeda somada! Novo total: {coin}. Disparando evento para a UI...");
        OnMoedasAlteradas?.Invoke(coin);
    }
}