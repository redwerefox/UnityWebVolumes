using Unity.Entities;
using UnityEngine;

public class CubeSpawnerAuthor : MonoBehaviour
{
    public GameObject CubePrefab;
    public int Amount;

    public float gapBetweenCubes = 0.9f;

    class Baker : Baker<CubeSpawnerAuthor>
    {
        public override void Bake(CubeSpawnerAuthor authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new CubeSpawner
            {
                Prefab = GetEntity(authoring.CubePrefab, TransformUsageFlags.Dynamic),
                Amount = authoring.Amount,
                GapBetweenCubes = authoring.gapBetweenCubes
            });
        }
    }
}

public struct CubeSpawner : IComponentData
{
    public Entity Prefab;
    public int Amount;
    public float GapBetweenCubes;
}
