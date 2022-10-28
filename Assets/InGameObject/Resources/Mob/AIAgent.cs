using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AIStats))]
public class AIAgent : MonoBehaviour
{
    public GameManager gm;
    public GameObject Visual;
    public PlayerData playerData => PlayerData.Instance;
    #region Stats
    public AIStats AIstats;
    
    public ScMob scMob;

    #endregion

    private void Awake()
    {
        gm = GameManager.Instance;
        Invoke(nameof(Activate), 0.5f);
    }
    public void Start()
    {
        #region Set stats

        AIstats = gameObject.GetComponent<AIStats>() ?? gameObject.AddComponent<AIStats>();

        AIstats.spriteRenderer.sprite = scMob.sprite;
        AIstats.spriteRenderer.sortingLayerName = "Mob";

        if (Timer.Instance.minute == 0)
        {
            AIstats.HP = 1;
        }
        else if (Timer.Instance.minute == 1)
        {
            AIstats.HP = scMob.HP;
        }
        else if (Timer.Instance.minute < 8)
        {
            AIstats.HP = Mathf.FloorToInt(scMob.HP * Timer.Instance.minute / 2);
        }
        else
        {
            AIstats.HP = scMob.HP * Timer.Instance.minute;
        }
        AIstats.Speed = scMob.Speed;
        AIstats.ShootRate = scMob.ShootRate;
        AIstats.Damage = scMob.Damage;
        AIstats.Range = scMob.Range;
        AIstats.RangeDetection = scMob.RangeDetection;
        AIstats.RangeDetectionShoot = scMob.RangeDetectionShoot;
        AIstats.XP = scMob.XPGiving;

        AIstats.CanonsRotation = new List<GameObject>();
        AIstats.Canons = new List<GameObject>();
        AIstats.CanonsScripts = new List<CanonBehaviour>();
        AIstats.CanonsPosition = new List<List<Vector3>>();
        AIstats.CanonsPosRotation = new List<List<Quaternion>>();

        if (AIstats.CanonHorlder == null)
        {
            AIstats.CanonHorlder = new GameObject();
            AIstats.CanonHorlder.transform.position = transform.position;
            AIstats.CanonHorlder.transform.rotation = transform.rotation;
            AIstats.CanonHorlder.transform.parent = transform;
        }

        foreach (var Canon in scMob.Canon)
        {
            GameObject CanonRot = new GameObject("CanonRotation");
            CanonRot.transform.parent = AIstats.CanonHorlder.transform;
            CanonRot.transform.localPosition = Vector3.zero;
            CanonRot.transform.rotation = new Quaternion(0, 0, 0, 0);

            GameObject canon = new GameObject("Mob Canon " + Canon.name);
            canon.transform.parent = CanonRot.transform;
            canon.transform.localPosition = Vector3.zero;
            canon.transform.rotation = new Quaternion(0, 0, 0, 0);

            canon.AddComponent<SpriteRenderer>().sprite = Canon.sprite;
            CanonBehaviour cb = canon.AddComponent(Type.GetType(Canon.scriptCanon.name)) as CanonBehaviour;
            AIstats.CanonsScripts.Add(cb);
            cb.scCanon = Canon;
            cb.Player = false;

            AIstats.CanonsRotation.Add(CanonRot);
            AIstats.Canons.Add(canon);
        }

        foreach (var pr in scMob.PositionAndRotationOfCanons)
        {
            AIstats.CanonsPosition.Add(pr.Position);
            AIstats.CanonsPosRotation.Add(pr.Rotation);
        }

        int a = 0;
        foreach (var c in AIstats.Canons)
        {
            StartCoroutine(GoToPosition(c, AIstats.CanonsPosition[AIstats.CanonsRotation.Count - 1][a], AIstats.CanonsPosRotation[AIstats.CanonsRotation.Count - 1][a]));
            a++;
        }

        _Start();

        #endregion
    }
    public virtual void _Start() { }

