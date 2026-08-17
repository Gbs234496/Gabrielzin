using System;
using UnityEngine;

public class ColetorDeMoedas : MonoBehaviour
{
    private int quantidadeMoedas = 0;
    public int QuantidadeMoedas => quantidadeMoedas;

    private void Start()
    {
        PlayerObserverManager.AdicionarMoeda(quantidadeMoedas);
    }

    private void OnTriggerEnter(Collider outro)
    {
        // RASTREADOR 1: O Unity detectou QUALQUER colisão?
        Debug.Log($"[FÍSICA] Encostei em algo chamado: {outro.name} | Tag real do objeto: '{outro.tag}'");

        if (outro.CompareTag("moeda"))
        {
            // RASTREADOR 2: A tag funcionou?
            Debug.Log("<color=cyan>[COLETA] Tag 'moeda' confirmada! Tentando avisar o Gerenciador...</color>");
            quantidadeMoedas++;
            PlayerObserverManager.AdicionarMoeda(quantidadeMoedas);
            Destroy(outro.gameObject);
        }
    }
}