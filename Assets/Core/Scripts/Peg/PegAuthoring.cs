using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Unity.Entities;

public class PegAuthoring : MonoBehaviour
{
    public PegType type;
    public int pointValue;
    public int bonusBallAmount = 3;
    
    public class PegAuthoringBaker : Baker<PegAuthoring>
    {
        public override void Bake(PegAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
        
            AddComponent(entity, new PegTag());
            AddComponent(entity, new Peg
            {
                Type = authoring.type,
                PointValue = authoring.pointValue,
                IsHit = false,
                bonusBallAmount = authoring.bonusBallAmount
            });
        }
    }
}