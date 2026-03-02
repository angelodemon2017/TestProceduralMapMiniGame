using Zenject;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        var uiservice = Container.Resolve<UIService>();
        uiservice.ChangeWindow<MainWindow>();
    }
}