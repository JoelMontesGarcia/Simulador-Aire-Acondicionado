using UnityEngine;

public class DropZone : MonoBehaviour
{
    [Header("Tipo válido en esta zona")]
    [SerializeField] private CableType validCableType;

    private Draggable currentItem;

    internal bool IsCorrect()
    {
        // No puede ser correcto si está vacío
        if (currentItem == null)
            return false;

        // Comprueba si el tipo coincide
        return currentItem.CableType == validCableType;
    }

    // Indica si la zona está ocupada
    public bool IsOccupied
    {
        get { return currentItem != null; }
    }

    // Acceso al objeto conectado actualmente
    public Draggable CurrentItem => currentItem;

    // Devuelve el tipo válido de esta zona
    public CableType ValidCableType => validCableType;

    public void SetItem(Draggable item)
    {
        // Evita reasignar el mismo objeto
        if (currentItem == item) return;

        currentItem = item;
    }

    public void ForceClearIfThisItem(Draggable item)
    {
        // Limpia solo si coincide el objeto
        if (currentItem == item)
            currentItem = null;
    }

    public void ClearItem()
    {
        // Limpia la zona manualmente
        currentItem = null;
    }
}