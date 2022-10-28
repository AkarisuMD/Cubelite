using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    PlayerData playerData;
    SpriteRenderer spriteRenderer;

    public AnimationCurve HPcurve5HP;
    public AnimationCurve HPcurve3HP;
    public AnimationCurve HPcurve1HP;

    private void Start()
    {
        playerData = PlayerData.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (playerData.HP > 10)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        }
        else if (playerData.HP > 5)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, HPcurve5HP.Evaluate(Time.time));
        }
        else if (playerData.HP > 3)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, HPcurve3HP.Evaluate(Time.time));
        }
        else
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, HPcurve1HP.Evaluate(Time.time));
        }
    }
}
