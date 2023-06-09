using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Player;

    public static GameManager Instance = null;

    public MessageManager messageManager;

    [SerializeField]
    GameObject inventoryButton;

    [SerializeField]
    GameObject AppPanel;

    [SerializeField]
    GameObject TextpopPref;
    void Awake() //Create Singleton
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != this)
            Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);


    }

    public void ClickInventoryButton()
    {
        if(!Inventory.Instance.showingPanel)
        {
            inventoryButton.SetActive(false);
            Inventory.Instance.Open();
            AudioManager.Instance.PlaySound(2);
        }       
    }

    public void CloseInventory()
    {
        if (Inventory.Instance.showingPanel)
        {
            inventoryButton.SetActive(true);
            Inventory.Instance.Close();
            AudioManager.Instance.PlaySound(3);
        }       
    }

    public void OpenAppPanel()
    {
        AppPanel.SetActive(true);
    }

    public void CloseAppPanel()
    {
        AppPanel.SetActive(false);
    }

    public void CloseApp()
    {
        Application.Quit();
    }
    

    public void TextPop(string str, Color color, Transform pos)
    {
        GameObject newObject = (GameObject)Instantiate(TextpopPref);
        newObject.transform.SetParent(this.transform);

        TextPopUp newTextPopUP = newObject.GetComponent<TextPopUp>();

        newTextPopUP.PopText(str, color, pos);
    }
}
