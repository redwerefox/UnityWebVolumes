using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;


public partial struct RotateContainer : ISystem
{

    public void OnUpdate(ref SystemState state)
    {

        if (SystemAPI.Time.ElapsedTime < 0.5)
        {
            return;
        }


        float mouseInputX = Input.GetAxis("Mouse X");
        float mouseInputY = Input.GetAxis("Mouse Y");

        mouseInputX *= SystemAPI.Time.DeltaTime * 1000f;
        mouseInputY *= SystemAPI.Time.DeltaTime * 1000f;


        float maxSpinSpeed = 5.0f;
        float spinSpeed = 2.0f;

        foreach (var transform in SystemAPI.Query<RefRW<PhysicsVelocity>>().WithAll<Container>())
        {

            float3 angularVelocity = new float3(-mouseInputY * spinSpeed, mouseInputX * spinSpeed, 0f);
            float angularVelocityMagnitude = math.length(angularVelocity);
            if (angularVelocityMagnitude > maxSpinSpeed)
            {
                angularVelocity = math.normalize(angularVelocity) * maxSpinSpeed;
            }
            transform.ValueRW.Angular = angularVelocity;
        }

    }
}
