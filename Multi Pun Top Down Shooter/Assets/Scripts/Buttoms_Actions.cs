using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttoms_Actions : MonoBehaviour
{
    public GameObject targetObject;

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
}
