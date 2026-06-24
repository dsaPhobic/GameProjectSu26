using System.Collections.Generic;
using UnityEngine;

public class FarmEntrance : MonoBehaviour
{
    private static readonly List<FarmEntrance> Entrances = new();

    private void OnEnable()
    {
        if (!Entrances.Contains(this))
            Entrances.Add(this);
    }

    private void OnDisable()
    {
        Entrances.Remove(this);
    }

    public static Transform GetNearest(Vector2 position)
    {
        FarmEntrance nearest = null;
        float nearestSqrDistance = float.MaxValue;

        foreach (var entrance in Entrances)
        {
            if (entrance == null) continue;

            float sqrDistance = ((Vector2)entrance.transform.position - position).sqrMagnitude;
            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearest = entrance;
            }
        }

        return nearest != null ? nearest.transform : null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }
}
