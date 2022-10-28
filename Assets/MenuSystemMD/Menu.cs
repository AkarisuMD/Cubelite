using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    MenuInputs mi;

    public GameObject firstButton, backTo;

    public bool back;
    public void OnEnable()
    {
        mi = new MenuInputs();
        EventSystem.current.SetSelectedGameObject(firstButton);
        mi.UI.Enable();

        //actions
        mi.UI.Back.performed += ctx => BackTo();
    }

    void BackTo()
    {
        StartCoroutine(wait());
        if (backTo != null && back == false)
        {
            backTo.SetActive(true);
            this.gameObject.SetActive(false);
            Debug.Log("passe");
        }
        Debug.Log("UI_Cancel_button performed");
        back = false;
    }
    void OnDisable() {mi.UI.Disable();}

    public void bloque()
    {
        back = true;
    }
    public void debloque()
    {
        back = false;
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.1f);
    }

    public void Stargame()
    {
        GameManager.Instance.StartGame();
    }
    public void Quit()
    {
        Application.Quit();
    }
}