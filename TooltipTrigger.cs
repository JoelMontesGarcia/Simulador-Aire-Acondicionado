using UnityEngine;

// Evita que se puedan añadir múltiples TooltipSource al mismo objeto
[DisallowMultipleComponent]
public class TooltipSource : MonoBehaviour
{
    // Texto que se mostrará cuando el jugador apunte al objeto
    [TextArea]
    public string tooltip = "Nombre del objeto";

    // Permite modificar el tooltip desde otros sistemas en runtime
    public void SetTooltip(string text)
    {
        tooltip = text;
    }
}