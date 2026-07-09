using UnityEngine;

[CreateAssetMenu(fileName = "SO_PetEgg", menuName = "MagicFarm/Pet Egg Data")]
public class PetEggData : ScriptableObject
{
    public string eggName = "Blue Egg";
    public Sprite eggSprite;
    public RuntimeAnimatorController hatchAnimatorController;
    public GameObject petPrefab;
    public int price = 50;
    public float hatchSeconds = 30f;
}
