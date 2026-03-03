using UnityEngine;

[CreateAssetMenu(fileName = "MaterialRegionConfig", menuName = "Scriptable Objects/MaterialRegionConfig")]
public class MaterialRegionConfig : ScriptableObject
{
    public AssetReferenceMaterial StandRegionMat;
    public AssetReferenceMaterial ActiveRegionMat;
    public AssetReferenceMaterial CurrentActivRegionMat;
    public AssetReferenceMaterial FocusRegionMat;
    public AssetReferenceMaterial FailRegionMat;
}