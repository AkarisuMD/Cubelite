using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonND<GameManager>
{
    private void Start()
    {
        Texture2D sprite = Resources.Load<Texture2D>("cursor02");
        Cursor.SetCursor(sprite, new Vector2(85f, 78f), CursorMode.Auto);
    }

    public void StartGame() => StartCoroutine(_StartGame());
    IEnumerator _StartGame()
    {
        Time.timeScale = 0.0f;
        AsyncOperation loadscene = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        yield return !loadscene.isDone;
        Time.timeScale = 1.0f;
    }
    public void Retry() => StartCoroutine(_Retry());
    IEnumerator _Retry()
    {
        AsyncOperation loadscene = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        yield return !loadscene.isDone;
        yield return new WaitForSeconds(0.01f);
        StartGame();
    }
}
