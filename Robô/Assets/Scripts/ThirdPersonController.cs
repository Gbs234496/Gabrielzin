using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(StarterAssetsInputs))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player Identity")]
        public int PlayerID = 1;

        [Header("Câmera Split-Screen")]
        public Transform playerCameraTransform;

        [Header("Player Movement")]
        public float MoveSpeed = 2.0f;
        public float SprintSpeed = 5.335f;
        public float SpeedBoostPerCoin = 0.5f;

        [Range(0.0f, 0.3f)] public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10.0f;

        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;
        public float JumpTimeout = 0.50f;
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        public GameObject CinemachineCameraTarget;

        private float _currentSpeed;
        private float _animBlendValue;
        private float _desiredRotationY = 0.0f;
        private float _rotVelocityRef;
        private float _verticalVelocity;
        private float _maxFallSpeed = 53.0f;

        private float _jumpTimer;
        private float _fallTimer;

        private int _hashSpeed;
        private int _hashGrounded;
        private int _hashJump;
        private int _hashFreeFall;
        private int _hashMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _inputComponent;
#endif
        private Animator _animComp;
        private CharacterController _charController;
        private StarterAssetsInputs _inputData;
        private bool _animatorExists;

        private void Start()
        {
            _animatorExists = TryGetComponent(out _animComp);
            _charController = GetComponent<CharacterController>();
            _inputData = GetComponent<StarterAssetsInputs>();

#if ENABLE_INPUT_SYSTEM 
            _inputComponent = GetComponent<PlayerInput>();
            SetupControlScheme();
#endif
            InitializeAnimationHashes();

            _jumpTimer = JumpTimeout;
            _fallTimer = FallTimeout;

            if (playerCameraTransform == null && Camera.main != null)
            {
                playerCameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            _animatorExists = TryGetComponent(out _animComp);

#if ENABLE_INPUT_SYSTEM
            FetchInputValues();
#endif
            ApplyJumpAndGravity();
            EvaluateGroundedState();
            ProcessMovement();
        }

        public void ResetCameraRotation() => ResetCameraRotation(transform.eulerAngles.y);

        public void ResetCameraRotation(float targetAngle)
        {
            _desiredRotationY = targetAngle;
            if (CinemachineCameraTarget != null)
            {
                CinemachineCameraTarget.transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            }
        }

        private void OnFootstep(AnimationEvent animationEvent) { }

        private void OnLand(AnimationEvent animationEvent) { }

#if ENABLE_INPUT_SYSTEM
        private void SetupControlScheme()
        {
            if (_inputComponent == null) return;

            _inputComponent.defaultActionMap = "Player";
            _inputComponent.currentActionMap?.Enable();

            if (Keyboard.current != null)
            {
                string schemeName = (PlayerID == 1) ? "Player1_scheme" : "Player2_scheme";
                _inputComponent.SwitchCurrentControlScheme(schemeName, Keyboard.current);
            }
        }

        private void FetchInputValues()
        {
            if (_inputComponent?.actions == null || _inputData == null) return;

            var moveAction = _inputComponent.actions.FindAction("Move");
            var jumpAction = _inputComponent.actions.FindAction("Jump");
            var sprintAction = _inputComponent.actions.FindAction("Sprint");

            if (moveAction != null) _inputData.move = moveAction.ReadValue<Vector2>();
            if (jumpAction != null) _inputData.jump = jumpAction.IsPressed();
            if (sprintAction != null) _inputData.sprint = sprintAction.IsPressed();
        }
#endif

    private void OnTriggerEnter(Collider other)
{
    bool isCoin = other.CompareTag("Coin") || other.gameObject.name.Contains("Coin");
    if (isCoin)
    {
        // 1. Aplica o boost de velocidade
        MoveSpeed += SpeedBoostPerCoin;
        SprintSpeed += SpeedBoostPerCoin;

        // 2. Adiciona a moeda ao contador individual
        PlayerOM.AddCoin(PlayerID);

        // 3. Destrói o objeto no mapa
        Destroy(other.gameObject);
    }
}
        private void InitializeAnimationHashes()
        {
            _hashSpeed = Animator.StringToHash("Speed");
            _hashGrounded = Animator.StringToHash("Grounded");
            _hashJump = Animator.StringToHash("Jump");
            _hashFreeFall = Animator.StringToHash("FreeFall");
            _hashMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void EvaluateGroundedState()
        {
            Vector3 spherePos = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePos, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            if (_animatorExists) _animComp.SetBool(_hashGrounded, Grounded);
        }

       private void ProcessMovement()
{
    float targetSpeed = _inputData.sprint ? SprintSpeed : MoveSpeed;
    if (_inputData.move == Vector2.zero) targetSpeed = 0.0f;

    Vector3 currentHorizontalVel = new Vector3(_charController.velocity.x, 0.0f, _charController.velocity.z);
    float currentHorizontalSpeed = currentHorizontalVel.magnitude;
    float speedOffset = 0.1f;
    float inputMagnitude = _inputData.analogMovement ? _inputData.move.magnitude : 1f;

    if (Mathf.Abs(currentHorizontalSpeed - targetSpeed) > speedOffset)
    {
        _currentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
        _currentSpeed = Mathf.Round(_currentSpeed * 1000f) * 0.001f;
    }
    else
    {
        _currentSpeed = targetSpeed;
    }

    _animBlendValue = Mathf.Lerp(_animBlendValue, targetSpeed, Time.deltaTime * SpeedChangeRate);
    if (_animBlendValue < 0.01f) _animBlendValue = 0f;

    Vector3 inputDirection = new Vector3(_inputData.move.x, 0.0f, _inputData.move.y).normalized;

    if (_inputData.move != Vector2.zero)
    {
        // Pega apenas a rotação Y da câmera em World Space
        float cameraYaw = playerCameraTransform != null ? playerCameraTransform.rotation.eulerAngles.y : 0f;
        
        _desiredRotationY = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraYaw;

        // Suaviza a rotação do personagem em relação ao target desejado
        float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _desiredRotationY, ref _rotVelocityRef, RotationSmoothTime);
        transform.rotation = Quaternion.Euler(0.0f, smoothedAngle, 0.0f);
    }

    Vector3 moveDirection = Quaternion.Euler(0.0f, _desiredRotationY, 0.0f) * Vector3.forward;
    Vector3 velocityVector = moveDirection.normalized * (_currentSpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity * Time.deltaTime, 0.0f);
    
    _charController.Move(velocityVector);

    if (_animatorExists)
    {
        _animComp.SetFloat(_hashSpeed, _animBlendValue);
        _animComp.SetFloat(_hashMotionSpeed, inputMagnitude);
    }
}
        private void ApplyJumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimer = FallTimeout;
                
                if (_animatorExists)
                {
                    _animComp.SetBool(_hashJump, false);
                    _animComp.SetBool(_hashFreeFall, false);
                }
                
                if (_verticalVelocity < 0.0f) _verticalVelocity = -2f;

                if (_inputData.jump && _jumpTimer <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    if (_animatorExists) _animComp.SetBool(_hashJump, true);
                }
                
                if (_jumpTimer >= 0.0f) _jumpTimer -= Time.deltaTime;
            }
            else
            {
                _jumpTimer = JumpTimeout;
                
                if (_fallTimer >= 0.0f) _fallTimer -= Time.deltaTime;
                else if (_animatorExists) _animComp.SetBool(_hashFreeFall, true);
                
                _inputData.jump = false;
            }

            if (_verticalVelocity < _maxFallSpeed)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }
    }
}