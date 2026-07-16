using UnityEngine;
using UnityEngine.EventSystems;

// Tracks press-and-hold state for a UI Button used as a movement control.
// Works with both touch and mouse since it goes through Unity's UI event system.
public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsHeld { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsHeld = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsHeld = false;
    }

    void OnDisable()
    {
        IsHeld = false;
    }
}
