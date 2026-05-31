using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;




public class BoxContainerAuthor : MonoBehaviour
{

    class Baker : Baker<BoxContainerAuthor>
    {
        public override void Bake(BoxContainerAuthor authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new BoxContainer
            {
                worldPosition = authoring.transform.position,
                width = 3f,
                height = 3f,
                depth = 3f
            });
        }
    }

}

public struct BoxContainer : IComponentData
{
    public float3 worldPosition;
    public float width;
    public float height;
    public float depth;
}
