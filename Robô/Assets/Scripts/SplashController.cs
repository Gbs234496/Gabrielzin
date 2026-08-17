using UnityEngine;
using System.Collections;

public class SplashController : MonoBehaviour
{
    IEnumerator Start()
    {
        // 1. Verificação de segurança: espera o final do frame para garantir que o GameManager existe
        yield return new WaitForEndOfFrame();

        if (GameManager.Instance != null)
        {
            // 2. Espera os 2 segundos reais
            yield return new WaitForSeconds(2f); 

            // 3. Muda o estado PRIMEIRO, depois carrega a cena (ordem mais segura)
            GameManager.Instance.MenuPrincipal();
        }
        else
        {
            Debug.LogError("O GameManager não foi encontrado! Você iniciou o jogo pela cena _Boot?");
        }
    }
}