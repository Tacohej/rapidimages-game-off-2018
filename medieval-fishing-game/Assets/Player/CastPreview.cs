using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPreview : MonoBehaviour {
    public CastStats castStats;
    private LineRenderer lineRenderer;
    [SerializeField]
    private Vector3[] positions;

    public float starting_height = 1.0f;
    // public float initial_velocity = 20.0f;
    public float air_resistance = 2.0f;
    // private float gravity = -5;
    private const int segments = 32;

    void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        positions = new Vector3[segments];
        lineRenderer.positionCount = segments;
        lineRenderer.startWidth = 2.0f;
        lineRenderer.endWidth = 0.1f;
    }

    void Update () {

        float t = 0.0f;
        float impact_t = calculate_impact_time();
        float step = impact_t / (positions.Length - 1);
        for(int i=0; i < positions.Length; i++) {
            t = i * step;
            var tt = t * t;
            positions[i] = calculate_arc_position(t);
        }

        lineRenderer.SetPositions(positions);
    }

    private Vector3 calculate_arc_position(float t) {
        var tt = t * t;
        Vector3 v;
        v.x = castStats.currentVelocity * t - air_resistance * tt;
        v.y = starting_height + castStats.currentVelocity * t + castStats.currentGravity * tt;
        v.z = 0;
        return v;
    }

    private float calculate_impact_time() {
        var center = castStats.currentVelocity / (2.0f * castStats.currentGravity);
        var offset = Mathf.Sqrt(center * center - starting_height / castStats.currentGravity);
        return -center + Mathf.Abs(offset);
    }
}
