using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadingPanel : MonoBehaviour
{
    public GameObject LoadPanelGameObject;
    public Button SaveButton;
    public Button ListButton;
    public Button CloseButton;
    public Field Field;
    public Transform ListContent;
    public GameObject CellPrefab;
    
    private void Start()
    {
        ListButton.onClick.AddListener(() =>
        {
            Field.Hide();
            UpdateTable();
            LoadPanelGameObject.SetActive(true);
        });
        
        CloseButton.onClick.AddListener(() =>
        {
            Field.Show();
            LoadPanelGameObject.SetActive(false);
        });
        
        SaveButton.onClick.AddListener(() =>
        {
            if (Field.GetPiecePositions().Count > 1)
                FieldStock.SaveField(Field.GetPiecePositions(), Field.GetLoadIndex());
        });
    }

    private void UpdateTable()
    {
        if (FieldStock.GetCount() == ListContent.childCount)
            return;
        foreach (Transform child in ListContent)
            Destroy(child.gameObject);

        for (var i = 0; i < FieldStock.GetCount(); i++)
        {
            var cell = Instantiate(CellPrefab, ListContent);
            var loadIndex = i;
            cell.GetComponent<Button>()
                .onClick.AddListener(() =>
                {
                    Field.StartWithLoad(loadIndex, FieldStock.LoadAt(loadIndex));
                    LoadPanelGameObject.SetActive(false);
                });
            cell.GetComponentInChildren<Text>().text = (loadIndex + 1).ToString();
        }
    }
}