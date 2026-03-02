using UnityEngine;

[CreateAssetMenu(fileName = "Config", menuName = "Scriptable Objects/Config")]
public class Config : ScriptableObject
{
    public AssetReferenceMaterial StandRegionMat;
    public AssetReferenceMaterial ActiveRegionMat;
    public AssetReferenceMaterial CurrentActivRegionMat;
    public AssetReferenceMaterial FocusRegionMat;
    public AssetReferenceMaterial FailRegionMat;
}