    public void DestroySelf()
    {
        for (int i = 0; i < AIstats.XP; i++)
        {
            GameObject xp = PoolXP.Instance.GetXp();
            xp.transform.position = new Vector2(transform.position.x + UnityEngine.Random.Range(-1f, 1f),
                                                transform.position.y + UnityEngine.Random.Range(-1f, 1f));
            xp.SetActive(true);
        }

        // mettre anim mort ici
        
        Destroy(gameObject);
    }

    IEnumerator GoToPosition(GameObject canon, Vector3 pos, Quaternion rot)
    {
        var dist = canon.transform.localPosition - pos;
        while (dist.magnitude >= 0.1f)
        {
            canon.transform.localPosition = Vector3.Lerp(canon.transform.localPosition, pos, 2f * Time.deltaTime);
            canon.transform.localRotation = Quaternion.Lerp(canon.transform.localRotation, rot, 2.5f * Time.deltaTime);
            dist = canon.transform.localPosition - pos;
            yield return null;
        }
        canon.transform.localPosition = pos;
        canon.transform.localRotation = rot;
    }

    public void HPModifier(int HP) { StartCoroutine(HPModifier_(HP)); }
    IEnumerator HPModifier_(int HP)
    {
        yield return null;
        AIstats.HP -= HP;
        if (AIstats.HP <= 0)
        {
            DestroySelf();
        }
    }
    
    #region Physics


    public Vector3 Velocity { get; private set; }
    public bool JumpDown { get; private set; }
    public bool JumpingThisFrame { get; private set; }
    public bool LandingThisFrame { get; private set; }
    public Vector3 RawMovement { get; private set; }
    public bool Grounded => _colGround;

    public Vector3 _lastPosition;
    public float _currentHorizontalSpeed, _currentVerticalSpeed;

    // This is horrible, but for some reason colliders are not fully established when update starts...
    public bool _active;
    void Activate() => _active = true;


    #region Collisions

