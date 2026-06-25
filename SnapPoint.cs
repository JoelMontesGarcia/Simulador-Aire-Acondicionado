using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    [Header("Snap points")]
    public List<Transform> snapPoints;

    [Header("Objetos draggable")]
    public List<Draggable> draggableObjects;

    [Header("Distancia de snap")]
    public float snapRange = 0.5f;

    void Start()
    {
        // Se suscribe al final del drag de cada objeto
        foreach (Draggable draggable in draggableObjects)
        {
            draggable.DragEndCallback += OnDragEnd;
        }
    }

    private void OnDragEnd(Draggable draggable)
    {
        Transform closestSnapPoint = null;
        DropZone closestZone = null;

        float closestDistance = float.MaxValue;

        // Busca el snap point más cercano válido
        foreach (Transform snapPoint in snapPoints)
        {
            DropZone dz = snapPoint.GetComponent<DropZone>();

            float distance =
                Vector3.Distance(draggable.transform.position,
                                 snapPoint.position);

            // Ignora puntos fuera de rango
            if (distance > snapRange)
                continue;

            // Ignora zonas ocupadas por otros objetos
            if (dz != null &&
                dz.IsOccupied &&
                dz.CurrentItem != draggable)
            {
                continue;
            }

            // Guarda el punto más cercano encontrado
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSnapPoint = snapPoint;
                closestZone = dz;
            }
        }

        // Si encontró una zona válida
        if (closestSnapPoint != null)
        {
            // Libera cualquier zona previa del objeto
            foreach (DropZone dz in snapPoints
                     .Select(p => p.GetComponent<DropZone>()))
            {
                if (dz != null)
                    dz.ForceClearIfThisItem(draggable);
            }

            // Hace snap en la nueva posición
            draggable.transform.position =
                closestSnapPoint.position;

            // Registra el objeto en la zona
            if (closestZone != null)
            {
                closestZone.SetItem(draggable);

                // Debug de validación
                if (closestZone.IsCorrect())
                    Debug.Log("Conexión correcta");
                else
                    Debug.Log("Conexión incorrecta");
            }
        }
        else
        {
            // Limpia zonas previas asociadas
            foreach (DropZone dz in snapPoints
                     .Select(p => p.GetComponent<DropZone>()))
            {
                if (dz != null)
                    dz.ForceClearIfThisItem(draggable);
            }

            // Devuelve el objeto a su posición inicial
            draggable.transform.position =
                draggable.DraggableStartPosition;
        }

        // Recalcula errores tras cada movimiento
        ErrorManager.Instance?.CheckErrors();
    }
}