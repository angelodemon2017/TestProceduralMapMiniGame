using UnityEngine;
using Zenject;

public class UIService
{
    private DiContainer _container;
    private IWindowBase _mainRootWindow;

    private Canvas _rootCanvas;

    public UIService(
        DiContainer container,
        [Inject(Id = Dicts.DiPrefabIds.Canvas)] Canvas canvasPrefab)
    {
        _container = container;
        _rootCanvas = GameObject.Instantiate(canvasPrefab);
        GameObject.DontDestroyOnLoad(_rootCanvas);
    }

    public TState ChangeWindow<TState>()
        where TState : IWindowBase
    {
        var nextWindow = _container.Resolve<TState>();
        nextWindow.GetTransform.SetParent(_rootCanvas.transform, false);

        ShowWindowAsync(nextWindow);
        return nextWindow;
    }

    private void ShowWindowAsync(IWindowBase window)
    {
        if (window == _mainRootWindow) return;
        if (_mainRootWindow != null)
        {
            _mainRootWindow.Hide();
        }

        _mainRootWindow = window;
        _mainRootWindow.Show();
    }
}