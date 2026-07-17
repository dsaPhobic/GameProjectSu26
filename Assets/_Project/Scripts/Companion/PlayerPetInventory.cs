using System.Collections.Generic;
using UnityEngine;

public static class PlayerPetInventory
{
    private class EggStack
    {
        public PetEggData egg;
        public int count;
    }

    private static readonly List<EggStack> SharedEggs = new();
    private static readonly List<GameObject> PersistentPets = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSharedRuntimeState()
    {
        SharedEggs.Clear();
        PersistentPets.Clear();
    }

    public static void RegisterPersistentPet(GameObject pet)
    {
        if (pet != null && !PersistentPets.Contains(pet))
            PersistentPets.Add(pet);
    }

    public static void ResetProgress()
    {
        SharedEggs.Clear();
        foreach (GameObject pet in PersistentPets)
        {
            if (pet != null) Object.Destroy(pet);
        }
        PersistentPets.Clear();
    }

    public static void AddEgg(PetEggData egg, int amount = 1)
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
