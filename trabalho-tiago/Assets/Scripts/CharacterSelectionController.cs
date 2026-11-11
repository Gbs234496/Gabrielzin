using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionController : MonoBehaviour
{
    [Header("Lista dos Assets RoboData")]
    public List<RoboData> listaRobos;

    [Header("Cores de Status")]
    public Color corEscolhendo = new Color(0.9f, 0.2f, 0.2f, 1f); // Vermelho
    public Color corPronto = new Color(0.2f, 0.9f, 0.2f, 1f);    // Verde

    [Header("UI Jogador 1")]
    public TextMeshProUGUI textoNomeP1;
    public Image imgBolaP1;
    public Image imgStatusP1; // Indicador visual do P1
    public TextMeshProUGUI textoStatsP1;
    public TextMeshProUGUI textoStatusConfirmacaoP1;

    [Header("UI Jogador 2")]
    public TextMeshProUGUI textoNomeP2;
    public Image imgBolaP2;
    public Image imgStatusP2; // Indicador visual do P2
    public TextMeshProUGUI textoStatsP2;
    public TextMeshProUGUI textoStatusConfirmacaoP2;

    private int _indexP1 = 0;
    private int _indexP2 = 0;
    private bool _p1Confirmou = false;
    private bool _p2Confirmou = false;

    private void Start()
    {
        AtualizarUIP1();
        AtualizarUIP2();
        AtualizarStatusVisualP1();
        AtualizarStatusVisualP2();
    }

    // --- BOTAOS DO JOGADOR 1 ---
    public void ProximoRoboP1()
    {
        if (_p1Confirmou || listaRobos.Count == 0) return;
        _indexP1 = (_indexP1 + 1) % listaRobos.Count;
        AtualizarUIP1();
    }

    public void AnteriorRoboP1()
    {
        if (_p1Confirmou || listaRobos.Count == 0) return;
        _indexP1 = (_indexP1 - 1 + listaRobos.Count) % listaRobos.Count;
        AtualizarUIP1();
    }

    public void ConfirmarP1()
    {
        _p1Confirmou = !_p1Confirmou;
        AtualizarStatusVisualP1();
        ChecarInicioDePartida();
    }

    // --- BOTOES DO JOGADOR 2 ---
    public void ProximoRoboP2()
    {
        if (_p2Confirmou || listaRobos.Count == 0) return;
        _indexP2 = (_indexP2 + 1) % listaRobos.Count;
        AtualizarUIP2();
    }

    public void AnteriorRoboP2()
    {
        if (_p2Confirmou || listaRobos.Count == 0) return;
        _indexP2 = (_indexP2 - 1 + listaRobos.Count) % listaRobos.Count;
        AtualizarUIP2();
    }

    public void ConfirmarP2()
    {
        _p2Confirmou = !_p2Confirmou;
        AtualizarStatusVisualP2();
        ChecarInicioDePartida();
    }

    private void AtualizarUIP1()
    {
        if (listaRobos.Count == 0) return;
        RoboData robo = listaRobos[_indexP1];
        
        if (textoNomeP1 != null) 
            textoNomeP1.text = robo.NomeRobo;

        if (imgBolaP1 != null && robo.IconeBola != null)
        {
            imgBolaP1.sprite = robo.IconeBola;
            imgBolaP1.enabled = true;
        }

        if (textoStatsP1 != null)
            textoStatsP1.text = $"Velocidade: {robo.VelocidadeInicial}\nForça: {robo.ForcaEmpurraoBase}\nAlcance: {robo.RaioEmpurrao}";
    }

    private void AtualizarUIP2()
    {
        if (listaRobos.Count == 0) return;
        RoboData robo = listaRobos[_indexP2];
        
        if (textoNomeP2 != null) 
            textoNomeP2.text = robo.NomeRobo;

        if (imgBolaP2 != null && robo.IconeBola != null)
        {
            imgBolaP2.sprite = robo.IconeBola;
            imgBolaP2.enabled = true;
        }

        if (textoStatsP2 != null)
            textoStatsP2.text = $"Velocidade: {robo.VelocidadeInicial}\nForça: {robo.ForcaEmpurraoBase}\nAlcance: {robo.RaioEmpurrao}";
    }

    private void AtualizarStatusVisualP1()
    {
        if (textoStatusConfirmacaoP1 != null)
            textoStatusConfirmacaoP1.text = _p1Confirmou ? "<color=green>PRONTO!</color>" : "Escolhendo...";

        if (imgStatusP1 != null)
        {
            imgStatusP1.color = _p1Confirmou ? corPronto : corEscolhendo;
        }
    }

    private void AtualizarStatusVisualP2()
    {
        if (textoStatusConfirmacaoP2 != null)
            textoStatusConfirmacaoP2.text = _p2Confirmou ? "<color=green>PRONTO!</color>" : "Escolhendo...";

        if (imgStatusP2 != null)
        {
            imgStatusP2.color = _p2Confirmou ? corPronto : corEscolhendo;
        }
    }

    private void ChecarInicioDePartida()
    {
        if (_p1Confirmou && _p2Confirmou)
        {
            PlayerOM.ResetGameData();
            PlayerOM.RoboP1 = listaRobos[_indexP1];
            PlayerOM.RoboP2 = listaRobos[_indexP2];

            if (_indexP1 == _indexP2)
            {
                PlayerOM.P2UsarCorSecundaria = true;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CarregarJogo();
            }
        }
    }
}