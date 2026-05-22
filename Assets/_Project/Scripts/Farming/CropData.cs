using UnityEngine;

[CreateAssetMenu(fileName = "SO_Crop", menuName = "MagicFarm/Crop Data")]
public class CropData : ScriptableObject
{
    public string cropName;
    public CropType cropType;
    public float growthTime = 30f;
    public int sellPrice = 10;
    public int xpReward = 5;
    public int maxHP = 30;
    public Sprite[] stageSprites = new Sprite[4];
}
