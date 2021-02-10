using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public unsafe class Wire : MonoBehaviour
{
    [Min(2)]
    public int TotalNodes = 200;

    [Min(0.001f)]
    public float StepTime = 0.01f;
    public float MaxStep = 0.1f;
    public float2 Gravity = new float2(0, -1f);

    private NativeArray<WireSimulation.Node> Nodes;

    private WireSimulation.Node* NodesPtr;
    private WireSimulation.Node* NodePtrAt(int index) => (NodesPtr + index);

    private WireSimulation.UpdateJob Job;
    private JobHandle Handle;
    private float timeAccum;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CalcExecutions()
    {
        timeAccum += Time.deltaTime;
        timeAccum = math.min(timeAccum, MaxStep);
        var executions = (int)(timeAccum / StepTime);
        timeAccum = timeAccum % StepTime;
        return executions;
    }

    private void Update()
    {
        Job = new WireSimulation.UpdateJob
        {
            Executions = CalcExecutions(),
            Gravity = Gravity,
            Friction = 0.5f,
            Nodes = Nodes,
            StepTime = StepTime,
        };

        Handle = Job.Schedule();
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
