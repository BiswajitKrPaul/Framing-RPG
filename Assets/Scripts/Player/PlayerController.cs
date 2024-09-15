using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player {
    public class PlayerController : MonoBehaviour {
        #region PlayerInputActions

        private PlayerInputActions _playerInputActions;
        private InputAction _moveInputActions;

        #endregion


        #region MovementVectors

        private Vector2 _movementDirection;
        public bool IsAttacking { get; private set; }
        public float XInput => _movementDirection.x;
        public float YInput => _movementDirection.y;
        public Animator PlayerAnimator => animator;
        public Rigidbody2D PlayerRigidBody2D => rb;

        #endregion


        #region Player States Info

        [Header("Player States Info")] [SerializeField]
        private StateMachine stateMachine;

        public PlayerGroundState groundState;
        public PlayerMoveState moveState;

        #endregion


        #region Components

        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D rb;

        #endregion

        #region Weapons

        public WeaponType PlayerCurrentWeapon { get; private set; } = WeaponType.None;
        private bool _hasCurrentWeapon = true;

        #endregion

        private void Awake() {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Player.Enable();
            _moveInputActions = _playerInputActions.Player.Movement;
            _playerInputActions.Player.Attack.performed += OnAttackOnPerformed;
            _playerInputActions.Player.Weapon.performed += OnWeaponEquippedPerformed;
            groundState.SetUp(this, stateMachine, PlayerConstants.Idle);
            moveState.SetUp(this, stateMachine, PlayerConstants.IsWalking);
        }

        private void OnWeaponEquippedPerformed(InputAction.CallbackContext context) {
            if (!context.performed) return;
            _hasCurrentWeapon = !_hasCurrentWeapon;
            PlayerCurrentWeapon = _hasCurrentWeapon ? WeaponType.None : WeaponType.Hoe;
            // animator.SetInteger(PlayerConstants.WeaponType, (int)PlayerCurrentWeapon);
        }

        private void Start() {
            stateMachine.Initialize(groundState);
        }


        private void OnAttackOnPerformed(InputAction.CallbackContext context) {
            if (context.performed)
                IsAttacking = !IsAttacking;
        }

        private void Update() {
            stateMachine.CurrentState.Process();
        }

        private void FixedUpdate() {
            stateMachine.CurrentState.PhysicsProcess();
            GetInputs();
        }


        private void GetInputs() {
            _movementDirection = _moveInputActions.ReadValue<Vector2>();
        }

        public void SetVelocity(Vector2 velocity) {
            rb.velocity = velocity;
        }
    }
}