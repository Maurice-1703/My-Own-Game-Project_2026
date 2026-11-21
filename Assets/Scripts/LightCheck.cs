using System;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Rendering.Universal; // for Light2D

// Checks if target is lit by this spot light
[RequireComponent(typeof(Light2D))]
public class LightCheck : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleMask; // what blocks light
    [SerializeField] private Collider2D targetCollider; // player collider
    [SerializeField] private float inset = 0.05f;       // keep points off floor/walls

    private Light2D light2D;

    private void Awake()
    {
        // cache light component
        light2D = GetComponent<Light2D>();
    }

    private void Update()
    {
        bool lit = IsAnyPointLit();

        if (lit) Debug.Log("Hit");
    }

    // true = target is in this light
    private bool IsLit(Vector2 targetPos)
    {
        Vector2 lightPos = transform.position;
        Vector2 toTarget = targetPos - lightPos; // vector light -> target

        // 1) range check
        if (toTarget.magnitude > light2D.pointLightOuterRadius) return false;

        // 2) cone check
        if (Vector2.Angle(transform.up, toTarget) > (light2D.pointLightOuterAngle / 2)) return false;

        // 3) line of sight
        RaycastHit2D hit = Physics2D.Linecast(lightPos, targetPos, obstacleMask);
        if (hit.collider != null) return false; // obstacle between light and target

        return true; // passed all checks
    }

    // true = at least one sample point is lit
    private bool IsAnyPointLit()
    {
        Bounds b = targetCollider.bounds; // world-space box around collider

        Vector2[] points =
            {
                b.center,
                new Vector2(b.min.x + inset, b.max.y - inset),  // top-left
                new Vector2(b.max.x - inset, b.max.y - inset),  // top-right
                new Vector2(b.min.x + inset, b.min.y + inset),  // bottom-left
                new Vector2(b.max.x - inset, b.min.y + inset)   // bottom-right
            };

        foreach (Vector2 p in points)
        {
            // return true as soon as one point is lit
            bool lit = IsLit(p);

            // draw debug line to p (red = lit, green = safe)
            Color lineColor = lit ? Color.red : Color.green;
            Debug.DrawLine(transform.position, p, lineColor);

            if (lit) return true;
        }

        return false; // no point lit
    }
}