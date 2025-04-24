using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Buttoms_Actions : MonoBehaviour
{
    public GameObject targetObject;
    public TMP_InputField playerNameInput;
    public TextMeshProUGUI playerNameText;
    private string playerName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ToggleGameObject()
    {
        if(targetObject != null)
        {
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }

    public void SetGameObjectActive(bool state)
    {
        if(targetObject != null)
        {
            targetObject.SetActive(state);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Exit Game");
        //Application.Quit();
        
    }

    public void SetPlayerName()
    {
        if (playerNameInput != null)
        {
            playerName = playerNameInput.text;
            if (playerNameText != null)
            {
                playerNameText.text = playerName;
            }
            PlayerPrefs.SetString("PlayerName", playerName);
            PlayerPrefs.Save();
        }
    }
}
