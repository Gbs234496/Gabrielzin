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

        [Header("Verificação de Chão")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        [Header("Áudio")]
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        // Atributos dinâmicos do Robô
        public float MoveSpeed { get; private set; } = 4.0f;
        public float SprintSpeed { get; private set; } = 6.0f;
        private float _forcaEmpurraoBase = 12.0f;
        private float _raioEmpurrao = 3.5f;
        private float _tempoCooldownEmpurrao = 3.0f;

        // Cooldown e Impulso de Empurrão
        private float _timerCooldown = 0f;
        private bool _podeEmpurrar = true;
        private Vector3 _impactoExterno = Vector3.zero;

        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private PlayerInput _playerInput;

        private bool _hasAnimator;

        private void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            _playerInput = GetComponent<PlayerInput>();

            if (_playerInput != null && _playerInput.currentActionMap != null)
            {
                _playerInput.currentActionMap.Enable();
            }

            AssignAnimationIDs();

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            if (playerCameraTransform == null && Camera.main != null)
            {
                playerCameraTransform = Camera.main.transform;
            }

            // Caso o robô já tenha um RoboData atribuído diretamente no Inspector
            if (DadosRobo != null)
            {
                InicializarComRoboData(DadosRobo, PlayerID == 2 && PlayerOM.P2UsarCorSecundaria);
            }
        }

        public void InicializarComRoboData(RoboData data, bool usarCorSecundaria)
        {
            if (data == null) return;

            // 1. Aplica Atributos de Gameplay
            MoveSpeed = data.VelocidadeInicial;
            SprintSpeed = data.VelocidadeInicial * 1.5f; // Proporção da velocidade de corrida
            _forcaEmpurraoBase = data.ForcaEmpurraoBase;
            _raioEmpurrao = data.RaioEmpurrao;
            _tempoCooldownEmpurrao = data.TempoCooldownEmpurrao;

            // 2. Aplica Escala de Tamanho do Robô
            transform.localScale = Vector3.one * data.EscalaTamanho;

            // 3. Aplica Material, Textura da Bola e Cor de Diferenciação
            Renderer renderizador = GetComponentInChildren<Renderer>();

            if (renderizador != null)
            {
                // Se houver um material base definido no RoboData, aplica primeiro
                if (data.MaterialBase != null)
                {
                    renderizador.material = new Material(data.MaterialBase);
                }

                Material mat = renderizador.material;

                // Aplica a textura da bola escolhida
                if (data.TexturaBola != null)
                {
                    mat.mainTexture = data.TexturaBola;
                }

                // Se ambos escolherem o mesmo robô, aplica a cor definida no ScriptableObject
                if (usarCorSecundaria)
                {
                    mat.color = data.CorDuplicado;
                }
                else
                {
                    mat.color = Color.white; // Mantém a cor e textura originais
                }
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

        // --- SISTEMA DE EMPURRÃO EM ARCO ---
        public void OnPush(InputValue value)
        {
            if (value.isPressed && _podeEmpurrar)
            {
                ExecutarEmpurrao();
            }
        }

        private void ExecutarEmpurrao()
        {
            _podeEmpurrar = false;
            _timerCooldown = _tempoCooldownEmpurrao;

            // Encontra robôs atingíveis dentro do raio
            Collider[] hits = Physics.OverlapSphere(transform.position, _raioEmpurrao);
            foreach (Collider hit in hits)
            {
                ThirdPersonController oponente = hit.GetComponent<ThirdPersonController>();
                if (oponente == null) oponente = hit.GetComponentInParent<ThirdPersonController>();

                if (oponente != null && oponente != this)
                {
                    float distancia = Vector3.Distance(transform.position, oponente.transform.position);

                    // Quanto mais perto, maior a proporção da força (0.0 a 1.0)
                    float fatorProximidade = Mathf.Clamp01(1.0f - (distancia / _raioEmpurrao));

                    // Força Total = (Base + Bônus de Moedas) * Proximidade
                    int moedasColetadas = PlayerOM.GetCoins(PlayerID);
                    float forcaTotal = (_forcaEmpurraoBase + (moedasColetadas * ForceBoostPerCoin)) * fatorProximidade;

                    // Vetor de arremesso em arco: Frente do Robô + Direção para Cima
                    Vector3 direcaoArco = (transform.forward + Vector3.up * 0.8f).normalized;

                    // Aplica o empurrão no oponente
                    oponente.ReceberEmpurrao(direcaoArco * forcaTotal);
                }
            }
        }

        public void ReceberEmpurrao(Vector3 vetorForca)
        {
            _impactoExterno += vetorForca;
            _verticalVelocity = vetorForca.y; // Eleva o jogador verticalmente
        }

        private void ProcessarCooldown()
        {
            if (!_podeEmpurrar)
            {
                _timerCooldown -= Time.deltaTime;
                float progresso = Mathf.Clamp01(1.0f - (_timerCooldown / _tempoCooldownEmpurrao));

                PlayerOM.NotifyCooldown(PlayerID, progresso);

                if (_timerCooldown <= 0f)
                {
                    _podeEmpurrar = true;
                    PlayerOM.NotifyCooldown(PlayerID, 1.0f);
                }
            }
        }

        // --- SISTEMA DE MOEDAS E VELOCIDADE ---
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

        private void Move()
        {
            Vector2 moveInput = GetMoveInput();
            bool isSprinting = GetSprintInput();

            float targetSpeed = isSprinting ? SprintSpeed : MoveSpeed;
            if (moveInput == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = (_input != null && _input.analogMovement) ? moveInput.magnitude : 1f;

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

            Vector3 inputDirection = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;

            if (moveInput != Vector2.zero)
            {
                float cameraYaw = playerCameraTransform != null ? playerCameraTransform.eulerAngles.y : 0f;
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraYaw;

                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // Dissipação gradual da força do empurrão recebido
            if (_impactoExterno.magnitude > 0.2f)
            {
                _impactoExterno = Vector3.Lerp(_impactoExterno, Vector3.zero, Time.deltaTime * 5f);
            }
            else
            {
                _impactoExterno = Vector3.zero;
            }

            Vector3 movimentoFinal = (targetDirection.normalized * (_speed * Time.deltaTime)) +
                                     new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime +
                                     _impactoExterno * Time.deltaTime;

            _controller.Move(movimentoFinal);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void AutoAlignCamera()
        {
            if (CinemachineCameraTarget == null) return;

            Vector2 moveInput = GetMoveInput();

            if (moveInput != Vector2.zero)
            {
                Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
                CinemachineCameraTarget.transform.rotation = Quaternion.Slerp(
                    CinemachineCameraTarget.transform.rotation,
                    targetRotation,
                    Time.deltaTime * 3.0f
                );
            }
        }

        private void JumpAndGravity()
        {
            bool jumpPressed = GetJumpInput();

            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }
                if (_verticalVelocity < 0.0f) _verticalVelocity = -2f;

                if (jumpPressed && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    if (_hasAnimator) _animator.SetBool(_animIDJump, true);
                }

                if (_jumpTimeoutDelta >= 0.0f) _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;
                if (_fallTimeoutDelta >= 0.0f) _fallTimeoutDelta -= Time.deltaTime;
                else if (_hasAnimator) _animator.SetBool(_animIDFreeFall, true);

                if (_input != null) _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity) _verticalVelocity += Gravity * Time.deltaTime;
        }

        private Vector2 GetMoveInput()
        {
            if (_playerInput != null && _playerInput.actions != null && _playerInput.actions.FindAction("Move") != null)
            {
                return _playerInput.actions["Move"].ReadValue<Vector2>();
            }
            return _input != null ? _input.move : Vector2.zero;
        }

        private bool GetSprintInput()
        {
            if (_playerInput != null && _playerInput.actions != null && _playerInput.actions.FindAction("Sprint") != null)
            {
                return _playerInput.actions["Sprint"].IsPressed();
            }
            return _input != null && _input.sprint;
        }

        private bool GetJumpInput()
        {
            if (_playerInput != null && _playerInput.actions != null && _playerInput.actions.FindAction("Jump") != null)
            {
                return _playerInput.actions["Jump"].IsPressed();
            }
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
            if (animationEvent.animatorClipInfo.weight > 0.5f && FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}