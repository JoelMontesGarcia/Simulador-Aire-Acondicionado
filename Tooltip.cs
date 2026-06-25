using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TooltipDisplay : MonoBehaviour
{
    [Header("UI")]
    public Canvas tooltipCanvas;              // Canvas donde se dibuja el tooltip
    public RectTransform tooltipPanel;        // Panel visual del tooltip
    public TextMeshProUGUI tooltipText;      // Texto dinámico

    [Header("Settings")]
    public Vector2 screenMargin = new Vector2(10f, 10f); // Separación respecto a la esquina
    public LayerMask layerMask = ~0;                     // Qué objetos pueden mostrar tooltip
    public float maxDistance = 100f;                     // Alcance del raycast

    [Header("Camera (IMPORTANT)")]
    [SerializeField] private Camera mainCamera;          // Cámara usada para raycast

    // Ajustes internos para dimensionar el panel según el texto
    private Vector2 basePadding = Vector2.zero;
    private Vector2 maxPanelSize = Vector2.zero;
    private RectTransform canvasRect;

    private void Awake()
    {
        // Si no se asigna cámara, usamos la principal
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Si no hay canvas asignado, intentamos encontrar uno en escena
        if (tooltipCanvas == null)
            tooltipCanvas = FindObjectOfType<Canvas>();

        if (tooltipCanvas != null)
            canvasRect = tooltipCanvas.GetComponent<RectTransform>();

        // Inicialización de tamaños del panel según el texto base
        if (tooltipPanel != null && tooltipText != null)
        {
            string sample = string.IsNullOrEmpty(tooltipText.text) ? " " : tooltipText.text;

            // Calcula tamaño mínimo necesario para el texto inicial
            Vector2 textSize = tooltipText.GetPreferredValues(sample);

            maxPanelSize = tooltipPanel.sizeDelta;

            // Padding = espacio extra del panel respecto al texto
            basePadding = maxPanelSize - textSize;

            basePadding.x = Mathf.Max(0f, basePadding.x);
            basePadding.y = Mathf.Max(0f, basePadding.y);

            // Tooltip oculto al inicio
            tooltipPanel.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Seguridad: si la cámara desaparece, intentamos recuperarla
        if (mainCamera == null)
        {
            mainCamera = Camera.main;

            // Si no hay cámara, ocultamos tooltip para evitar errores
            if (mainCamera == null)
            {
                HideTooltip();
                return;
            }
        }

        // Si el cursor está sobre UI, no mostramos tooltips del mundo
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            HideTooltip();
            return;
        }

        // Ray desde la posición del mouse hacia el mundo
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Detecta objetos interactuables dentro del rango
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
        {
            var trigger = hit.collider.GetComponent<TooltipSource>();

            // Si el objeto tiene tooltip válido, lo mostramos
            if (trigger != null && !string.IsNullOrEmpty(trigger.tooltip))
            {
                ShowTooltip(trigger.tooltip);
                return;
            }
        }

        // Si no hay nada bajo el cursor, ocultamos tooltip
        HideTooltip();
    }

    private void ShowTooltip(string text)
    {
        // Actualizamos el contenido del texto
        tooltipText.text = text;

        // Fuerza recalculo del layout UI
        Canvas.ForceUpdateCanvases();

        // Calcula tamaño óptimo según el contenido del texto
        Vector2 textSize = tooltipText.GetPreferredValues(text);
        Vector2 panelSize = textSize + basePadding;

        // Limita el tamaño del panel si hay máximos definidos
        if (maxPanelSize.x > 0f)
            panelSize.x = Mathf.Min(panelSize.x, maxPanelSize.x);

        if (maxPanelSize.y > 0f)
            panelSize.y = Mathf.Min(panelSize.y, maxPanelSize.y);

        tooltipPanel.sizeDelta = panelSize;

        // Configuración para posicionamiento en pantalla (esquina inferior izquierda)
        tooltipPanel.anchorMin = Vector2.zero;
        tooltipPanel.anchorMax = Vector2.zero;
        tooltipPanel.pivot = Vector2.zero;

        tooltipPanel.anchoredPosition = screenMargin;

        tooltipPanel.gameObject.SetActive(true);
    }

    private void HideTooltip()
    {
        // Oculta el panel completo del tooltip
        if (tooltipPanel != null)
            tooltipPanel.gameObject.SetActive(false);
    }

    // Permite cambiar la cámara desde sistemas externos (Cinemachine, switches, etc.)
    public void SetCamera(Camera cam)
    {
        mainCamera = cam;
    }
}