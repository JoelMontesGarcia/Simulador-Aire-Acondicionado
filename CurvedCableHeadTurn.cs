using UnityEngine;

// Este script sirve para colocar y rotar los cabezales de un cable con cuelgue
public class CableHeadsFromCurve : MonoBehaviour
{
    // Punto inicial del cable
    public Transform startPoint;

    // Punto final del cable
    public Transform endPoint;

    // Primer cabezal
    public Transform headStart;

    // Segundo cabezal
    public Transform headEnd;

    // Cuánto cuelgue tiene el lineRenderer
    public float sag = 0.1f;

    // Asigna automáticamente el startPoint al transform del objeto si no está asignado
    void Awake()
    {
        if (startPoint == null)
            startPoint = this.transform;
    }

    void LateUpdate()
    {
        // Si falta algún punto, no hacemos nada
        if (startPoint == null || endPoint == null) return;

        // Guardamos posiciones
        Vector3 startPos = startPoint.position;
        Vector3 endPos = endPoint.position;

        // Calculamos la posición real del cable al inicio y al final con curva
        Vector3 realStart = GetCablePoint(0f, startPos, endPos);
        Vector3 realEnd = GetCablePoint(1f, startPos, endPos);

        // Calculamos la dirección del cable en los extremos
        Vector3 dirStart = GetCableTangent(0.01f, startPos, endPos);
        Vector3 dirEnd = GetCableTangent(0.99f, startPos, endPos);

        // Si la dirección es muy pequeña, usamos una dirección básica
        if (dirStart.sqrMagnitude < 0.00001f)
            dirStart = (endPos - startPos);

        if (dirEnd.sqrMagnitude < 0.00001f)
            dirEnd = (startPos - endPos);

        // Cabeza principio
        if (dirStart.sqrMagnitude > 0.00001f)
        {
            // Colocamos el objeto en el inicio del cable
            headStart.position = realStart;

            // Rotación mirando en la dirección del cable
            Quaternion targetRot = Quaternion.LookRotation(dirStart.normalized, Vector3.up);

            // Aplicamos rotación suavizada
            headStart.rotation = SmoothStableRotation(headStart.rotation, targetRot);
        }

        // Cabeza final
        if (dirEnd.sqrMagnitude > 0.00001f)
        {
            // Colocamos el objeto en el final del cable
            headEnd.position = realEnd;

            // Rotación mirando en la dirección opuesta para mantener consistencia
            Quaternion targetRot = Quaternion.LookRotation(-dirEnd.normalized, Vector3.up);

            // Aplicamos rotación suavizada
            headEnd.rotation = SmoothStableRotation(headEnd.rotation, targetRot);
        }
    }

    // Esta función suaviza la rotación y evita que dé vueltas raras
    Quaternion SmoothStableRotation(Quaternion current, Quaternion target)
    {
        // Si están en direcciones opuestas, corregimos el target
        if (Quaternion.Dot(current, target) < 0f)
        {
            target = new Quaternion(-target.x, -target.y, -target.z, -target.w);
        }

        // Interpolamos suavemente
        return Quaternion.Slerp(current, target, Time.deltaTime * 15f);
    }

    // Devuelve un punto del cable en función de t (0 = inicio, 1 = final)
    Vector3 GetCablePoint(float t, Vector3 start, Vector3 end)
    {
        // Interpolación entre los dos puntos
        Vector3 pos = Vector3.Lerp(start, end, t);

        // Aplicamos una curva hacia abajo con seno
        pos.y -= Mathf.Sin(t * Mathf.PI) * sag;

        return pos;
    }

    // Calcula la dirección del cable en un punto concreto
    Vector3 GetCableTangent(float t, Vector3 start, Vector3 end)
    {
        float delta = 0.02f;

        // Tomamos dos puntos cercanos
        Vector3 a = GetCablePoint(Mathf.Clamp01(t - delta), start, end);
        Vector3 b = GetCablePoint(Mathf.Clamp01(t + delta), start, end);

        // La dirección es la resta entre ellos
        return (b - a);
    }
}