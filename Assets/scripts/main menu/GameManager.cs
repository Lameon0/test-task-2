using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject panelSettings;
    public GameObject menuPanels;
    public GameObject Shop;
    void Start()
    {
        panelSettings.SetActive(false);
        Shop.SetActive(false);
        menuPanels.SetActive(true);
    }

    // Update is called once per frame
    public void play()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
    }
    public void openSetting()
    {
        if (panelSettings.activeSelf == false && menuPanels.activeSelf == true)
        {
            menuPanels.SetActive(false);
            panelSettings.SetActive(true);
        }
    }
    public void closeSetting()
    {
        if (panelSettings.activeSelf == true && menuPanels.activeSelf == false)
        {
            menuPanels.SetActive(true);
            panelSettings.SetActive(false);
        }
    }
    public void openShop()
    {
        if (Shop.activeSelf == false && menuPanels.activeSelf == true)
        {
            menuPanels.SetActive(false);
            Shop.SetActive(true);
        }
    }
    public void closeShop()
    {
        if (Shop.activeSelf == true && menuPanels.activeSelf == false)
        {
            menuPanels.SetActive(true);
            Shop.SetActive(false);
        }
    }
    public void Exit()
    {
        Application.Quit();
    }
}