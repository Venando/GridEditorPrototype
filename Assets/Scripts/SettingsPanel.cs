using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public GameObject SettingsPanelGameObject;
    public Field Field;
    public Button SettingsButton;
    public Button CloseSettingsButton;
    public Button Start17Button;
    public Button Start35Button;
    public Button Start51Button;

    private void Start()
    {
        SettingsPanelGameObject.SetActive(true);
        SettingsButton.onClick.AddListener(() =>
        {
            Field.Hide();
            SettingsPanelGameObject.SetActive(true);
            CloseSettingsButton.gameObject.SetActive(true);
        });
        
        CloseSettingsButton.onClick.AddListener(() =>
        {
            Field.Show();
            SettingsPanelGameObject.SetActive(false);
        });
        CloseSettingsButton.gameObject.SetActive(false);
        
        Field.Hide();

        SetButton(Start17Button, 17);
        SetButton(Start35Button, 35);
        SetButton(Start51Button, 51);
    }

    private void SetButton(Button button, int size)
    {
        button.onClick.AddListener(() =>
        {
            Field.StartWithSize(size);
            SettingsPanelGameObject.SetActive(false);
        });
    }
}