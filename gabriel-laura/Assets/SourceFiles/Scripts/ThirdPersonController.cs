using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(StarterAssetsInputs))]
    [RequireComponent(typeof(PlayerInput))]
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Identificação do Jogador")]
        public int PlayerID = 1;
        public RoboData DadosRobo;

        [Header("Câmera")]
        public Transform playerCameraTransform;
        public GameObject CinemachineCameraTarget;

        [Header("Ajustes Físicos e Movimento")]
        public float SpeedBoostPerCoin = 0.5f;
        public float ForceBoostPerCoin = 2.0f;
        [Range(0.0f, 0.3f)] public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10.0f;
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;
        public float JumpTimeout = 0.50f;
        public float FallTimeout = 0.15f;

        [Header("Áudio")]
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        // Verificação de chão oculta no Inspector, preservando nomes e valores serializados
        [HideInInspector] public bool Grounded = true;
        [HideInInspector] public float GroundedOffset = -0.14f;
        [HideInInspector] public float GroundedRadius = 0.28f;
        [HideInInspector] public LayerMask GroundLayers;

        public float MoveSpeed { get; private set; } = 4.0f;
        public float SprintSpeed { get; private set; } = 6.0f;

        // Parâmetros internos de empurrão e cooldown
        private float _forcaEmpurraoBase = 12.0f;
        private float _raioEmpurrao = 3.5f;
        private float _tempoCooldownEmpurrao = 3.0f;
        private float _timerCooldown = 0f;
        private bool _podeEmpurrar = true;
        private Vector3 _impactoExterno = Vector3.zero;

        // Estados internos de movimentação e física
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // Hashes de animação
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        // Referências de componentes
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private PlayerInput _playerInput;
        private bool _hasAnimator;

        #region Ciclo de Vida Unity

        private void Start()
        {
            ObterComponentes();
            ConfigurarInputsECamera();
            AssignAnimationIDs();

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            if (DadosRobo != null)
            {
                bool usarSecundaria = PlayerID == 2 && PlayerOM.P2UsarCorSecundaria;
                InicializarComRoboData(DadosRobo, usarSecundaria);
            }
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            ProcessarCooldown();
            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            AutoAlignCamera();
        }

        #endregion

        #region Inicialização

        private void ObterComponentes()
        {
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void ConfigurarInputsECamera()
        {
            if (_playerInput != null && _playerInput.currentActionMap != null)
                _playerInput.currentActionMap.Enable();

            if (playerCameraTransform == null && Camera.main != null)
                playerCameraTransform = Camera.main.transform;
        }

        public void InicializarComRoboData(RoboData data, bool usarCorSecundaria)
        {
            if (data == null) return;

            AplicarAtributosRobo(data);
            AplicarVisualRobo(data, usarCorSecundaria);
        }

        private void AplicarAtributosRobo(RoboData data)
        {
            MoveSpeed = data.VelocidadeInicial;
            SprintSpeed = data.VelocidadeInicial * 1.5f;
            _forcaEmpurraoBase = data.ForcaEmpurraoBase;
            _raioEmpurrao = data.RaioEmpurrao;
            _tempoCooldownEmpurrao = data.TempoCooldownEmpurrao;

            transform.localScale = Vector3.one * data.EscalaTamanho;
        }

        private void AplicarVisualRobo(RoboData data, bool usarCorSecundaria)
        {
            Renderer renderizador = GetComponentInChildren<Renderer>();
            if (renderizador == null) return;

            if (data.MaterialBase != null)
                renderizador.material = new Material(data.MaterialBase);

            Material mat = renderizador.material;

            if (data.TexturaBola != null)
                mat.mainTexture = data.TexturaBola;

            mat.color = usarCorSecundaria ? data.CorDuplicado : Color.white;
        }

        #endregion

        #region Combate e Coletáveis

        public void OnPush(InputValue value)
        {
            if (!value.isPressed || !_podeEmpurrar) return;
            ExecutarEmpurrao();
        }

        private void ExecutarEmpurrao()
        {
            _podeEmpurrar = false;
            _timerCooldown = _tempoCooldownEmpurrao;

            Collider[] hits = Physics.OverlapSphere(transform.position, _raioEmpurrao);
            for (int i = 0; i < hits.Length; i++)
            {
                AplicarEmpurraoNoAlvo(hits[i]);
            }
        }

        private void AplicarEmpurraoNoAlvo(Collider hit)
        {
            ThirdPersonController oponente = hit.GetComponent<ThirdPersonController>()
                                             ?? hit.GetComponentInParent<ThirdPersonController>();

            if (oponente == null || oponente == this) return;

            float distancia = Vector3.Distance(transform.position, oponente.transform.position);
            float fatorProximidade = Mathf.Clamp01(1.0f - (distancia / _raioEmpurrao));

            int moedasColetadas = PlayerOM.GetCoins(PlayerID);
            float forcaTotal = (_forcaEmpurraoBase + (moedasColetadas * ForceBoostPerCoin)) * fatorProximidade;
            Vector3 direcaoArco = (transform.forward + Vector3.up * 0.8f).normalized;

            oponente.ReceberEmpurrao(direcaoArco * forcaTotal);
        }

        public void ReceberEmpurrao(Vector3 vetorForca)
        {
            _impactoExterno += vetorForca;
            _verticalVelocity = vetorForca.y;
        }

        private void ProcessarCooldown()
        {
            if (_podeEmpurrar) return;

            _timerCooldown -= Time.deltaTime;
            float progresso = Mathf.Clamp01(1.0f - (_timerCooldown / _tempoCooldownEmpurrao));
            PlayerOM.NotifyCooldown(PlayerID, progresso);

            if (_timerCooldown <= 0f)
            {
                _podeEmpurrar = true;
                PlayerOM.NotifyCooldown(PlayerID, 1.0f);
            }
        }

        public void AplicarBonusVelocidadeMoeda()
        {
            MoveSpeed += SpeedBoostPerCoin;
            SprintSpeed += SpeedBoostPerCoin;
        }

        public void ColetarMoeda()
        {
            AplicarBonusVelocidadeMoeda();
            PlayerOM.AddCoin(PlayerID);
        }

        public void ApplySpeedBoost()
        {
            ColetarMoeda();
        }

        #endregion

        #region Movimentação, Física e Câmera

        private void Move()
        {
            Vector2 moveInput = GetMoveInput();
            bool isSprinting = GetSprintInput();

            float targetSpeed = (moveInput == Vector2.zero) ? 0.0f : (isSprinting ? SprintSpeed : MoveSpeed);
            float inputMagnitude = (_input != null && _input.analogMovement) ? moveInput.magnitude : 1f;

            AtualizarVelocidadeHorizontal(targetSpeed, inputMagnitude);
            AtualizarRotacaoPersonagem(moveInput);
            DissiparImpactoExterno();

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            Vector3 movimentoFinal = (targetDirection.normalized * (_speed * Time.deltaTime))
                                   + (new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime)
                                   + (_impactoExterno * Time.deltaTime);

            _controller.Move(movimentoFinal);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void AtualizarVelocidadeHorizontal(float targetSpeed, float inputMagnitude)
        {
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;
        }

        private void AtualizarRotacaoPersonagem(Vector2 moveInput)
        {
            if (moveInput == Vector2.zero) return;

            Vector3 inputDirection = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;
            float cameraYaw = playerCameraTransform != null ? playerCameraTransform.eulerAngles.y : 0f;

            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraYaw;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        private void DissiparImpactoExterno()
        {
            _impactoExterno = (_impactoExterno.magnitude > 0.2f)
                ? Vector3.Lerp(_impactoExterno, Vector3.zero, Time.deltaTime * 5f)
                : Vector3.zero;
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
                _animator.SetBool(_animIDGrounded, Grounded);
        }

        private void JumpAndGravity()
        {
            bool jumpPressed = GetJumpInput();

            if (Grounded)
                ProcessarEstadoNoChao(jumpPressed);
            else
                ProcessarEstadoNoAr();

            if (_verticalVelocity < _terminalVelocity)
                _verticalVelocity += Gravity * Time.deltaTime;
        }

        private void ProcessarEstadoNoChao(bool jumpPressed)
        {
            _fallTimeoutDelta = FallTimeout;

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            if (_verticalVelocity < 0.0f)
                _verticalVelocity = -2f;

            if (jumpPressed && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                if (_hasAnimator) _animator.SetBool(_animIDJump, true);
            }

            if (_jumpTimeoutDelta >= 0.0f)
                _jumpTimeoutDelta -= Time.deltaTime;
        }

        private void ProcessarEstadoNoAr()
        {
            _jumpTimeoutDelta = JumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
                _fallTimeoutDelta -= Time.deltaTime;
            else if (_hasAnimator)
                _animator.SetBool(_animIDFreeFall, true);

            if (_input != null)
                _input.jump = false;
        }

        private void AutoAlignCamera()
        {
            if (CinemachineCameraTarget == null || GetMoveInput() == Vector2.zero) return;

            Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            CinemachineCameraTarget.transform.rotation = Quaternion.Slerp(
                CinemachineCameraTarget.transform.rotation,
                targetRotation,
                Time.deltaTime * 3.0f
            );
        }

        #endregion

        #region Leitura de Inputs e Eventos de Animação

        private InputAction BuscarAcao(string nomeAcao)
        {
            if (_playerInput == null || _playerInput.actions == null) return null;
            return _playerInput.actions.FindAction(nomeAcao);
        }

        private Vector2 GetMoveInput()
        {
            InputAction acao = BuscarAcao("Move");
            if (acao != null) return acao.ReadValue<Vector2>();
            return _input != null ? _input.move : Vector2.zero;
        }

        private bool GetSprintInput()
        {
            InputAction acao = BuscarAcao("Sprint");
            if (acao != null) return acao.IsPressed();
            return _input != null && _input.sprint;
        }

        private bool GetJumpInput()
        {
            InputAction acao = BuscarAcao("Jump");
            if (acao != null) return acao.IsPressed();
            return _input != null && _input.jump;
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight <= 0.5f || FootstepAudioClips.Length == 0) return;

            int index = Random.Range(0, FootstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight <= 0.5f) return;

            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }

        #endregion
    }
}