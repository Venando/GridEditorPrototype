using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PiecePicker : MonoBehaviour
{
    public RectTransform SelectedPieceTransform;
    public EventTrigger SelectionTrigger;
    public Image PieceDemo;
    public Field Field;
    
    public event Action<Vector2Int> OnSelection;
    public event Action OnRelease;
    
    private Vector2Int _selectedPieceType;
    
    private void Start()
    {
        AddEvent(EventTriggerType.PointerDown, () => OnSelection?.Invoke(_selectedPieceType));
        AddEvent(EventTriggerType.PointerUp, () =>
        {
            OnRelease?.Invoke();
            Field.NextColor();
        });

        Field.OnColorChange += col =>
        {
            UpdateColor();
        };
    }

    private void UpdateColor()
    {
        PieceDemo.color = Field.GetColor();
    }

    private void AddEvent(EventTriggerType type, Action callback)
    {
        var triggerEvent = new EventTrigger.TriggerEvent();
        triggerEvent.AddListener(eventData => { callback?.Invoke(); });
        SelectionTrigger.triggers.Add(new EventTrigger.Entry {callback = triggerEvent, eventID = type});
    }

    public void SetSize(Vector2 sizeDelta, Vector2Int pieceType)
    {
        SelectedPieceTransform.sizeDelta = sizeDelta;
        _selectedPieceType = pieceType;
    }
}
