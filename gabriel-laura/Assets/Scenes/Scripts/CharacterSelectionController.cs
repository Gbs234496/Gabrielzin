using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionController : MonoBehaviour
{
    [Header("Lista dos 5 Assets RoboData (Ordem dos Botões)")]
    public List<RoboData> listaRobos;

    [Header("Cores de Status")]
    public Color corEscolhendo = new Color(0.9f, 0.2f, 0.2f, 1f); // Vermelho
    public Color corPronto = new Color(0.2f, 0.9f, 0.2f, 1f);     // Verde

    [Header("UI Jogador 1")]
    public GameObject painelAtributosP1; // Objeto pai com Nome, Ícone e Stats (fica oculto até escolher)
    public TextMeshProUGUI textoNomeP1;
    public Image imgBolaP1;
    public TextMeshProUGUI textoStatsP1;
    public Image imgStatusP1;
    public TextMeshProUGUI textoStatusConfirmacaoP1;
    public Button[] botoesRobosP1;       // Os 5 botões de escolha do P1
    public Button botaoConfirmarP1;
    public Button botaoRefazerP1;

    [Header("UI Jogador 2")]
    public GameObject painelAtributosP2; // Objeto pai com Nome, Ícone e Stats (fica oculto até escolher)
    public TextMeshProUGUI textoNomeP2;
    public Image imgBolaP2;
    public TextMeshProUGUI textoStatsP2;
    public Image imgStatusP2;
    public TextMeshProUGUI textoStatusConfirmacaoP2;
    public Button[] botoesRobosP2;       // Os 5 botões de escolha do P2
    public Button botaoConfirmarP2;
    public Button botaoRefazerP2;

    private int _indexP1 = -1; // -1 indica que nenhum robô foi escolhido ainda
    private int _indexP2 = -1;
    private bool _p1Confirmou = false;
    private bool _p2Confirmou = false;

    private void Start()
    {
        AtualizarUIP1();
        AtualizarUIP2();
    }

    // --- AÇÕES DO JOGADOR 1 ---
    // Vincule os 5 botões do P1 nesta função passando 0, 1, 2, 3 e 4 no Inspector
    public void SelecionarRoboP1(int index)
    {
        if (_p1Confirmou || index < 0 || index >= listaRobos.Count) return;

        _indexP1 = index;
        AtualizarUIP1();
    }

    public void ConfirmarP1()
    {
        if (_indexP1 < 0 || _p1Confirmou) return;

        _p1Confirmou = true;
        AtualizarUIP1();
        ChecarInicioDePartida();
    }

    public void RefazerEscolhaP1()
    {
        // Só permite refazer se a partida ainda não tiver sido iniciada pelos dois
        if (_p1Confirmou && _p2Confirmou) return;

        _p1Confirmou = false;
        _indexP1 = -1; // Limpa a seleção para escolher novamente
        AtualizarUIP1();
    }

    // --- AÇÕES DO JOGADOR 2 ---
    // Vincule os 5 botões do P2 nesta função passando 0, 1, 2, 3 e 4 no Inspector
    public void SelecionarRoboP2(int index)
    {
        if (_p2Confirmou || index < 0 || index >= listaRobos.Count) return;

        _indexP2 = index;
        AtualizarUIP2();
    }

    public void ConfirmarP2()
    {
        if (_indexP2 < 0 || _p2Confirmou) return;

        _p2Confirmou = true;
        AtualizarUIP2();
        ChecarInicioDePartida();
    }

    public void RefazerEscolhaP2()
    {
        if (_p1Confirmou && _p2Confirmou) return;

        _p2Confirmou = false;
        _indexP2 = -1;
        AtualizarUIP2();
    }

    // --- ATUALIZAÇÃO VISUAL ---
    private void AtualizarUIP1()
    {
        bool temSelecao = _indexP1 >= 0 && _indexP1 < listaRobos.Count;

        if (painelAtributosP1 != null)
            painelAtributosP1.SetActive(temSelecao);

        if (temSelecao)
        {
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

        // Trava os 5 botões enquanto o jogador estiver confirmado
        if (botoesRobosP1 != null)
        {
            foreach (Button btn in botoesRobosP1)
            {
                if (btn != null) btn.interactable = !_p1Confirmou;
            }
        }

        // Botão Confirmar só aparece depois de clicar em um dos 5 robôs e antes de confirmar
        if (botaoConfirmarP1 != null)
            botaoConfirmarP1.gameObject.SetActive(temSelecao && !_p1Confirmou);

        // Botão Refazer só aparece depois de confirmar (enquanto aguarda o P2)
        if (botaoRefazerP1 != null)
            botaoRefazerP1.gameObject.SetActive(_p1Confirmou);

        if (imgStatusP1 != null)
            imgStatusP1.color = _p1Confirmou ? corPronto : corEscolhendo;

        if (textoStatusConfirmacaoP1 != null)
        {
            if (_p1Confirmou)
                textoStatusConfirmacaoP1.text = "<color=green>PRONTO!</color>";
            else if (temSelecao)
                textoStatusConfirmacaoP1.text = "Confirme sua escolha...";
            else
                textoStatusConfirmacaoP1.text = "Escolha uma joia...";
        }
    }

    private void AtualizarUIP2()
    {
        bool temSelecao = _indexP2 >= 0 && _indexP2 < listaRobos.Count;

        if (painelAtributosP2 != null)
            painelAtributosP2.SetActive(temSelecao);

        if (temSelecao)
        {
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

        if (botoesRobosP2 != null)
        {
            foreach (Button btn in botoesRobosP2)
            {
                if (btn != null) btn.interactable = !_p2Confirmou;
            }
        }

        if (botaoConfirmarP2 != null)
            botaoConfirmarP2.gameObject.SetActive(temSelecao && !_p2Confirmou);

        if (botaoRefazerP2 != null)
            botaoRefazerP2.gameObject.SetActive(_p2Confirmou);

        if (imgStatusP2 != null)
            imgStatusP2.color = _p2Confirmou ? corPronto : corEscolhendo;

        if (textoStatusConfirmacaoP2 != null)
        {
            if (_p2Confirmou)
                textoStatusConfirmacaoP2.text = "<color=green>PRONTO!</color>";
            else if (temSelecao)
                textoStatusConfirmacaoP2.text = "Confirme sua escolha...";
            else
                textoStatusConfirmacaoP2.text = "Escolha uma joia...";
        }
    }

    private void ChecarInicioDePartida()
    {
        if (_p1Confirmou && _p2Confirmou && _indexP1 >= 0 && _indexP2 >= 0)
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