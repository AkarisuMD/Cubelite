using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    InputManager inputManager;
    PlayerData playerData;

    [SerializeField] private PlayerAnimator playerAnimator;
    public ScPlayer scPlayer;
    private void Start()
    {
        inputManager = InputManager.Instance;
        SetPlayer(scPlayer);

        inputManager.OnJump += Jump;
        inputManager.OnJumpCanceled += StopJump;

        cam = CameraManager.Instance;
        playerAnimator = GetComponentInChildren<PlayerAnimator>();

        playerAnimator.Spawn();
    }

    #region Start Set Player

    [Header("PLAYER PREFAB")]
    [SerializeField] private SpriteRenderer spritePlayer;

    void SetPlayer(ScPlayer scPlayer)
    {
        playerData = PlayerData.Instance;
        playerData.player = gameObject;
        playerData.Canons = new List<GameObject>();
        playerData.CanonsRotation = new List<GameObject>();
        playerData.CanonsScripts = new List<CanonBehaviour>();
        playerData.CanonsPosition = new List<List<Vector3>>();
        playerData.CanonsPosRotation = new List<List<Quaternion>>();

        spritePlayer.sprite = scPlayer.sprite;
        playerData.HP = scPlayer.HP;
        playerData.Speed = scPlayer.Speed;
        playerData.JumpForce = scPlayer.JumpForce;
        playerData.ShootRate = scPlayer.ShootRate;
        playerData.Damage = scPlayer.Damage;
        playerData.Range = scPlayer.Range;
        playerData.Critical = scPlayer.Critical;
        playerData.XP = 0;
        playerData.Level = 1;
        playerData.XpForLevelUp = scPlayer.XPforLevel;
        playerData.MaxCanon = scPlayer.MaxCanonPossible;

        foreach (var pr in scPlayer.PositionAndRotationOfCanons)
        {
            playerData.CanonsPosition.Add(pr.Position);
            playerData.CanonsPosRotation.Add(pr.Rotation);
        }

        AddCanon(scPlayer.CanonDeDepart);
    }

    #endregion

    #region Health && XP

    bool OnlyDieOnce = false;
    [SerializeField] private AudioClip GetHitAudio;
    public void TakeDamage(int dmg, bool IsCrit)
    {
        playerData.HP -= dmg;
        CameraManager.Instance.ScreenShake();
        GetComponent<AudioSource>().PlayOneShot(GetHitAudio);

        if (playerData.HP <= 0 && !OnlyDieOnce)
        {
            OnlyDieOnce = true;
            Death();
        }
    }

    public void AddXP(int XP)
    {
        playerData.XP += XP;

        if (playerData.XpForLevelUp.Count > playerData.Level - 1 && playerData.XP >= playerData.XpForLevelUp[playerData.Level - 1])
        {
            PowerUpSystem.Instance.ActiveSystem();
        }
    }

    public GameObject particuledeath;
    void Death() => StartCoroutine(_Death());
    IEnumerator _Death()
    {
        playerAnimator.Death();
        Instantiate(particuledeath, transform.position, new quaternion(0, 0, 0, 0));
        Timer.Instance.Actif = false;
        yield return null;
        DeathScreen.Instance.Dead();
    }

    #endregion

    #region Update

    public Quaternion PlayerRotation => Quaternion.AngleAxis(transform.eulerAngles.z, transform.forward);

    [Space(10)] public float WantedRotation;

    private void Update()
    {
        if (!_active) return;
        _lastPosition = transform.position;

        RunCollisionChecks();

        CalculateWalk(); // Horizontal movement
        CalculateJumpApex(); // Affects fall speed, so calculate before gravity
        CalculateGravity(); // Vertical movement
        CalculateJump(); // Possibly overrides vertical

        MoveCharacter(); // Actu.ally perform the axis movement

        ControlCanon(); // Rotation of canon

        if (ownCamera) CameraFollow(); // Make the camera follow the player
    }

    /// <summary>
    /// Tarodev script
    /// </summary>

    public Vector3 Velocity { get; private set; }
    public bool JumpDown { get; private set; }
    public bool JumpingThisFrame { get; private set; }
    public bool LandingThisFrame { get; private set; }
    public Vector3 RawMovement { get; private set; }
    public bool Grounded => _colGround;

    private Vector3 _lastPosition;
    private float _currentHorizontalSpeed, _currentVerticalSpeed;

    // This is horrible, but for some reason colliders are not fully established when update starts...
    private bool _active;
    void Awake() => Invoke(nameof(Activate), 0.5f);
    void Activate() => _active = true;

    #endregion

    #region Collisions

    [Header("COLLISION")] [SerializeField] private Bounds _characterBounds;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private int _detectorCount = 3;
    [SerializeField] private float _detectionRayLength = 0.1f;
    [SerializeField] [Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f; // Prevents side detectors hitting the ground

    private RayRange _raysUp, _raysRight, _raysDown, _raysLeft;
    public RayRange _currentGround;
    public GroundType Ground  => playerData.Ground;
    public bool _colUp, _colRight, _colDown, _colLeft;
    [SerializeField] public bool _colGround, _colHead;

    private float _timeLeftGrounded;

    // We use these raycast checks for pre-collision information
    private void RunCollisionChecks()
    {
        // Generate ray ranges. 
        CalculateRayRanged();

        // Ground
        LandingThisFrame = false;

        // Check collision
        GroundCollision();

        //Type of gravtyChanger from the Map
        switch (playerData.gravityChangeType)
        {
            case GravityChangeType.Collision:
                CollisionGravity();
                break;
            case GravityChangeType.Bloc:
                NormalGravity();
                break;
            case GravityChangeType.Zone:
                NormalGravity();
                break;
            default:
                break;
        }
    }

    #region sous

    void CollisionGravity()
    {
        if (playerData.CanChangeGravity)
        {
            if (_colDown && _colLeft)
            {

            }
            else if (_colDown && _colRight)
            {

            }
            else if (_colUp && _colRight)
            {

            }
            else if (_colUp&& _colRight)
            {

            }
            else if (_colLeft)
            {
                switch (Ground)
                {
                    case GroundType.Down:
                        playerData.Ground = GroundType.Left;
                        WantedRotation = 270f;
                        break;
                    case GroundType.Right:
                        playerData.Ground = GroundType.Down;
                        WantedRotation = 0f;
                        break;
                    case GroundType.Left:
                        playerData.Ground = GroundType.Up;
                        WantedRotation = 180f;
                        break;
                    case GroundType.Up:
                        playerData.Ground = GroundType.Right;
                        WantedRotation = 90f;
                        break;
                    default:
                        break;
                }
            }
            else if (_colUp)
            {
                switch (Ground)
                {
                    case GroundType.Down:
                        playerData.Ground = GroundType.Up;
                        WantedRotation = 180f;
                        break;
                    case GroundType.Right:
                        playerData.Ground = GroundType.Left;
                        WantedRotation = 270f;
                        break;
                    case GroundType.Left:
                        playerData.Ground = GroundType.Right;
                        WantedRotation = 90f;
                        break;
                    case GroundType.Up:
                        playerData.Ground = GroundType.Down;
                        WantedRotation = 0f;
                        break;
                    default:
                        break;
                }
            }
            else if (_colRight)
            {
                switch (Ground)
                {
                    case GroundType.Down:
                        playerData.Ground = GroundType.Right;
                        WantedRotation = 90f;
                        break;
                    case GroundType.Right:
                        playerData.Ground = GroundType.Up;
                        WantedRotation = 180f;
                        break;
                    case GroundType.Left:
                        playerData.Ground = GroundType.Down;
                        WantedRotation = 0f;
                        break;
                    case GroundType.Up:
                        playerData.Ground = GroundType.Left;
                        WantedRotation = 270f;
                        break;
                    default:
                        break;
                }
            }
        }

        _currentGround = _raysDown;
    }
    void NormalGravity()
    {
        if (playerData.CanChangeGravity)
        {
            if (WantedRotation > 45 && WantedRotation < 135)
            {
                playerData.Ground = GroundType.Right;
            }
            else if (WantedRotation > 225 && WantedRotation < 315)
            {
                playerData.Ground = GroundType.Left;
            }
            else if (WantedRotation >= 135 && WantedRotation <= 225)
            {
                playerData.Ground = GroundType.Up;
            }
            else
            {
                playerData.Ground = GroundType.Down;
            }
        }

        _currentGround = _raysDown;
    }

    void GroundCollision()
    {
        switch (Ground)
        {
            case GroundType.Down:
                var groundedCheckD = RunDetection(_currentGround);
                if (_colGround && !groundedCheckD) _timeLeftGrounded = Time.time; // Only trigger when first leaving
                else if (!_colGround && groundedCheckD)
                {
                    _coyoteUsable = true; // Only trigger when first touching
                    LandingThisFrame = true;
                }
                _colGround = groundedCheckD;
                _colDown = _colGround;
                _colLeft = RunDetection(_raysLeft);
                _colRight = RunDetection(_raysRight);
                _colUp = RunDetection(_raysUp);
                break;
            case GroundType.Right:
                var groundedCheckR = RunDetection(_currentGround);
                if (_colGround && !groundedCheckR) _timeLeftGrounded = Time.time; // Only trigger when first leaving
                else if (!_colGround && groundedCheckR)
                {
                    _coyoteUsable = true; // Only trigger when first touching
                    LandingThisFrame = true;
                }
                _colGround = groundedCheckR;
                _colDown = _colGround;
                _colLeft = RunDetection(_raysRight);
                _colRight = RunDetection(_raysLeft);
                _colUp = RunDetection(_raysUp);
                break;
            case GroundType.Left:
                var groundedCheckL = RunDetection(_currentGround);
                if (_colGround && !groundedCheckL) _timeLeftGrounded = Time.time; // Only trigger when first leaving
                else if (!_colGround && groundedCheckL)
                {
                    _coyoteUsable = true; // Only trigger when first touching
                    LandingThisFrame = true;
                }
                _colGround = groundedCheckL;
                _colDown = _colGround;
                _colLeft = RunDetection(_raysRight);
                _colRight = RunDetection(_raysLeft);
                _colUp = RunDetection(_raysUp);
                break;
            case GroundType.Up:
                var groundedCheckU = RunDetection(_currentGround);
                if (_colGround && !groundedCheckU) _timeLeftGrounded = Time.time; // Only trigger when first leaving
                else if (!_colGround && groundedCheckU)
                {
                    _coyoteUsable = true; // Only trigger when first touching
                    LandingThisFrame = true;
                }
                _colGround = groundedCheckU;
                _colDown = _colGround;
                _colLeft = RunDetection(_raysLeft);
                _colRight = RunDetection(_raysRight);
                _colUp = RunDetection(_raysUp);
                break;
            default:
                break;
        }

        bool RunDetection(RayRange range)
        {
            return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, _detectionRayLength, _groundLayer));
        }
    }

    private void CalculateRayRanged()
    {
        // This is crying out for some kind of refactor. 
        var b = new Bounds(transform.position, _characterBounds.size);

        switch (Ground)
        {
            case GroundType.Down:
                _raysDown = new RayRange(new Vector2(b.min.x + _rayBuffer, b.min.y), new Vector2(b.max.x - _rayBuffer, b.min.y), Vector2.down);
                _raysUp = new RayRange(new Vector2(b.min.x + _rayBuffer, b.max.y), new Vector2(b.max.x - _rayBuffer, b.max.y), Vector2.up);
                _raysLeft = new RayRange(new Vector2(b.min.x, b.min.y + _rayBuffer), new Vector2(b.min.x, b.max.y - _rayBuffer), Vector2.left);
                _raysRight = new RayRange(new Vector2(b.max.x, b.min.y + _rayBuffer), new Vector2(b.max.x, b.max.y - _rayBuffer), Vector2.right);
                break;
            case GroundType.Right:
                _raysDown = new RayRange(new Vector2(b.max.x, b.min.y + _rayBuffer), new Vector2(b.max.x, b.max.y - _rayBuffer), Vector2.right);
                _raysUp = new RayRange(new Vector2(b.min.x, b.min.y + _rayBuffer), new Vector2(b.min.x, b.max.y - _rayBuffer), Vector2.left);
                _raysLeft = new RayRange(new Vector2(b.min.x + _rayBuffer, b.max.y), new Vector2(b.max.x - _rayBuffer, b.max.y), Vector2.up);
                _raysRight = new RayRange(new Vector2(b.min.x + _rayBuffer, b.min.y), new Vector2(b.max.x - _rayBuffer, b.min.y), Vector2.down);
                break;
            case GroundType.Left:
                _raysDown = new RayRange(new Vector2(b.min.x, b.min.y + _rayBuffer), new Vector2(b.min.x, b.max.y - _rayBuffer), Vector2.left);
                _raysUp = new RayRange(new Vector2(b.max.x, b.min.y + _rayBuffer), new Vector2(b.max.x, b.max.y - _rayBuffer), Vector2.right);
                _raysLeft = new RayRange(new Vector2(b.min.x + _rayBuffer, b.min.y), new Vector2(b.max.x - _rayBuffer, b.min.y), Vector2.down);
                _raysRight = new RayRange(new Vector2(b.min.x + _rayBuffer, b.max.y), new Vector2(b.max.x - _rayBuffer, b.max.y), Vector2.up);
                break;
            case GroundType.Up:
                _raysDown = new RayRange(new Vector2(b.min.x + _rayBuffer, b.max.y), new Vector2(b.max.x - _rayBuffer, b.max.y), Vector2.up);
                _raysUp = new RayRange(new Vector2(b.min.x + _rayBuffer, b.min.y), new Vector2(b.max.x - _rayBuffer, b.min.y), Vector2.down);
                _raysLeft = new RayRange(new Vector2(b.max.x, b.min.y + _rayBuffer), new Vector2(b.max.x, b.max.y - _rayBuffer), Vector2.right);
                _raysRight = new RayRange(new Vector2(b.min.x, b.min.y + _rayBuffer), new Vector2(b.min.x, b.max.y - _rayBuffer), Vector2.left);
                break;
            default:
                break;
        }
    }
    private void CalculateRayRangedGizmo()
    {
        // This is crying out for some kind of refactor. 
        var b = new Bounds(transform.position, _characterBounds.size);

        _raysDown = new RayRange(new Vector2(b.min.x + _rayBuffer, b.min.y), new Vector2(b.max.x - _rayBuffer, b.min.y), Vector2.down);
        _raysUp = new RayRange(new Vector2(b.min.x + _rayBuffer, b.max.y), new Vector2(b.max.x - _rayBuffer, b.max.y), Vector2.up);
        _raysLeft = new RayRange(new Vector2(b.min.x, b.min.y + _rayBuffer), new Vector2(b.min.x, b.max.y - _rayBuffer), Vector2.left);
        _raysRight = new RayRange(new Vector2(b.max.x, b.min.y + _rayBuffer), new Vector2(b.max.x, b.max.y - _rayBuffer), Vector2.right);
    }

    private IEnumerable<Vector2> EvaluateRayPositions(RayRange range)
    {
        for (var i = 0; i < _detectorCount; i++)
        {
            var t = (float)i / (_detectorCount - 1);
            yield return Vector2.Lerp(range.Start, range.End, t);
        }
    }

    private void OnDrawGizmos()
    {
        // Bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);

        // Rays
        if (!Application.isPlaying)
        {
            CalculateRayRangedGizmo();


            Gizmos.color = Color.blue;
            foreach (var point in EvaluateRayPositions(_raysUp))
            {
                Gizmos.DrawRay(point, _raysUp.Dir * _detectionRayLength);
            }

            Gizmos.color = Color.red;
            foreach (var point in EvaluateRayPositions(_raysRight))
            {
                Gizmos.DrawRay(point, _raysRight.Dir * _detectionRayLength);
            }

            Gizmos.color = Color.magenta;
            foreach (var point in EvaluateRayPositions(_raysDown))
            {
                Gizmos.DrawRay(point, _raysDown.Dir * _detectionRayLength);
            }

            Gizmos.color = Color.green;
            foreach (var point in EvaluateRayPositions(_raysLeft))
            {
                Gizmos.DrawRay(point, _raysLeft.Dir * _detectionRayLength);
            }
        }

        if (!Application.isPlaying) return;

        // Draw the future position. Handy for visualizing gravity
        Gizmos.color = Color.red;
        var move = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed) * Time.deltaTime;
        Gizmos.DrawWireCube(transform.position + move, _characterBounds.size);
    }

    #endregion

    #endregion

    #region Walk

    [Header("WALKING")] [SerializeField] private float _acceleration = 90;
    [SerializeField] private float _moveClamp = 13;
    [SerializeField] private float _deAcceleration = 60f;
    [SerializeField] private float _apexBonus = 2;

    private void CalculateWalk()
    {
        if (inputManager.GetPlayerMovement().magnitude != 0)
        {
            //
            // Set horizontal move speed

            switch (Ground)
            {
                case GroundType.Down:
                    _currentHorizontalSpeed += inputManager.GetPlayerMovement().x * _acceleration * Time.deltaTime;
                    break;
                case GroundType.Right:
                    _currentHorizontalSpeed += inputManager.GetPlayerMovement().y * _acceleration * Time.deltaTime;
                    break;
                case GroundType.Left:
                    _currentHorizontalSpeed += -inputManager.GetPlayerMovement().y * _acceleration * Time.deltaTime;
                    break;
                case GroundType.Up:
                    _currentHorizontalSpeed += -inputManager.GetPlayerMovement().x * _acceleration * Time.deltaTime;
                    break;
                default:
                    break;
            }

            // clamped by max frame movement
            _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp * playerData.Speed, _moveClamp * playerData.Speed);

            // Apply bonus at the apex of a jump
            var apexBonus = 0f;
            switch (Ground)
            {
                case GroundType.Down:
                    apexBonus = Mathf.Sign(inputManager.GetPlayerMovement().x) * _apexBonus * _apexPoint;
                    break;
                case GroundType.Right:
                    apexBonus = Mathf.Sign(inputManager.GetPlayerMovement().y) * _apexBonus * _apexPoint;
                    break;
                case GroundType.Left:
                    apexBonus = Mathf.Sign(inputManager.GetPlayerMovement().y) * _apexBonus * _apexPoint;
                    break;
                case GroundType.Up:
                    apexBonus = Mathf.Sign(inputManager.GetPlayerMovement().x) * _apexBonus * _apexPoint;
                    break;
                default:
                    break;
            }
            _currentHorizontalSpeed += apexBonus * Time.deltaTime;
        }
        else
        {
            // No input. Let's slow the character down
            _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
        }

        if (_currentHorizontalSpeed > 0 && _colRight || _currentHorizontalSpeed < 0 && _colLeft)
        {
            // Don't walk through walls
            _currentHorizontalSpeed = 0;
        }
    }

    #endregion

    #region Gravity

    [Header("GRAVITY")] [SerializeField] private float _fallClamp = -40f;
    [SerializeField] private float _minFallSpeed = 80f;
    [SerializeField] private float _maxFallSpeed = 120f;
    private float _fallSpeed;

    private void CalculateGravity()
    {
        if (_colGround)
        {
            // Move out of the ground
            if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;

            if (playerData.gravityChangeType == GravityChangeType.Collision && _colGround && _currentVerticalSpeed > 0)
            {
                _currentVerticalSpeed = 0;
            }
        }
        else
        {
            // Add downward force while ascending if we ended the jump early
            var fallSpeed = _endedJumpEarly && _currentVerticalSpeed > 0 ? _fallSpeed * _jumpEndEarlyGravityModifier : _fallSpeed;

            // Fall
            _currentVerticalSpeed -= fallSpeed * Time.deltaTime;

            // Clamp
            if (_currentVerticalSpeed < _fallClamp) _currentVerticalSpeed = _fallClamp;
        }
    }

    #endregion

    #region Jump

    [Header("JUMPING")] [SerializeField] private float _jumpHeight = 30;
    [SerializeField] private float _jumpApexThreshold = 10f;
    [SerializeField] private float _coyoteTimeThreshold = 0.1f;
    [SerializeField] private float _jumpBuffer = 0.1f;
    [SerializeField] private float _jumpEndEarlyGravityModifier = 3;
    private bool _coyoteUsable;
    private bool _endedJumpEarly = true;
    private float _apexPoint; // Becomes 1 at the apex of a jump
    private float _lastJumpPressed;
    private bool CanUseCoyote => _coyoteUsable && !_colGround && _timeLeftGrounded + _coyoteTimeThreshold > Time.time;
    private bool HasBufferedJump => _colGround && _lastJumpPressed + _jumpBuffer > Time.time;

    private void CalculateJumpApex()
    {
        if (!_colGround)
        {
            // Gets stronger the closer to the top of the jump
            _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
            _fallSpeed = Mathf.Lerp(_minFallSpeed, _maxFallSpeed, _apexPoint);
        }
        else
        {
            _apexPoint = 0;
        }
    }

    private void CalculateJump()
    {
        // Jump if: grounded or within coyote threshold || sufficient jump buffer
        if (JumpDown && CanUseCoyote)
        {
            _currentVerticalSpeed = _jumpHeight * playerData.JumpForce;
            _endedJumpEarly = false;
            _coyoteUsable = false;
            _timeLeftGrounded = float.MinValue;
            JumpingThisFrame = true;
        }
        else if (HasBufferedJump && !_colHead)
        {
            _currentVerticalSpeed = _jumpHeight * playerData.JumpForce;
            _endedJumpEarly = false;
            _coyoteUsable = false;
            _timeLeftGrounded = float.MinValue;
            JumpingThisFrame = true;
        }
        else
        {
            _lastJumpPressed = 0;
            JumpingThisFrame = false;
        }

        // End the jump early if button released
        if (!_colGround && !JumpDown && !_endedJumpEarly)
        {
            // _currentVerticalSpeed = 0;
            _endedJumpEarly = true;
        }
        
        if (_colHead)
        {
            if (_currentVerticalSpeed > 0) _currentVerticalSpeed = 0;
        }
    }
    void Jump()
    {
        JumpDown = true; 
        _lastJumpPressed = Time.time;
    }
    void StopJump()
    {
        JumpDown = false;
    }

    #endregion

    #region Move

    [Header("MOVE")]
    [SerializeField, Tooltip("Raising this value increases collision accuracy at the cost of performance.")]
    private int _freeColliderIterations = 10;

    // We cast our bounds before moving to avoid future collisions
    private void MoveCharacter()
    {
        var pos = transform.position;
        switch (Ground)
        {
            case GroundType.Down:
                RawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed); // Used externally
                break;
            case GroundType.Right:
                RawMovement = new Vector3(-_currentVerticalSpeed, _currentHorizontalSpeed);
                break;
            case GroundType.Left:
                RawMovement = new Vector3(_currentVerticalSpeed, -_currentHorizontalSpeed);
                break;
            case GroundType.Up:
                RawMovement = new Vector3(-_currentHorizontalSpeed, -_currentVerticalSpeed); 
                break;
            default:
                break;
        }

        var move = RawMovement * Time.deltaTime;
        var furthestPoint = pos + move;

        if (transform.eulerAngles.z != WantedRotation)
        {
            transform.rotation = Quaternion.Euler(0, 0, WantedRotation);
            _currentVerticalSpeed = 0;
            _currentHorizontalSpeed = -_currentHorizontalSpeed;
        }

        // check furthest movement. If nothing hit, move and don't do extra checks
        var hit = Physics2D.OverlapBox(furthestPoint, _characterBounds.size, 0, _groundLayer);
        if (!hit)
        {
            transform.position += move;
            return;
        }

        // otherwise increment away from current pos; see what closest position we can move to
        var positionToMoveTo = transform.position;
        for (int i = 1; i < _freeColliderIterations; i++)
        {
            // increment to check all but furthestPoint - we did that already
            var t = (float)i / _freeColliderIterations;
            var posToTry = Vector2.Lerp(pos, furthestPoint, t);

            if (Physics2D.OverlapBox(posToTry, _characterBounds.size, 0, _groundLayer))
            {
                transform.position = positionToMoveTo;

                // We've landed on a corner or hit our head on a ledge. Nudge the player gently
                if (i == 1)
                {
                    if (_currentVerticalSpeed < 0 || _currentVerticalSpeed > 0)
                    {
                        _currentVerticalSpeed = 0;
                        var dir = transform.position - hit.transform.position;
                        transform.position += dir.normalized * move.magnitude;
                    }
                }

                return;
            }

            positionToMoveTo = posToTry;
        }
    }

    #endregion

    #region Rotation

    public void NewRotation(float newRotation)
    {
        WantedRotation = newRotation;
    }

    #endregion

    #region Camera

    [Header("CAMERA")] [SerializeField] private CameraManager cam;
    [SerializeField] private bool ownCamera => cam.FollowPlayer;
    void CameraFollow()
    {
        cam.SetCameraTransform(new Vector3(transform.position.x, transform.position.y, 0f), new Quaternion(0, 0, 0, 0), 2f, 15f, SensCamera.Null);
    }

    #endregion

    #region Canon

    public void AddCanon(ScCanon scCanon)
    {
        if (playerData.Canons.Count >= playerData.MaxCanon) return;
        if (playerData.CanonHorlder == null)
        {
            GameObject CanonHorlder = new GameObject();
            CanonHorlder.transform.parent = transform;
            CanonHorlder.transform.localPosition = Vector3.zero;
            CanonHorlder.transform.rotation = new Quaternion(0, 0, 0, 0);
            playerData.CanonHorlder = CanonHorlder;
        }

        GameObject canonRot = new GameObject("CanonRotation");
        canonRot.transform.parent = playerData.CanonHorlder.transform;
        canonRot.transform.localPosition = Vector3.zero;
        canonRot.transform.rotation = new Quaternion(0, 0, 0, 0);

        GameObject canon = new GameObject("Canon " + scCanon.name);
        canon.transform.parent = canonRot.transform;
        canon.transform.localPosition = Vector3.zero;
        canon.transform.rotation = new Quaternion(0, 0, 0, 0);
        canon.AddComponent<SpriteRenderer>().sprite = scCanon.sprite;

        canon.AddComponent<AudioSource>();

        CanonBehaviour cb = canon.AddComponent(Type.GetType(scCanon.scriptCanon.name)) as CanonBehaviour;
        playerData.CanonsScripts.Add(cb);
        cb.scCanon = scCanon;
        cb.Player = true;

        playerData.CanonsRotation.Add(canonRot);
        playerData.Canons.Add(canon);

        int a = 0;
        foreach (var c in playerData.Canons)
        {
            StartCoroutine(GoToPosition(c, playerData.CanonsPosition[playerData.CanonsRotation.Count - 1][a], playerData.CanonsPosRotation[playerData.CanonsRotation.Count - 1][a]));
            a++;
        }

    }

    IEnumerator GoToPosition(GameObject canon, Vector3 pos, Quaternion rot)
    {
        var dist = canon.transform.localPosition - pos;
        while (dist.magnitude >= 0.01f)
        {
            canon.transform.localPosition = Vector3.Lerp(canon.transform.localPosition, pos, 2f * Time.deltaTime);
            canon.transform.localRotation = Quaternion.Lerp(canon.transform.localRotation, rot, 2.5f * Time.deltaTime);
            dist = canon.transform.localPosition - pos;
            yield return null;
        }
        canon.transform.localPosition = pos;
        canon.transform.localRotation = rot;
    }

    #region Direction

    void DirectionLook(GameObject canonRot, Vector2 Look)
    {
        float targetAngle = Mathf.Atan2(Look.x, -Look.y) * Mathf.Rad2Deg;
        canonRot.transform.rotation = Quaternion.Slerp(canonRot.transform.rotation, Quaternion.Euler(0, 0, 180 + targetAngle), 10 * Time.deltaTime);
    }
    void DirectionNoLook(GameObject canonRot, Vector2 Look)
    {
        float targetAngle = Mathf.Atan2(transform.up.x, -transform.up.y) * Mathf.Rad2Deg;
        canonRot.transform.rotation = Quaternion.Slerp(canonRot.transform.rotation, Quaternion.Euler(0, 0, 180 + targetAngle), 10 * Time.deltaTime);
    }
    void DirectionMouse(GameObject canonRot)
    {
        Vector2 LookMouse = inputManager.GetMousePosition();
        var dir = Camera.main.ScreenToWorldPoint(LookMouse) - canonRot.transform.position;
        float targetAngle = Mathf.Atan2(dir.x, -dir.y) * Mathf.Rad2Deg;
        canonRot.transform.rotation = Quaternion.Slerp(canonRot.transform.rotation, Quaternion.Euler(0, 0, 180 + targetAngle), 10 * Time.deltaTime);
    }

    void ControlCanon()
    {
        if (playerData.CanonHorlder != null)
        {
            playerData.CanonHorlder.transform.rotation = new Quaternion(0, 0, -transform.rotation.z, 0);

            if (playerData.CanonsRotation != null)
                foreach (var canonRot in playerData.CanonsRotation)
                    if (Cursor.visible == false)
                    {
                        Vector2 Look = inputManager.GetLook();
                        if (Look.magnitude > 0.2f)
                            DirectionLook(canonRot, Look);
                        else
                            DirectionNoLook(canonRot, Look);
                    }
                    else
                    {
                        DirectionMouse(canonRot);
                    }
        }

        if (inputManager.GetShoot())
        {
            foreach (var script in playerData.CanonsScripts)
            {
                script.Shoot();
            }
        }
    }

    #endregion

    #endregion
}

public struct RayRange
{
    public RayRange(Vector2 xy1 , Vector2 xy2 , Vector2 dir)
    {
        Start = xy1;
        End = xy2;
        Dir = dir;
    }

    public readonly Vector2 Start, End, Dir;
}

