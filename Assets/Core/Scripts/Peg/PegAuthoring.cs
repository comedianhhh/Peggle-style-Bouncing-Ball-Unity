using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Unity.Entities;

public class PegAuthoring : MonoBehaviour
{
    public PegType pegType;
    public int pointValue;
    
    public class PegAuthoringBaker : Baker<PegAuthoring>
    {
        public override void Bake(PegAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
        
            AddComponent(entity, new PegTag());
            AddComponent(entity, new Peg
            {
                Type = authoring.pegType,
                PointValue = authoring.pointValue,
                IsHit = false
            });
        }
    }
}

// We need a component to hold the PegType enum.