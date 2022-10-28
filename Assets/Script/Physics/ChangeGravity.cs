using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGravity : MonoBehaviour
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
    [SerializeField] private GroundType newSide;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerData.Instance.gravityChangeType = GravityChangeType.Bloc;
            Debug.Log("change gravity");
            PlayerBehaviour pb = collision.GetComponentInParent<PlayerBehaviour>();
            switch (newSide)
            {
                case GroundType.Down:
                    pb.NewRotation(0);
                    break;
                case GroundType.Right:
                    pb.NewRotation(90);
                    break;
                case GroundType.Left:
                    pb.NewRotation(270);
                    break;
                case GroundType.Up:
                    pb.NewRotation(180);
                    break;
                default:
                    break;
            }
        }
        StartCoroutine(timer());
    }
    IEnumerator timer()
    {
        yield return new WaitForSeconds(0.25f);
        PlayerData.Instance.gravityChangeType = GravityChangeType.Collision;
    }
}
