using System.Drawing;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This is a pretty filthy script. I was just arbitrarily adding to it as I went.
/// You won't find any programming prowess here.
/// This is a supplementary script to help with effects and animation. Basically a juice factory.
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private AudioSource _source;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private ParticleSystem _jumpParticles, _launchParticles;
    [SerializeField] private ParticleSystem _moveParticles, _landParticles;
    [SerializeField] private AudioClip[] _footsteps;
    [SerializeField] private float _maxTilt = .1f;
    [SerializeField] private float _tiltSpeed = 1;
    [SerializeField, Range(1f, 3f)] private float _maxIdleSpeed = 2;
    [SerializeField] private float _maxParticleFallSpeed = -40;

    private InputManager inputManager;
    private PlayerBehaviour _player;
    private bool _playerGrounded;
    [SerializeField] private ParticleSystem.MinMaxGradient _currentGradient;
    private Vector2 _movement;

    void Awake()
    {
        _player = GetComponentInParent<PlayerBehaviour>();
        inputManager = InputManager.Instance;
    }

    void Update()
    {
        if (_player == null) return;

        // Flip the sprite
        switch (_player.Ground)
        {
            case GroundType.Down:
                if (inputManager.GetPlayerMovement().x != 0) transform.localScale = new Vector3(inputManager.GetPlayerMovement().x > 0 ? 1 : -1, 1, 1);
                break;
            case GroundType.Right:
                if (inputManager.GetPlayerMovement().y != 0) transform.localScale = new Vector3(inputManager.GetPlayerMovement().y < 0 ? 1 : -1, 1, 1);
                break;
            case GroundType.Left:
                if (inputManager.GetPlayerMovement().y != 0) transform.localScale = new Vector3(inputManager.GetPlayerMovement().y < 0 ? 1 : -1, 1, 1);
                break;
            case GroundType.Up:
                if (inputManager.GetPlayerMovement().x != 0) transform.localScale = new Vector3(inputManager.GetPlayerMovement().x < 0 ? 1 : -1, 1, 1);
                break;
            default:
                break;
        }

        // Lean while running
        Vector3 targetRotVector = Vector3.zero;
        switch (_player.Ground)
        {
            case GroundType.Down:
                targetRotVector = new Vector3(0, 0, Mathf.Lerp(-_maxTilt + transform.eulerAngles.z, _maxTilt + transform.eulerAngles.z, Mathf.InverseLerp(-1, 1, inputManager.GetPlayerMovement().x)));
                break;
            case GroundType.Right:
                targetRotVector = new Vector3(0, 0, Mathf.Lerp(-_maxTilt + transform.eulerAngles.z, _maxTilt + transform.eulerAngles.z, Mathf.InverseLerp(-1, 1, inputManager.GetPlayerMovement().y)));
                break;
            case GroundType.Left:
                targetRotVector = new Vector3(0, 0, Mathf.Lerp(-_maxTilt + transform.eulerAngles.z, _maxTilt + transform.eulerAngles.z, Mathf.InverseLerp(-1, 1, -inputManager.GetPlayerMovement().y)));
                break;
            case GroundType.Up:
                targetRotVector = new Vector3(0, 0, Mathf.Lerp(-_maxTilt + transform.eulerAngles.z, _maxTilt + transform.eulerAngles.z, Mathf.InverseLerp(-1, 1, -inputManager.GetPlayerMovement().x)));
                break;
            default:
                break;
        }
        _anim.transform.rotation = Quaternion.RotateTowards(_anim.transform.rotation, Quaternion.Euler(targetRotVector), _tiltSpeed * Time.deltaTime);

        // Speed up idle while running
        switch (_player.Ground)
        {
            case GroundType.Down:
                _anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, Mathf.Abs(inputManager.GetPlayerMovement().x)));
                break;
            case GroundType.Right:
                _anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, Mathf.Abs(-inputManager.GetPlayerMovement().y)));
                break;
            case GroundType.Left:
                _anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, Mathf.Abs(inputManager.GetPlayerMovement().y)));
                break;
            case GroundType.Up:
                _anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, Mathf.Abs(-inputManager.GetPlayerMovement().x)));
                break;
            default:
                break;
        }

        // Splat
        if (_player.LandingThisFrame)
        {
            _anim.SetTrigger(GroundedKey);
            _source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
        }

        // Jump effects
        if (_player.JumpingThisFrame)
        {
            _anim.SetTrigger(JumpKey);
            _anim.ResetTrigger(GroundedKey);

            // Only play particles when grounded (avoid coyote)
            if (_player.Grounded)
            {
                SetColor(_jumpParticles);
                SetColor(_launchParticles);
                _jumpParticles.Play();
            }
        }

        // Play landing effects and begin ground movement effects
        if (!_playerGrounded && _player.Grounded)
        {
            _playerGrounded = true;
            _moveParticles.Play();
            _landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, _maxParticleFallSpeed, _movement.y);
            SetColor(_landParticles);
            _landParticles.Play();
        }
        else if (_playerGrounded && !_player.Grounded)
        {
            _playerGrounded = false;
            _moveParticles.Stop();
        }

        // Detect ground color

        _movement = _player.RawMovement; // Previous frame movement is more valuable
    }

    public void Spawn()
    {
        _anim.SetTrigger(SpawnKey);
    }

    public void Death()
    {
        _anim.SetTrigger(DeathKey);
    }

    private void OnDisable()
    {
        _moveParticles.Stop();
    }

    private void OnEnable()
    {
        _moveParticles.Play();
    }

    void SetColor(ParticleSystem ps)
    {
        var main = ps.main;
        main.startColor = _currentGradient;
    }

    #region Animation Keys

    private static readonly int GroundedKey = Animator.StringToHash("Grounded");
    private static readonly int IdleSpeedKey = Animator.StringToHash("IdleSpeed");
    private static readonly int JumpKey = Animator.StringToHash("Jump");
    private static readonly int SpawnKey = Animator.StringToHash("Spawn");
    private static readonly int DeathKey = Animator.StringToHash("Death");

    #endregion
}