    [Header("COLLISION")][SerializeField] private Bounds _characterBounds;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private int _detectorCount = 3;
    [SerializeField] private float _detectionRayLength = 0.1f;
    [SerializeField][Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f; // Prevents side detectors hitting the ground

    private RayRange _raysUp, _raysRight, _raysDown, _raysLeft, _raysJumpingLeft, _raysJumpingRight;
    public RayRange _currentGround;
    public GroundType Ground => playerData.Ground;
    public bool _colUp, _colRight, _colDown, _colLeft, _colJumpLeft, _colJumpRight;
    [SerializeField] public bool _colGround, _colHead;

    private float _timeLeftGrounded;

    // We use these raycast checks for pre-collision information
    public void RunCollisionChecks()
    {
        // Generate ray ranges. 
        CalculateRayRanged();

        // Ground
        LandingThisFrame = false;

        _currentGround = _raysDown;

        // Check collision
        GroundCollision();
    }

    #region sous

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

        if (!Application.isPlaying)
        {
            CalculateRayRanged();
            CalculateRayRangedWall();

            Gizmos.color = Color.blue;
            foreach (var point in EvaluateRayPositions(_raysUp))
            {
                Gizmos.DrawRay(point, _raysUp.Dir * _detectionRayLength);
            }

            Gizmos.color = Color.blue;
            foreach (var point in EvaluateRayPositions(_raysRight))
            {
                Gizmos.DrawRay(point, _raysRight.Dir * _detectionRayLength);
            }

            Gizmos.color = Color.blue;
            foreach (var point in EvaluateRayPositions(_raysDown))
            {
                Gizmos.DrawRay(point, _raysDown.Dir * _detectionRayLength);
            }

            Gizmos.color = Color.blue;
            foreach (var point in EvaluateRayPositions(_raysLeft))
            {
                Gizmos.DrawRay(point, _raysLeft.Dir * _detectionRayLength);
            }

            Gizmos.color = Color.red;
            foreach (var point in EvaluateRayPositionsJump(_raysJumpingLeft))
            {
                Gizmos.DrawRay(point, _raysJumpingLeft.Dir * _detectionRayLengthJump);
            }

            Gizmos.color = Color.green;
            foreach (var point in EvaluateRayPositionsJump(_raysJumpingRight))
            {
                Gizmos.DrawRay(point, _raysJumpingRight.Dir * _detectionRayLengthJump);
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

    [Header("WALKING")][SerializeField] private float _acceleration = 90;
    [SerializeField] private float _moveClamp = 13;
    [SerializeField] private float _deAcceleration = 60f;
    [SerializeField] private float _apexBonus = 2;

    public void CalculateWalk()
    {
        var dir = playerData.player.transform.position - transform.position;
        if (dir.magnitude >= 4f)
        {
            //
            // Set horizontal move speed

            switch (Ground)
            {
                case GroundType.Down:
                    _currentHorizontalSpeed += Mathf.Clamp(dir.x, -1f, 1f) * _acceleration * Time.deltaTime;
                    break;
                case GroundType.Right:
                    _currentHorizontalSpeed += Mathf.Clamp(dir.y, -1f, 1f) * _acceleration * Time.deltaTime;
                    break;
                case GroundType.Left:
                    _currentHorizontalSpeed += Mathf.Clamp(-dir.y, -1f, 1f)  * _acceleration * Time.deltaTime;
                    break;
                case GroundType.Up:
                    _currentHorizontalSpeed += Mathf.Clamp(-dir.x, -1f, 1f)  * _acceleration * Time.deltaTime;
                    break;
                default:
                    break;
            }

            // clamped by max frame movement
            _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp * AIstats.Speed, _moveClamp * AIstats.Speed);

            // Apply bonus at the apex of a jump
            var apexBonus = 0f;
            switch (Ground)
            {
                case GroundType.Down:
                    apexBonus = Mathf.Sign(dir.x) * _apexBonus * _apexPoint;
                    break;
                case GroundType.Right:
                    apexBonus = Mathf.Sign(dir.y) * _apexBonus * _apexPoint;
                    break;
                case GroundType.Left:
                    apexBonus = Mathf.Sign(dir.y) * _apexBonus * _apexPoint;
                    break;
                case GroundType.Up:
                    apexBonus = Mathf.Sign(dir.x) * _apexBonus * _apexPoint;
                    break;
                default:
                    break;
            }
            _currentHorizontalSpeed += apexBonus * Time.deltaTime;
        }

        // Let's slow the character down
        switch (playerData.Ground)
        {
            case GroundType.Down:
                if (playerData.player.transform.position.x - transform.position.x <= 2)
                    _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
                break;
            case GroundType.Right:
                if (playerData.player.transform.position.y - transform.position.y <= 2)
                    _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
                break;
            case GroundType.Left:
                if (playerData.player.transform.position.y - transform.position.y <= 2)
                    _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
                break;
            case GroundType.Up:
                if (playerData.player.transform.position.x - transform.position.x <= 2)
                    _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
                break;
            default:
                break;
        }

        if (_currentHorizontalSpeed > 0 && _colRight || _currentHorizontalSpeed < 0 && _colLeft)
        {
            // Don't walk through walls
            _currentHorizontalSpeed = 0;
        }
    }

    #endregion

    #region Gravity

    [Header("GRAVITY")][SerializeField] private float _fallClamp = -40f;
    [SerializeField] private float _minFallSpeed = 80f;
    [SerializeField] private float _maxFallSpeed = 120f;
    private float _fallSpeed;

    public void CalculateGravity()
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

    [Header("JUMPING")][SerializeField] private float _jumpHeight = 30;
    [SerializeField] private float _jumpApexThreshold = 10f;
    [SerializeField] private float _coyoteTimeThreshold = 0.1f;
    [SerializeField] private float _jumpBuffer = 0.1f;
    [SerializeField] private float _jumpEndEarlyGravityModifier = 3;
    private bool _coyoteUsable;
    private bool _endedJumpEarly = true;
    private float _apexPoint; // Becomes 1 at the apex of a jump
    private float _lastJumpPressed;
    [SerializeField] private bool CanUseCoyote => _coyoteUsable && !_colGround && _timeLeftGrounded + _coyoteTimeThreshold > Time.time;
    private bool HasBufferedJump => _colGround && _lastJumpPressed + _jumpBuffer > Time.time;

    public void CalculateJumpApex()
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

    public void CalculateJump()
    {
        CalculateRayRangedWall();

        _colJumpLeft = RunDetection(_raysJumpingLeft);
        _colJumpRight = RunDetection(_raysJumpingRight);

        // Jump if: detecte a wall on his direction && grounded
        if ((_colJumpLeft || _colJumpRight) && _colGround)
        {
            _currentVerticalSpeed = _jumpHeight * playerData.JumpForce;
            _endedJumpEarly = false;
            _coyoteUsable = false;
            _timeLeftGrounded = float.MinValue;
            JumpingThisFrame = true;
        }
        else
        {
            JumpingThisFrame = false;
        }

        if (_colHead)
        {
            if (_currentVerticalSpeed > 0) _currentVerticalSpeed = 0;
        }

        bool RunDetection(RayRange range)
        {
            return EvaluateRayPositionsJump(range).Any(point => Physics2D.Raycast(point, range.Dir, _detectionRayLengthJump, _groundLayer));
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


    [SerializeField] private int _detectorCountWall = 1;
    [SerializeField] private float _detectionRayLengthJump = 0.3f;
    private void CalculateRayRangedWall()
    {
        var b = new Bounds(transform.position, _characterBounds.size);

        switch (Ground)
        {
            case GroundType.Down:
                    _raysJumpingLeft = new RayRange(new Vector2(b.min.x, b.min.y + _rayBuffer), new Vector2(b.min.x , b.max.y - _rayBuffer), Vector2.left);
                    _raysJumpingRight = new RayRange(new Vector2(b.max.x, b.min.y + _rayBuffer), new Vector2(b.max.x , b.max.y - _rayBuffer), Vector2.right);
                break;
            case GroundType.Right:
                    _raysJumpingLeft = new RayRange(new Vector2(b.min.x + _rayBuffer, b.max.y), new Vector2(b.max.x - _rayBuffer, b.max.y), Vector2.up);
                    _raysJumpingRight = new RayRange(new Vector2(b.min.x + _rayBuffer, b.min.y), new Vector2(b.max.x - _rayBuffer, b.min.y), Vector2.down);
                break;
            case GroundType.Left:
                    _raysJumpingLeft = new RayRange(new Vector2(b.min.x + _rayBuffer, b.min.y), new Vector2(b.max.x - _rayBuffer, b.min.y), Vector2.down);
                    _raysJumpingRight = new RayRange(new Vector2(b.min.x + _rayBuffer, b.max.y), new Vector2(b.max.x - _rayBuffer, b.max.y), Vector2.up);
                break;
            case GroundType.Up:
                    _raysJumpingLeft = new RayRange(new Vector2(b.max.x, b.min.y + _rayBuffer), new Vector2(b.max.x, b.max.y - _rayBuffer), Vector2.right);
                    _raysJumpingRight = new RayRange(new Vector2(b.min.x, b.min.y + _rayBuffer), new Vector2(b.min.x, b.max.y - _rayBuffer), Vector2.left);
                break;
            default:
                break;
        }
    }
    private IEnumerable<Vector2> EvaluateRayPositionsJump(RayRange range)
    {
        for (var i = 0; i < _detectorCountWall; i++)
        {
            var t = (float)i / (_detectorCountWall - 1);
            yield return Vector2.Lerp(range.Start, range.End, t);
        }
    }

    #endregion

    #region Move

    [Header("MOVE")]
    [SerializeField, Tooltip("Raising this value increases collision accuracy at the cost of performance.")]
    private int _freeColliderIterations = 10;

    // We cast our bounds before moving to avoid future collisions
    public void MoveCharacter()
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

        switch (playerData.Ground)
        {
            case GroundType.Down:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case GroundType.Right:
                transform.rotation = Quaternion.Euler(0, 0, 270);
                break;
            case GroundType.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case GroundType.Up:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            default:
                break;
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

    #endregion
}