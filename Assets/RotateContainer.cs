using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;


public partial struct RotateContainer : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float mouseInputX = Input.GetAxis("Mouse X");
        float mouseInputY = Input.GetAxis("Mouse Y");

        mouseInputX *= SystemAPI.Time.DeltaTime * 1000f;
        mouseInputY *= SystemAPI.Time.DeltaTime * 1000f;


        foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<Container>())
        {

            Debug.Log("Found the Container Entity!");
            Debug.Log($"Mouse Input X: {mouseInputX}, Mouse Input Y: {mouseInputY}");
            transform.ValueRW.Rotation = math.mul(
            transform.ValueRO.Rotation,
            quaternion.Euler(math.radians(new float3(-mouseInputY, mouseInputX, 0f)))
        );

        }
    }
}
