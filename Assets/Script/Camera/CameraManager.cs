using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    Camera _camera;
    public bool FollowPlayer;
    public SensCamera cameraSens;

    private void Awake() => _camera = GetComponent<Camera>();
    public void SetCameraTransform(Vector3 pos, quaternion rot, float speed, float Size, SensCamera sensCamera)
    {
        transform.position = Vector3.Lerp(transform.position, pos, speed * Time.deltaTime);
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, Size, speed * Time.deltaTime);

        if (sensCamera == SensCamera.Null)
        {
            switch (cameraSens)
            {
                case SensCamera.Down:
                    transform.rotation = Quaternion.Lerp(transform.rotation, rot, speed * Time.deltaTime);
                    break;
                case SensCamera.Up:
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rot.value.z + 180), speed * Time.deltaTime);
                    break;
                case SensCamera.Left:
                    transform.rotation = Quaternion.Lerp(transform.rotation, rot, speed * Time.deltaTime);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rot.value.z - 90), speed * Time.deltaTime);
                    break;
                case SensCamera.Right:
                    transform.rotation = Quaternion.Lerp(transform.rotation, rot, speed * Time.deltaTime);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rot.value.z + 90), speed * Time.deltaTime);
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (sensCamera)
            {
                case SensCamera.Down:
                    transform.rotation = Quaternion.Lerp(transform.rotation, rot, speed * Time.deltaTime);
                    break;
                case SensCamera.Up:
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rot.value.z + 180), speed * Time.deltaTime);
                    break;
                case SensCamera.Left:
                    transform.rotation = Quaternion.Lerp(transform.rotation, rot, speed * Time.deltaTime);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rot.value.z - 90), speed * Time.deltaTime);
                    break;
                case SensCamera.Right:
                    transform.rotation = Quaternion.Lerp(transform.rotation, rot, speed * Time.deltaTime);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rot.value.z + 90), speed * Time.deltaTime);
                    break;
                default:
                    break;
            }
            cameraSens = sensCamera;
        }
    }

    private void Update() => _ScreenShake();
    [Space(5)] [Header("ScreenShake")]
    public float shakeDuration = 0;
    public float shakeMagnitude = 0.1f;
    public void ScreenShake() => shakeDuration = 0.5f;
    public void _ScreenShake()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition += UnityEngine.Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeDuration = 0f;
        }
    }
}

public enum SensCamera
{
    Null,
    Down,
    Up,
    Left,
    Right
}
