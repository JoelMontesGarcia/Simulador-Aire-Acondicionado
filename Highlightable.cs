using UnityEngine;
using System.Collections;

public class Highlightable : MonoBehaviour
{
    // Renderers afectados por el highlight (puede incluir hijos)
    private Renderer[] renderers;

    // Referencia a la coroutine actual para evitar superposición de animaciones
    private Coroutine highlightRoutine;

    void Awake()
    {
        // Obtenemos todos los renderers del objeto y sus hijos
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void SetHighlight(bool state)
    {
        // Si ya hay una animación en curso, la detenemos para evitar conflictos visuales
        if (highlightRoutine != null)
            StopCoroutine(highlightRoutine);

        // Iniciamos transición suave hacia el estado deseado
        highlightRoutine = StartCoroutine(AnimateHighlight(state));
    }

    IEnumerator AnimateHighlight(bool state)
    {
        // Capturamos el estado actual para interpolar suavemente
        float startFresnel = GetCurrentValue("FresnelPower");
        float startIntensity = GetCurrentValue("HighlightIntensity");

        float target = state ? 2f : 0f;
        float duration = 0.25f;
        float t = 0f;

        // Animación progresiva del highlight
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            float fresnel = Mathf.Lerp(startFresnel, target, lerp);
            float intensity = Mathf.Lerp(startIntensity, target, lerp);

            Apply(fresnel, intensity);

            yield return null;
        }

        // Aseguramos el estado final exacto
        Apply(target, target);
    }

    void Apply(float fresnel, float intensity)
    {
        // Aplicamos los valores a todos los materiales afectados
        foreach (var r in renderers)
        {
            if (r == null) continue;

            var mat = r.material;

            // Ajustamos efectos visuales solo si el shader los soporta
            if (mat.HasProperty("_FresnelPower"))
                mat.SetFloat("_FresnelPower", fresnel);

            if (mat.HasProperty("_HighlightIntensity"))
                mat.SetFloat("_HighlightIntensity", intensity);
        }
    }

    float GetCurrentValue(string name)
    {
        if (renderers.Length == 0) return 0f;

        var mat = renderers[0].material;

        // Obtiene valor actual del shader si existe
        if (mat.HasProperty(name))
            return mat.GetFloat(name);

        return 0f;
    }
}