using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputHelper
{
    public static Vector3 GetPointerPosition()
    {
#if UNITY_EDITOR
        return Input.mousePosition;
#else
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;
        return Vector3.zero;
#endif
    }
}
