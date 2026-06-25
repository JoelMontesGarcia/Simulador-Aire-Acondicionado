using UnityEngine;

public class Draggable : MonoBehaviour
{
    [Header("Tipo de cable")]
    public CableType CableType;

    public bool IsDragging => isDragged;

    public delegate void DragEndDelegate(Draggable draggableObject);
    public event DragEndDelegate DragEndCallback;

    public Vector3 DraggableStartPosition;

    private bool isDragged = false;

    private Vector3 offset;

    private float fixedZ;

    private Camera cam;

    public Highlightable[] connectables;

    private LayerMask dragLayer;

    private Collider col;

    public Transform snapTarget;
    public float snapRange = 1f;

    private void Start()
    {
        DraggableStartPosition = transform.position;

        fixedZ = transform.position.z;

        cam = Camera.main;

        col = GetComponent<Collider>();

        dragLayer = LayerMask.GetMask("Connector", "Draggable");
    }

    private void Update()
    {
        HandleInput();
        HandleDrag();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, dragLayer))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    StartDrag(hit);
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragged)
        {
            StopDrag();
        }
    }

    void StartDrag(RaycastHit hit)
    {
        isDragged = true;

        // 🔥 CLAVE: evita interferencias con raycasts globales
        if (col != null)
            col.enabled = false;

        Plane dragPlane = new Plane(Vector3.forward, new Vector3(0, 0, fixedZ));

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorld = ray.GetPoint(distance);
            offset = transform.position - mouseWorld;
        }

        SetHighlight(true);
    }

    void HandleDrag()
    {
        if (!isDragged) return;

        Plane dragPlane = new Plane(Vector3.forward, new Vector3(0, 0, fixedZ));

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);

            Vector3 finalPos = worldPos + offset;
            finalPos.z = fixedZ;

            transform.position = finalPos;
        }
    }

    void StopDrag()
    {
        isDragged = false;

        if (col != null)
            col.enabled = true;

        SetHighlight(false);

        TrySnap();

        DragEndCallback?.Invoke(this);
    }

    void TrySnap()
    {
        if (snapTarget == null) return;

        float distance = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(snapTarget.position.x, snapTarget.position.y)
        );

        if (distance <= snapRange)
        {
            Vector3 snapPos = snapTarget.position;
            snapPos.z = fixedZ;

            transform.position = snapPos;
        }
    }

    void SetHighlight(bool state)
    {
        if (connectables == null) return;

        foreach (var obj in connectables)
        {
            if (obj != null)
                obj.SetHighlight(state);
        }
    }
}