using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProceduralCable : MonoBehaviour
{
    public Transform startPoint; // Punto donde empieza el cable
    public Transform endPoint;   // Punto donde termina el cable

    [Header("Cable Settings")]
    public int segments = 20;        // Cuántos puntos tiene el cable
    public float sag = 2f;           // Cuánto cuelga el cable hacia abajo

    private LineRenderer line;

    void Awake()
    {
        // Guardamos el LineRenderer y le decimos cuántos puntos va a usar
        line = GetComponent<LineRenderer>();
        line.positionCount = segments;
    }

    void Update()
    {
        // Cada frame volvemos a dibujar el cable
        DrawCable();
    }

    void DrawCable()
    {
        // Recorremos todos los puntos del cable
        for (int i = 0; i < segments; i++)
        {
            // Esto nos da un valor entre 0 y 1 dependiendo de la posición en el cable
            float t = i / (float)(segments - 1);

            // Colocamos el punto entre el inicio y el final
            Vector3 pos = Vector3.Lerp(startPoint.position, endPoint.position, t);

            //El cable cuelga en el centro
            float sagOffset = Mathf.Sin(t * Mathf.PI) * sag;
            pos.y -= sagOffset;

            // Guardamos la posición final en el LineRenderer
            line.SetPosition(i, pos);
        }
    }
}