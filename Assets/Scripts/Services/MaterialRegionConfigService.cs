using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class MaterialRegionConfigService
{
    private readonly SignalBus _signalBus;
    private readonly MaterialRegionConfig _materialRegionConfig;

    private Material _currentRegionMaterial;
    private Material _standartMaterial;
    private Material _focusedMaterial;
    private Material _activeRegionMaterial;
    private Material _failRegionMaterial;

    public MaterialRegionConfigService(
        SignalBus signalBus,
        MaterialRegionConfig config)
    {
        _signalBus = signalBus;
        _materialRegionConfig = config;

        _ = InitMaterialsAsync();

        Debug.Log("ConfigService Inited");
    }

    private async Task InitMaterialsAsync()
    {
        _currentRegionMaterial = await _materialRegionConfig.CurrentActivRegionMat.LoadAssetAsync<Material>().Task;
        _standartMaterial = await _materialRegionConfig.StandRegionMat.LoadAssetAsync<Material>().Task;
        _focusedMaterial = await _materialRegionConfig.FocusRegionMat.LoadAssetAsync<Material>().Task;
        _activeRegionMaterial = await _materialRegionConfig.ActiveRegionMat.LoadAssetAsync<Material>().Task;
        _failRegionMaterial = await _materialRegionConfig.FailRegionMat.LoadAssetAsync<Material>().Task;

        _signalBus.Fire(new StartNewGameSignal());
    }

    public Material GetCurrentRegionMaterial() => _currentRegionMaterial;
    public Material GetStandartMaterial() => _standartMaterial;
    public Material GetFocusedMaterial() => _focusedMaterial;
    public Material GetActiveRegionMaterial() => _activeRegionMaterial;
    public Material GetFailRegionMaterial() => _failRegionMaterial;
}