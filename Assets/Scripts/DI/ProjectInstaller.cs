using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private Config _config;
    [SerializeField] private GenerateConfigs _generateConfig;

    [SerializeField] private Canvas _canvasPrefab;
    [SerializeField] private RegionView _regionViewPrefab;
    [SerializeField] private MainWindow _mainWindow;

    public override void InstallBindings()
    {
        InstallConfigs();
        InstallPrefabs();
        InstallModel();
        InstallServices();
        InstallUI();
        InstallSignals();
    }

    private void InstallConfigs()
    {
        Container.BindInstance(_config).AsSingle();
        Container.BindInstance(_generateConfig).AsSingle();
    }

    private void InstallPrefabs()
    {
        Container.BindInstance(_canvasPrefab).WithId(Dicts.DiPrefabIds.Canvas);
        Container.Bind<RegionView>().FromComponentInNewPrefab(_regionViewPrefab).AsTransient();
    }

    private void InstallModel()
    {
        Container.BindInterfacesAndSelfTo<WindowModelBase>().AsSingle();
    }

    private void InstallServices()
    {
        Container.BindInterfacesAndSelfTo<RegionsHandlerModel>().AsSingle();
        Container.BindInterfacesAndSelfTo<ConfigService>().AsSingle();
        Container.BindInterfacesAndSelfTo<GeneratorService>().AsSingle();
        Container.BindInterfacesAndSelfTo<RegionsHandlerService>().AsSingle();
        Container.BindInterfacesAndSelfTo<UIService>().AsSingle();
    }

    private void InstallUI()
    {
        Container.Bind<MainWindow>().FromComponentInNewPrefab(_mainWindow).AsSingle();
    }

    private void InstallSignals()
    {
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<FailSignal>();
        Container.DeclareSignal<OpenRegionsSignal>();
        Container.DeclareSignal<StartNewGameSignal>();
        Container.DeclareSignal<RegionUnFocusSignal>();
        Container.DeclareSignal<RegionFocusSignal>();
        Container.DeclareSignal<RegionClickSignal>();
        Container.DeclareSignal<RestartSignal>();
        Container.DeclareSignal<RegionsGeneratedSignal>();
        Container.DeclareSignal<UpdateRegionSignal>();
        Container.DeclareSignal<WinSignal>();
    }
}