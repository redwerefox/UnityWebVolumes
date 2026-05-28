using UnityEngine;
using Unity.Transforms;
using Unity.Entities;


public struct Container : IComponentData
{ }

public class ContainerAuthor : MonoBehaviour
{
    class Baker : Baker<ContainerAuthor>
    {
        public override void Bake(ContainerAuthor author)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<Container>(entity);
        }
    }
}
