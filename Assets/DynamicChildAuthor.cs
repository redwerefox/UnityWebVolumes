using Unity.Entities;
using UnityEngine;

public class DynamicChildAuthor : MonoBehaviour
{
    class Baker : Baker<DynamicChildAuthor>
    {
        public override void Bake(DynamicChildAuthor author)
        {
            // Requesting Dynamic here tells Unity not to flatten this child
            GetEntity(TransformUsageFlags.Dynamic);
        }
    }
}