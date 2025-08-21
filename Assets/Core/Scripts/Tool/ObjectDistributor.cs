using UnityEngine;

[System.Serializable]
public class ObjectDistributor : MonoBehaviour
{
    public Transform[] objectsToDistribute;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public bool distributeOnStart = true;
    
    [Space]
    public DistributionType distributionType = DistributionType.Linear;
    public AnimationCurve distributionCurve = AnimationCurve.Linear(0, 0, 1, 1);
    
    void Start()
    {
        if (distributeOnStart)
        {
            DistributeObjects();
        }
    }
    
    public void DistributeObjects()
    {
        if (objectsToDistribute == null || objectsToDistribute.Length == 0)
            return;
            
        int count = objectsToDistribute.Length;
        
        switch (distributionType)
        {
            case DistributionType.Linear:
                DistributeLinear(count);
                break;
            case DistributionType.Circle:
                DistributeCircle(count);
                break;
            case DistributionType.Grid:
                DistributeGrid(count);
                break;
        }
    }
    
    void DistributeLinear(int count)
    {
        Vector3 direction = endPosition - startPosition;
        
        for (int i = 0; i < count; i++)
        {
            float t = count == 1 ? 0 : (float)i / (count - 1);
            Vector3 position = startPosition + direction * t;
            objectsToDistribute[i].position = position;
        }
    }
    
    void DistributeCircle(int count)
    {
        float radius = Vector3.Distance(startPosition, endPosition);
        Vector3 center = startPosition;
        
        for (int i = 0; i < count; i++)
        {
            float angle = (float)i / count * 360f * Mathf.Deg2Rad;
            Vector3 position = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );
            objectsToDistribute[i].position = position;
        }
    }
    
    void DistributeGrid(int count)
    {
        int cols = Mathf.CeilToInt(Mathf.Sqrt(count));
        int rows = Mathf.CeilToInt((float)count / cols);
        
        Vector3 spacing = new Vector3(
            (endPosition.x - startPosition.x) / Mathf.Max(1, cols - 1),
            0,
            (endPosition.z - startPosition.z) / Mathf.Max(1, rows - 1)
        );
        
        for (int i = 0; i < count; i++)
        {
            int row = i / cols;
            int col = i % cols;
            
            Vector3 position = startPosition + new Vector3(
                col * spacing.x,
                0,
                row * spacing.z
            );
            
            objectsToDistribute[i].position = position;
        }
    }
    
    void OnDrawGizmos()
    {
        if (objectsToDistribute == null) return;
        
        // 绘制分布线
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPosition, endPosition);
        
        // 绘制分布点预览
        Gizmos.color = Color.red;
        int count = objectsToDistribute.Length;
        
        if (count > 0)
        {
            switch (distributionType)
            {
                case DistributionType.Linear:
                    DrawLinearPreview(count);
                    break;
                case DistributionType.Circle:
                    DrawCirclePreview(count);
                    break;
                case DistributionType.Grid:
                    DrawGridPreview(count);
                    break;
            }
        }
    }
    
    void DrawLinearPreview(int count)
    {
        Vector3 direction = endPosition - startPosition;
        for (int i = 0; i < count; i++)
        {
            float t = count == 1 ? 0 : (float)i / (count - 1);
            Vector3 position = startPosition + direction * t;
            Gizmos.DrawWireSphere(position, 0.2f);
        }
    }
    
    void DrawCirclePreview(int count)
    {
        float radius = Vector3.Distance(startPosition, endPosition);
        Vector3 center = startPosition;
        
        for (int i = 0; i < count; i++)
        {
            float angle = (float)i / count * 360f * Mathf.Deg2Rad;
            Vector3 position = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );
            Gizmos.DrawWireSphere(position, 0.2f);
        }
    }
    
    void DrawGridPreview(int count)
    {
        int cols = Mathf.CeilToInt(Mathf.Sqrt(count));
        int rows = Mathf.CeilToInt((float)count / cols);
        
        Vector3 spacing = new Vector3(
            (endPosition.x - startPosition.x) / Mathf.Max(1, cols - 1),
            0,
            (endPosition.z - startPosition.z) / Mathf.Max(1, rows - 1)
        );
        
        for (int i = 0; i < count; i++)
        {
            int row = i / cols;
            int col = i % cols;
            
            Vector3 position = startPosition + new Vector3(
                col * spacing.x,
                0,
                row * spacing.z
            );
            
            Gizmos.DrawWireSphere(position, 0.2f);
        }
    }
}

public enum DistributionType
{
    Linear,
    Circle,
    Grid
}