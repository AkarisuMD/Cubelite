using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreePowerUp : MonoBehaviour
{
    [SerializeField] private AnimationCurve DropAnimationX, DropAnimationY;
    [SerializeField] private float puissance;
    float timeBreath;
    private void Update()
    {
        transform.position = transform.position + new Vector3(DropAnimationX.Evaluate(timeBreath) * puissance,
                                                              DropAnimationY.Evaluate(timeBreath) * puissance, 0f);
        timeBreath += Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PowerUpSystem.Instance.ActiveSystem();
            Destroy(gameObject);
        }
    }
}
