using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using System.Linq;
using UnityEngine.TestTools;



// We use 'partial' so the Burst compiler can generate optimized code for us.
public partial struct CubeSpawnerSystem : ISystem
{

    bool hasSpawned;

    public void OnCreate(ref SystemState state) { hasSpawned = false; }


    private float3 PositionInsideBoxContainer(in float norm_x, in float norm_y, in float norm_z, BoxContainer container)
    {
        return container.worldPosition + new float3(
            (norm_x - 0.5f) * container.width,
            (norm_y - 0.5f) * container.height,
            (norm_z - 0.5f) * container.depth
        );
    }

    private float3 FakeRandomRotations(in float3 position)
    {
        return math.sin(position * 0.1f) * 360f;
    }

    private float ScaleByContainerSizeAndAmount(BoxContainer container, int amount, float gapBetweenCubes)
    {
        if (gapBetweenCubes <= 0f)
        {
            //clamp for safety
            gapBetweenCubes = 0.1f;
        }
        float containerVolume = container.width * container.height * container.depth;
        float cubeVolume = containerVolume / amount;
        return math.pow(cubeVolume * gapBetweenCubes, 1f / 3f);
    }

    public void OnUpdate(ref SystemState state)
    {
        if (hasSpawned)
            return;

        //var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        BoxContainer containerInfo = SystemAPI.GetSingleton<BoxContainer>();

        foreach (var spawner in SystemAPI.Query<RefRW<CubeSpawner>>())
        {

            int amount = spawner.ValueRO.Amount;
            float gapBetweenCubes = spawner.ValueRO.GapBetweenCubes;

            int sideCount = (int)math.ceil(math.pow(amount, 1f / 3f));
            int spawnedCount = 0;

            var prefabCollider = SystemAPI.GetComponent<PhysicsCollider>(spawner.ValueRO.Prefab);

            for (int x = 0; x < sideCount; x++)
            {
                for (int y = 0; y < sideCount; y++)
                {
                    for (int z = 0; z < sideCount; z++)
                    {
                        if (spawnedCount >= amount)
                            break;

                        Entity instance = ecb.Instantiate(spawner.ValueRO.Prefab);

                        float3 position = PositionInsideBoxContainer((float)x / sideCount, (float)y / sideCount, (float)z / sideCount, containerInfo);
                        float3 rotation = FakeRandomRotations(position);
                        float scale = ScaleByContainerSizeAndAmount(containerInfo, amount, gapBetweenCubes);

                        // SET transform instead of add
                        ecb.SetComponent(instance,
                        LocalTransform.FromPositionRotationScale(
                            position,
                            quaternion.EulerXYZ(math.radians(rotation)),
                            scale)
                        );

                        ecb.SetName(instance, new Unity.Collections.FixedString32Bytes("Spawned Cube"));

                        float randomValue = UnityEngine.Random.value;

                        float mass;
                        float3 color;

                        if (randomValue < 0.2f)
                        {
                            mass = 1f;
                            color = new float3(0.8f, 0.1f, 0.05f);
                        }
                        else if (randomValue < 0.4f)
                        {
                            mass = 2f;
                            color = new float3(0f, 0.8f, 0.1f);
                        }
                        else if (randomValue < 0.6f)
                        {
                            mass = 3f;
                            color = new float3(0f, 0f, 0.8f);
                        }
                        else if (randomValue < 0.8f)
                        {
                            mass = 4f;
                            color = new float3(0.8f, 0.8f, 0.1f);
                        }
                        else
                        {
                            mass = 5f;
                            color = new float3(0.9f, 0f, 0.8f);
                        }

                        ecb.SetComponent(instance, new CubeData
                        {
                            mass = mass,
                            color = color
                        });

                        // SET physics mass
                        ecb.SetComponent(instance,
                            PhysicsMass.CreateDynamic(
                                prefabCollider.MassProperties,
                                mass
                            ));

                        ecb.SetComponent(instance, new URPMaterialPropertyBaseColor
                        {
                            Value = new float4((float3)color, 1f)
                        });

                        spawnedCount++;
                    }
                }
            }


            state.Enabled = false;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

    }
}