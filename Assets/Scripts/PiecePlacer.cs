using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PiecePlacer : MonoBehaviour
{
    public PiecePicker PiecePicker;
    public Field Field;

    private Transform _placingPiece;
    private bool _placingProcess;
    private Vector2Int _placingType;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        PiecePicker.OnSelection += selectedType =>
        {
            _placingType = selectedType;
            _placingProcess = true;
            _placingPiece = Field.SpawnedPieceBase(selectedType, false).transform;
        };

        PiecePicker.OnRelease += () =>
        {
            _placingProcess = false;
            if (TryGetFieldPoint(out var position))
            {
                Field.SpawnPiece(position, _placingType);
            }
            Destroy(_placingPiece.gameObject);
        };
    }

    private void Update()
    {
        if (!_placingProcess)
            return;
        TryGetFieldPoint(out var position);
        _placingPiece.position = position;
    }

    private bool TryGetFieldPoint(out Vector3 position)
    {
        var screenPos = InputHelper.GetPointerPosition();
        var halfCellSize = Field.GetCellSize() / 2f;
        
        var worldPoint = _camera.ScreenToWorldPoint(screenPos) - halfCellSize * _placingType.x * Vector3.right
                                                                      - halfCellSize * _placingType.y * Vector3.up;
        
        
        var closestPoint = Field.GetClosestWorldPosition(worldPoint, _placingType);

        worldPoint.z = -1;
        closestPoint.z = -1;
        
        if ((closestPoint - worldPoint).sqrMagnitude < 2)
        {
            position = closestPoint;
            return true;
        }
        else
        {
            position = worldPoint;
            return false;
        }
    }
}
