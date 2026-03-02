using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class ConfigService
{
    private readonly SignalBus _signalBus;
    private readonly Config _config;

    private Material _currentRegionMaterial;
    private Material _standartMaterial;
    private Material _focusedMaterial;
    private Material _activeRegionMaterial;
    private Material _failRegionMaterial;

    public ConfigService(
        SignalBus ssignalBus,
        Config config)
    {
        _signalBus = ssignalBus;
        _config = config;

        _ = InitMaterialsAsync();

        Debug.Log("ConfigService Inited");
    }

    private async Task InitMaterialsAsync()
    {
        _currentRegionMaterial = await _config.CurrentActivRegionMat.LoadAssetAsync<Material>().Task;
        _standartMaterial = await _config.StandRegionMat.LoadAssetAsync<Material>().Task;
        _focusedMaterial = await _config.FocusRegionMat.LoadAssetAsync<Material>().Task;
        _activeRegionMaterial = await _config.ActiveRegionMat.LoadAssetAsync<Material>().Task;
        _failRegionMaterial = await _config.FailRegionMat.LoadAssetAsync<Material>().Task;

        _signalBus.Fire(new StartNewGameSignal());
    }

    public Material GetCurrentRegionMaterial() => _currentRegionMaterial;
    public Material GetStandartMaterial() => _standartMaterial;
    public Material GetFocusedMaterial() => _focusedMaterial;
    public Material GetActiveRegionMaterial() => _activeRegionMaterial;
    public Material GetFailRegionMaterial() => _failRegionMaterial;
}