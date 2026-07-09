using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPetInventory : MonoBehaviour
{
    [Serializable]
    private class EggStack
    {
        public PetEggData egg;
        public int count;
    }

    private static readonly List<EggStack> SharedEggs = new();

    public void AddEgg(PetEggData egg, int amount = 1)
    {
        if (egg == null || amount <= 0) return;

        EggStack stack = FindStack(egg, SharedEggs);
        if (stack == null)
        {
            stack = new EggStack { egg = egg };
            SharedEggs.Add(stack);
        }

        stack.count += amount;
        Debug.Log($"Received {amount} {egg.eggName}. Total: {stack.count}");
    }

    public bool TryConsumeFirstEgg(out PetEggData egg)
    {
        return TryConsumeAnyEgg(out egg);
    }

    public bool HasAnyEgg()
    {
        return HasSharedEgg();
    }

    public static bool TryConsumeAnyEgg(out PetEggData egg)
    {
        egg = null;

        for (int i = 0; i < SharedEggs.Count; i++)
        {
            EggStack stack = SharedEggs[i];
            if (stack == null || stack.egg == null || stack.count <= 0) continue;

            stack.count--;
            egg = stack.egg;
            Debug.Log($"Placed {egg.eggName}. Remaining eggs: {stack.count}");
            return true;
        }

        return false;
    }

    public static bool HasSharedEgg()
    {
        for (int i = 0; i < SharedEggs.Count; i++)
        {
            EggStack stack = SharedEggs[i];
            if (stack != null && stack.egg != null && stack.count > 0)
                return true;
        }

        return false;
    }

    private static EggStack FindStack(PetEggData egg, List<EggStack> eggs)
    {
        for (int i = 0; i < eggs.Count; i++)
        {
            if (eggs[i] != null && eggs[i].egg == egg)
                return eggs[i];
        }

        return null;
    }
}
