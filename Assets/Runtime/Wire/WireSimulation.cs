using Unity.Jobs;
using Unity.Burst;

using Unity.Mathematics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System;
using Unity.Collections;

public static class WireSimulation
{
    public const int ITERATIONS = 50;
    public const float NODE_DISTANCE = .25f;

    [BurstCompile]
    public struct UpdateJob : IJob
    {
        // Parameters
        public float StepTime;
        public float Executions;
        public float2 Friction;
        public float2 Gravity;

        // Nodes
        public NativeArray<Node> Nodes;

        public void Execute()
        {
            for (int i = 0; i < Executions; i++)
            {
                Simulate();
                for (int j = 0; j < ITERATIONS; j++)
                {
                    ApplyConstraints();
                }
            }
        }

        private void Simulate()
        {
            var stepTimeSq = math.sqrt(StepTime);
            for (int i = 0; i < Nodes.Length; i++)
            {
                var node = Nodes[i];

                if (node.Constrained) continue;

                var velocity = node.Velocity;
                var acceleration = Gravity;

                // Limiting maximum move gives the simulation more stability.
                if (math.length(velocity) > 1f)
                {
                    velocity = math.normalize(velocity) * 1f;
                }

                node.Previous = node.Position;
                node.Position += velocity * (1f - Friction) + stepTimeSq * acceleration;

                Nodes[i] = node;
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

