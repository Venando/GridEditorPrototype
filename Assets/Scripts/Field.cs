using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class Field : MonoBehaviour
{
    public int Size = 17;
    
    private int Row => Size;
    private int Col => Size;

    public GameObject Prefab;
    public LineRenderer LineRendererPrefab;
    public Material PieceMaterial;
    public RectTransform FieldBackground;
    public RectTransform FieldSizeRect;
    public PieceEcsSpawner PieceEcsSpawner;
    public event Action<Color> OnColorChange;
    
    private readonly List<(Vector2Int, Vector3)> _cellWorldPositions = new List<(Vector2Int, Vector3)>();
    private readonly List<GameObject> _spawnedLineRenders = new List<GameObject>();
    private readonly List<PieceInfo> _spawnedPiecePositions = new List<PieceInfo>();
    
    private float _cellWidth;
    private float _cellHeight;
    private float _pieceOutlineSize;
    private int _loadIndex;
    private Color _color = Color.white;
    
    public void NextColor()
    {
        SetColor(Color.HSVToRGB(Random.value, 0.58f, 0.8f));
    }

    public Color GetColor()
    {
        return _color;
    }

    private void SetColor(Color color)
    {
        _color = color;
        OnColorChange?.Invoke(color);
    }
    
    public void Hide()
    {
        FieldBackground.gameObject.SetActive(false);
        gameObject.SetActive(false);
        PieceEcsSpawner.SetVisible(false);
    }
    
    public void Show()
    {
        FieldBackground.gameObject.SetActive(true);
        gameObject.SetActive(true);
        PieceEcsSpawner.SetVisible(true);
    }

    public void StartWithSize(int size)
    {
        _loadIndex = -1;
        Size = size;
        SpawnAllPieces();
    }

    public void StartWithLoad(int loadIndex, List<PieceInfo> piecesList)
    {
        _loadIndex = loadIndex;
        var firstPiece = piecesList.First();
        Size = firstPiece.Size.x;
        SpawnAllPieces();
        foreach (var piecePosSize in piecesList.Skip(1))
        {
            SpawnPiece(piecePosSize);
            NextColor();
        }
    }
    
    [ContextMenu("Spawn All")]
    public void SpawnAllPieces()
    {
        SetColor(Color.white);
        Show();
        var rect = FieldSizeRect.rect;
        var fieldSize = Mathf.Min(rect.width, rect.height);
        FieldBackground.sizeDelta = Vector2.one * fieldSize;
        _cellWidth = ((fieldSize * 5.151f) / 1038) / Row;
        _cellHeight = _cellWidth;
        _pieceOutlineSize = 0.0268f;//(float)(0.0268 + (0.0268 - ((_cellWidth * 0.0268) / 0.303)) * 1);//(float) ((_cellWidth * 0.0268) / 0.303);

        foreach (var lineRender in _spawnedLineRenders.Where(render => render != null))
            DestroyImmediate(lineRender.gameObject);
        
        _spawnedLineRenders.Clear();
        _cellWorldPositions.Clear();
        _spawnedPiecePositions.Clear();
        PieceEcsSpawner.DeleteAll();

        SpawnPiece(new PieceInfo(Vector2Int.zero, new Vector2Int(Row, Col), GetColor()), true);
        NextColor();

        for (var i = 0; i < Row; i++)
        {
            for (var j = 0; j < Col; j++)
            {
                var coordinates = new Vector2Int(i, j);
                _cellWorldPositions.Add((coordinates, GetPosition(coordinates) + transform.position));
            }
        }
    }

    public void SpawnPiece(Vector3 position, Vector2Int size)
    {
        var (coordinates, _) = GetClosestLocalPoint(position, size);
        SpawnPiece(new PieceInfo(coordinates, size, GetColor()));
    }

    private void SpawnPiece(PieceInfo pieceInfo, bool basePiece = false)
    {
        var spawnPosition = GetWorldPosition(pieceInfo.Coords);
        if (basePiece)
            spawnPosition += Vector3.forward;
        PieceEcsSpawner.SpawnPiece(spawnPosition, pieceInfo.Color, pieceInfo.Size, _cellWidth);
        if (!basePiece)
            SpawnOutline(GetPosition(pieceInfo.Coords), pieceInfo.Size, transform);
        _spawnedPiecePositions.Add(pieceInfo);
    }

    private Vector3 GetWorldPosition(Vector2Int coordinates)
    {
        var halfRows = Row / 2f;
        var halfCols = Col / 2f;
        return transform.position + new Vector3(coordinates.x * _cellWidth - halfRows * _cellWidth,
            coordinates.y * _cellHeight - halfCols * _cellHeight);
    }

    private Vector3 GetPosition(Vector2Int coordinates)
    {
        var halfRows = Row / 2f;
        var halfCols = Col / 2f;
        return new Vector3(coordinates.x * _cellWidth - halfRows * _cellWidth,
            coordinates.y * _cellHeight - halfCols * _cellHeight);
    }

    public GameObject SpawnedPieceBase(Vector2Int size, bool basePiece)
    {
        var pieceItem = Instantiate(Prefab, transform);
        var pieceTransform = pieceItem.transform.GetChild(0);
        var cellScaleX = _cellWidth * size.x;
        var cellScaleY = _cellHeight * size.y;

        pieceTransform.localScale = new Vector3(cellScaleX, cellScaleY, 1f);

        var pieceRenderer = pieceTransform.GetChild(0).GetComponent<Renderer>();

        pieceRenderer.material = PieceMaterial;
        pieceRenderer.material.mainTextureScale = new Vector2(size.x, size.y);
        pieceRenderer.material.color = _color;

        if (!basePiece)
            SpawnOutline(Vector3.zero, size, pieceItem.transform);
        return pieceItem;
    }

    public void SpawnOutline(Vector3 pos, Vector2Int size, Transform  parent)
    {
        var cellScaleX = _cellWidth * size.x;
        var cellScaleY = _cellHeight * size.y;
        var lineRenderer = Instantiate(LineRendererPrefab, parent);
        lineRenderer.startWidth = _pieceOutlineSize;
        lineRenderer.endWidth = _pieceOutlineSize;
        var linePoints = new Vector3[5];
        linePoints[0] = pos - Vector3.forward;
        linePoints[1] = linePoints[0] + Vector3.right * cellScaleX;
        linePoints[2] = linePoints[1] + Vector3.up * cellScaleY;
        linePoints[3] = linePoints[0] + Vector3.up * cellScaleY;
        linePoints[4] = linePoints[0] + Vector3.down * lineRenderer.startWidth / 2f;

        lineRenderer.SetPositions(linePoints);
        _spawnedLineRenders.Add(lineRenderer.gameObject);
    }

    public Vector3 GetClosestWorldPosition(Vector3 worldPoint, Vector2Int size)
    {
        return GetClosestLocalPoint(worldPoint, size).position;
    }

    public (Vector2Int coordinates, Vector3 position) GetClosestLocalPoint(Vector3 worldPoint, Vector2Int size)
    {
        return _cellWorldPositions.Aggregate((Vector2Int.zero, Vector3.one * 100f), (closestTuple, currentTuple) =>
        {
            if (!CanBePlaced(currentTuple.Item1, size))
                return closestTuple;
            if ((currentTuple.Item2 - worldPoint).sqrMagnitude < (closestTuple.Item2 - worldPoint).sqrMagnitude)
                return currentTuple;
            else
                return closestTuple;
        });
    }

    private bool CanBePlaced(Vector2Int coordinates, Vector2Int size)
    {
        return CanBePlaced(new PieceInfo()
        {
            Coords = coordinates,
            Size = size
        });
    }
    
    private bool CanBePlaced(PieceInfo pieceInfo)
    {
        if (IsOutOfBounds(pieceInfo))
            return false;
        return _spawnedPiecePositions.Skip(1).All(spawnedPiecePosSize => !IsOverlapping(pieceInfo, spawnedPiecePosSize));
    }
    
    private bool IsOverlapping(PieceInfo piecePosSize1, PieceInfo piecePosSize2)
    {
        var posX1 = piecePosSize1.Coords.x;
        var posX2 = piecePosSize2.Coords.x;
        var sizeX1 = piecePosSize1.Size.x;
        var sizeX2 = piecePosSize2.Size.x;
        
        var posY1 = piecePosSize1.Coords.y;
        var posY2 = piecePosSize2.Coords.y;
        var sizeY1 = piecePosSize1.Size.y;
        var sizeY2 = piecePosSize2.Size.y;

        return IsOverlapping(posX1, posX1 + sizeX1, posX2, posX2 + sizeX2) &&
               IsOverlapping(posY1, posY1 + sizeY1, posY2, posY2 + sizeY2);
    }

    private bool IsOutOfBounds(PieceInfo piecePosSize)
    {
        if (piecePosSize.Coords.x + piecePosSize.Size.x > Col)
            return true;
        if (piecePosSize.Coords.y + piecePosSize.Size.y > Row)
            return true;
        return false;
    }

    private bool IsOverlapping(int a1, int b1, int a2, int b2)
    {
        if (a1 == a2 || b1 == b2)
            return true;
        if (a1 < a2 && b1 > a2) // [(]), [()]
            return true;
        if (a1 > a2 && a1 < b2) // ([)]
            return true; 
        if (a1 > a2 && b1 < b2) // [()]
            return true;
        return false;
    }

    public float GetCellSize()
    {
        return _cellWidth;
    }

    public int GetLoadIndex()
    {
        return _loadIndex;
    }

    public IReadOnlyList<PieceInfo> GetPiecePositions()
    {
        return _spawnedPiecePositions;
    }
}

[Serializable]
public struct PieceInfo
{
    public Vector2Int Coords;
    public Vector2Int Size;
    [JsonIgnore]
    public Color Color => new Color(ColorVector3.x, ColorVector3.y, ColorVector3.z, 1f);
    public Vector3 ColorVector3;

    public PieceInfo(Vector2Int coords, Vector2Int size, Color color)
    {
        Coords = coords;
        Size = size;
        ColorVector3 = new Vector3(color.r, color.g, color.b);
    }
}