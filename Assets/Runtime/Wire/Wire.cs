using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

public unsafe class Wire : MonoBehaviour
{
    [Min(2)]
    public int TotalNodes = 200;
    public int Iterations = 50;

    [Min(0.001f)]
    public float StepTime = 0.01f;
    public float MaxStep = 0.1f;
    public float2 Gravity = new float2(0, -1f);

    public bool Gizmos = false;

    private NativeArray<WireSimulation.Node> Nodes;

    private WireSimulation.Node* NodesPtr;
    private WireSimulation.Node* NodePtrAt(int index) => (NodesPtr + index);

    private WireSimulation.UpdateJob Job;
    private JobHandle Handle;

    public WireSimulation.Node[] NodeArray => Nodes.ToArray();

    private void Awake()
    {
        var origin = (float3)transform.position;

        Nodes = new NativeArray<WireSimulation.Node>(TotalNodes, Allocator.Persistent);
        NodesPtr = (WireSimulation.Node*)NativeArrayUnsafeUtility.GetUnsafePtr(Nodes);

        for (int i = 0; i < Nodes.Length; i++)
        {
            Nodes[i] = new WireSimulation.Node(origin.xy);
            origin.y -= WireSimulation.NODE_DISTANCE;
        }

        NodePtrAt(0)->Constrained = true;
    }


    private void Update()
    {
        Handle = new WireSimulation.GravityJob
        {
            Gravity = Gravity,
            Friction = 0.5f,
            Nodes = Nodes,
            StepTime = math.sqrt(StepTime),
        }.Schedule(Nodes.Length, 16, Handle);

        Handle = new WireSimulation.UpdateJob
        {
            Nodes = Nodes,
            Iterations = Iterations
        }.Schedule(Handle);
    }

    private void LateUpdate()
    {
        Handle.Complete();

        transform.position = new float3(Nodes[0].Position, transform.position.z);

        // @todo remove
        // for debugging purposes follow mouse
        NodesPtr->Position = ((float3)Camera.main.ScreenToWorldPoint(Input.mousePosition)).xy;
    }


    private void OnDestroy()
    {
        Nodes.Dispose();
    }

}

#if UNITY_EDITOR
public static class WireGizmos
{
    [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy | GizmoType.InSelectionHierarchy)]
    private static void DrawZoneBounds(Wire wire, GizmoType gizmoType)
    {
        if (!Application.isPlaying || !wire.Gizmos)
        {
            return;
        }

        for (int i = 0; i < wire.NodeArray.Length - 1; i++)
        {
            if (i % 2 == 0)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.white;
            }

            Gizmos.DrawLine(
                new float3(wire.NodeArray[i].Position, 0),
                new float3(wire.NodeArray[i + 1].Position, 0));
        }
    }
}
#endif
