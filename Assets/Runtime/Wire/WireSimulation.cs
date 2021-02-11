using Unity.Jobs;
using Unity.Burst;

using Unity.Mathematics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System;
using Unity.Collections;

public static class WireSimulation
{
    public const float NODE_DISTANCE = .25f;

    [BurstCompile]
    public struct GravityJob : IJobParallelFor
    {
        public float StepTime;
        public float2 Friction, Gravity;

        public NativeArray<Node> Nodes;

        public void Execute(int index)
        {
            var node = Nodes[index];

            if (node.Constrained)
                return;

            var velocity = node.Velocity;
            var acceleration = Gravity;

            if (math.length(velocity) > 1f)
            {
                velocity = math.normalize(velocity) * 1f;
            }

            node.Previous = node.Position;
            node.Position += velocity * (1f - Friction) + StepTime * acceleration;
            Nodes[index] = node;
        }
    }


    [BurstCompile]
    public struct UpdateJob : IJob
    {
        public int Iterations;
        public NativeArray<Node> Nodes;

        public void Execute()
        {
            for (int i = 0; i < Iterations; i++)
            {
                ApplyConstraints();
            }
        }

        private void ApplyConstraints()
        {
            for (int i = 0; i < Nodes.Length; i++)
            {
                if (i + 1 >= Nodes.Length) break;

                var current = Nodes[i];
                var next = Nodes[i + 1];

                var distance = math.distance(next.Position, current.Position);
                var value = (distance - NODE_DISTANCE) * 0.99f;

                var currentPosition = current.Position;

                if (!current.Constrained)
                {
                    current.Position += math.normalize(next.Position - current.Position) * value;
                }

                if (!next.Constrained)
                {
                    next.Position += math.normalize(currentPosition - next.Position) * value;
                }

                Nodes[i] = current;
                Nodes[i + 1] = next;
            }
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Node : IEquatable<Node>
    {
        public float2 Position;
        public float2 Previous;

        public bool Constrained;

        public float2 Velocity => Position - Previous;

        public Node(float2 position)
        {
            Position = position;
            Previous = position;
            Constrained = false;
        }

        public bool Equals(Node other)
        {
            return Position.Equals(other.Position) && Previous.Equals(other.Previous);
        }
    }
}

