using UnityEngine;
using System.Collections;

public class SplashSequence : MonoBehaviour
{
    IEnumerator Start()
    {
        
        yield return new WaitForSeconds(2f); // Espera 2 segundos
        
        
        GameManager.Instance.MenuPrincipal();
    }
}