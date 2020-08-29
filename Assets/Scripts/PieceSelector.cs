using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PieceSelector : MonoBehaviour
{
    public GameObject Prefab;
    public RectTransform TableContent;
    public Vector2Int[] PieceTypes;

    public float PieceWidth;
    public float PieceHeight;
    public float PixelPerUnit;

    public PiecePicker PiecePicker;

    private Image[] _selectionImages;
    private readonly Color _selectedColor = new Color(0.2039216f, 0.5215687f, 1f, 1f);
    private readonly Color _notSelectedColor = new Color(1f, 1f, 1f, 0.01f);
    
    private IEnumerator Start()
    {
        foreach (Transform child in TableContent)
            Destroy(child.gameObject);
        foreach (var pieceType in PieceTypes)
        {
            var pieceItem = Instantiate(Prefab, TableContent);
            var pieceItemImage = pieceItem.GetComponent<Image>();
            var piece =  (RectTransform)pieceItem.transform.GetChild(0);
            piece.sizeDelta = new Vector2(PieceWidth * pieceType.x, PieceHeight * pieceType.y);
            piece.GetComponent<Image>().pixelsPerUnitMultiplier = PixelPerUnit;
            pieceItemImage.color = _notSelectedColor;
            pieceItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                DisableSelection();
                PiecePicker.SetSize(piece.sizeDelta, pieceType);
                pieceItemImage.color = _selectedColor;
            });
        }
        yield return null;
        _selectionImages = TableContent.Cast<Transform>().Select(child => child.GetComponent<Image>()).ToArray();
        TableContent.Cast<Transform>().Skip(1).First().GetComponent<Button>().onClick.Invoke();
    }

    private void DisableSelection()
    {
        foreach (var selectedImage in _selectionImages)
            selectedImage.color = _notSelectedColor;
    }
}
