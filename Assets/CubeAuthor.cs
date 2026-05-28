using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;

public class CubeAuthor : MonoBehaviour
{
    public float mass;
    public float3 color;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public class CubeBaker : Baker<CubeAuthor>
    {
        public override void Bake(CubeAuthor authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new CubeData
            {
                mass = authoring.mass,
                color = authoring.color
            });

            AddComponent(entity, new URPMaterialPropertyBaseColor
            {
                Value = new float4((float3)authoring.color, 1f)
            });
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

public struct CubeData : IComponentData
{
    public float mass;

    public float3 color;
}
