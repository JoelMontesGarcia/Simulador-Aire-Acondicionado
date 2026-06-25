using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorManager : MonoBehaviour
{
    public static ErrorManager Instance;

    [Header("Todas las zonas")]
    [SerializeField] private List<DropZone> dropZones;

    [Header("UI")]
    [SerializeField] private GameObject errorPanel;

    [SerializeField] private TextMeshProUGUI resultText;

    private void Awake()
    {
        // Guarda referencia global al manager
        Instance = this;

        // Oculta el panel al iniciar
        errorPanel.SetActive(false);
    }

    public void CheckErrors()
    {
        // Comprueba si todas las zonas tienen conexión
        foreach (DropZone zone in dropZones)
        {
            if (!zone.IsOccupied)
            {
                // Mientras falten conexiones, ocultamos el panel
                errorPanel.SetActive(false);

                return;
            }
        }

        // Si todas están ocupadas, mostramos el panel
        errorPanel.SetActive(true);

        int errorCount = 0;

        // Cuenta conexiones incorrectas
        foreach (DropZone zone in dropZones)
        {
            if (!zone.IsCorrect())
            {
                errorCount++;
            }
        }

        // Muestra resultado final
        if (errorCount == 0)
        {
            resultText.text = "Conexiones correctas";
        }
        else
        {
            resultText.text = $"Hay {errorCount} errores";
        }
    }
}