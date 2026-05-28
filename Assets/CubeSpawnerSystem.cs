using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;

// We use 'partial' so the Burst compiler can generate optimized code for us.
public partial struct CubeSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        //var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var spawner in SystemAPI.Query<RefRW<CubeSpawner>>())
        {
            int amount = spawner.ValueRO.Amount;

            int sideCount = (int)math.ceil(math.pow(amount, 1f / 3f));
            int spawnedCount = 0;

            var prefabCollider =
                state.EntityManager.GetComponentData<PhysicsCollider>(
                    spawner.ValueRO.Prefab);

            for (int x = 0; x < sideCount; x++)
            {
                for (int y = 0; y < sideCount; y++)
                {
                    for (int z = 0; z < sideCount; z++)
                    {
                        if (spawnedCount >= amount)
                            break;

                        Entity instance = ecb.Instantiate(spawner.ValueRO.Prefab);

                        float3 position = new float3(x, y, z) * 1.5f;

                        // SET transform instead of add
                        ecb.SetComponent(instance, LocalTransform.FromPosition(position));

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