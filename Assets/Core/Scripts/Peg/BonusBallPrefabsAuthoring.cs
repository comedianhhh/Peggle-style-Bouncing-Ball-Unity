using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BonusBallPrefabsAuthoring : MonoBehaviour
{
    public List<GameObject> bonusBallPrefabs;

    class BonusBallPrefabsBaker : Baker<BonusBallPrefabsAuthoring>
    {
        public override void Bake(BonusBallPrefabsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            var buffer = AddBuffer<BonusBallPrefabs>(entity);
            foreach (var prefab in authoring.bonusBallPrefabs)
            {
                buffer.Add(new BonusBallPrefabs
                {
                    Value = GetEntity(prefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}
