using UnityEngine;

public class DragLine : MonoBehaviour
{

    private LineRenderer line;
    [SerializeField] private PlayerController plCont;
    [SerializeField] private Camera camera;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth = line.endWidth = 0.0013f;
        line.startColor = Color.white;
        line.endColor = Color.white;

    }

    void FixedUpdate()
    {
        LinePosition();
    }

    private void LinePosition()
    {
        if (plCont.pulledRb != null && !plCont.onRb)
        {
            Vector3 screenPoint = camera.WorldToScreenPoint(plCont.pulledRb.position);
            Vector2 result;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gameObject.GetComponent<RectTransform>(), screenPoint, camera, out result);
            line.numCapVertices = 7;
            line.widthMultiplier = 0.0005f * Mathf.Clamp(1 + ((Vector2.Distance(Vector2.zero, result) * -1) * 0.001f), 0.3f, 1f);
            if (Vector3.Dot((plCont.pulledRb.position - transform.position), transform.forward) < 0)
            {
                line.SetPosition(1, -result);
            }
            else
            {
                line.SetPosition(1, result);
            }
        }
        else
        {
            line.numCapVertices = 0;
            line.SetPosition(1, Vector3.zero);
        }
    }

}
