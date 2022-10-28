using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobAnimation : MonoBehaviour
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

    private AIAgent _mob;
    private bool _playerGrounded;
    [SerializeField] private ParticleSystem.MinMaxGradient _currentGradient;
    private Vector2 _movement;

    void Awake()
    {
        _mob = GetComponentInParent<AIAgent>();
    }

    void Update()
    {
        if (_mob == null) return;

        // Flip the sprite
        if (_mob._currentHorizontalSpeed != 0) transform.localScale = new Vector3(_mob._currentHorizontalSpeed > 0 ? 1 : -1, 1, 1);

        // Lean while running
        Vector3 targetRotVector = Vector3.zero;
        switch (_mob.Ground)
        {
            case GroundType.Down:
                targetRotVector = new Vector3(0, 0, Mathf.Lerp(-_maxTilt + transform.eulerAngles.z, _maxTilt + transform.eulerAngles.z, Mathf.InverseLerp(-1, 1, _mob._currentHorizontalSpeed)));
                break;
            case GroundType.Right:
                targetRotVector = new Vector3(0, 0, Mathf.Lerp(-_maxTilt + transform.eulerAngles.z, _maxTilt + transform.eulerAngles.z, Mathf.InverseLerp(-1, 1, _mob._currentHorizontalSpeed)));
                break;
            case GroundType.Left:
                targetRotVector = new Vector3(0, 0, Mathf.Lerp(-_maxTilt + transform.eulerAngles.z, _maxTilt + transform.eulerAngles.z, Mathf.InverseLerp(-1, 1, _mob._currentHorizontalSpeed)));
                break;
            case GroundType.Up:
                targetRotVector = new Vector3(0, 0, Mathf.Lerp(-_maxTilt + transform.eulerAngles.z, _maxTilt + transform.eulerAngles.z, Mathf.InverseLerp(-1, 1, _mob._currentHorizontalSpeed)));
                break;
            default:
                break;
        }
        _anim.transform.rotation = Quaternion.RotateTowards(_anim.transform.rotation, Quaternion.Euler(targetRotVector), _tiltSpeed * Time.deltaTime);

        // Speed up idle while running
        switch (_mob.Ground)
        {
            case GroundType.Down:
                _anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, Mathf.Abs(_mob._currentHorizontalSpeed)));
                break;
            case GroundType.Right:
                _anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, Mathf.Abs(_mob._currentHorizontalSpeed)));
                break;
            case GroundType.Left:
                _anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, Mathf.Abs(_mob._currentHorizontalSpeed)));
                break;
            case GroundType.Up:
                _anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, Mathf.Abs(_mob._currentHorizontalSpeed)));
                break;
            default:
                break;
        }

        // Splat
        if (_mob.LandingThisFrame)
        {
            _anim.SetTrigger(GroundedKey);
            _source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
        }

        // Jump effects
        if (_mob.JumpingThisFrame)
        {
            _anim.SetTrigger(JumpKey);
            _anim.ResetTrigger(GroundedKey);

            // Only play particles when grounded (avoid coyote)
            if (_mob.Grounded)
            {
                SetColor(_jumpParticles);
                SetColor(_launchParticles);
                _jumpParticles.Play();
            }
        }

        // Play landing effects and begin ground movement effects
        if (!_playerGrounded && _mob.Grounded)
        {
            _playerGrounded = true;
            _moveParticles.Play();
            _landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, _maxParticleFallSpeed, _movement.y);
            SetColor(_landParticles);
            _landParticles.Play();
        }
        else if (_playerGrounded && !_mob.Grounded)
        {
            _playerGrounded = false;
            _moveParticles.Stop();
        }

        // Detect ground color

        _movement = _mob.RawMovement; // Previous frame movement is more valuable
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

    #endregion
}
