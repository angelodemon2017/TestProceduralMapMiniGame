using UnityEngine;

[CreateAssetMenu(fileName = "GenerateConfigs", menuName = "Scriptable Objects/GenerateConfigs")]
public class GenerateConfigs : ScriptableObject
{
    [Header("Настройки карты")]
    public Vector2 MapSize = new Vector2(100, 100);
    public int TotalSites = 120;
    public int CentralDensity = 75;

    [Header("Отступы")]
    public float RegionInset = 0.5f;
}