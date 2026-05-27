using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

// We use 'partial' so the Burst compiler can generate optimized code for us.
public partial struct CubeSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        // SystemAPI.Query finds all entities in the world that have our CubeSpawner data.
        foreach (var spawner in SystemAPI.Query<RefRW<CubeSpawner>>())
        {

            int amount = spawner.ValueRO.Amount;

            // Calculate the size of one side of the cube (cubic root)
            int sideCount = (int)math.ceil(math.pow(amount, 1f / 3f));
            int spawnedCount = 0;

            for (int x = 0; x < sideCount; x++)
            {
                for (int y = 0; y < sideCount; y++)
                {
                    for (int z = 0; z < sideCount; z++)
                    {
                        // Stop if we hit the limit set in the Inspector
                        if (spawnedCount >= amount) break;

                        // Instantiate creates a new Entity copy of the prefab
                        Entity instance = state.EntityManager.Instantiate(spawner.ValueRO.Prefab);

                        // Calculate 3D position with 1.5 units of spacing
                        float3 position = new float3(x, y, z) * 1.5f;

                        // Set the ECS Transform component
                        state.EntityManager.SetComponentData(instance, new LocalTransform
                        {
                            Position = position,
                            Rotation = quaternion.identity,
                            Scale = 1f
                        });

                        spawnedCount++;
                    }
                }
            }

            // CRITICAL: Disable the system so it only spawns once on the first frame.
            state.Enabled = false;
        }
    }
}