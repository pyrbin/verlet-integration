using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Wire), typeof(LineRenderer))]
public class WireRenderer : MonoBehaviour
{

    private LineRenderer LineRenderer;
    private Wire Wire;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out Wire);
        TryGetComponent(out LineRenderer);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LineRenderer.startWidth = .15f;
        LineRenderer.endWidth = .15f;
        LineRenderer.useWorldSpace = true;
        LineRenderer.alignment = LineAlignment.View;
        LineRenderer.positionCount = Wire.TotalNodes;
        LineRenderer.SetPositions(Wire.NodeArray.Select(x => (Vector3)new float3(x.Position, 0)).ToArray());
    }
}
