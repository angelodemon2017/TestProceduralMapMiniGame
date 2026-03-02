using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainWindow : UIWindowBase<WindowModelBase>
{
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private TextMeshProUGUI _titleText;

    private SignalBus _signalBus;

    [Inject]
    private void Construct(
        SignalBus signalBus)
    {
        _signalBus = signalBus;

        InitSignals();
    }

    private void InitSignals()
    {
        _signalBus.Subscribe<OpenRegionsSignal>(OnHandle);
        _signalBus.Subscribe<WinSignal>(OnHandle);
        _signalBus.Subscribe<FailSignal>(OnHandle);
    }

    private void OnHandle(OpenRegionsSignal openRegions)
    {
        _titleText.text = $"regions: {openRegions.Opens}/{openRegions.Max}";
        _titleText.color = Color.white;
    }

    private void OnHandle(WinSignal win)
    {
        _titleText.text = "Win!";
        _titleText.color = Color.green;
    }

    private void OnHandle(FailSignal fail)
    {
        _titleText.text = "Fail =(";
        _titleText.color = Color.red;
    }

    public override void Show()
    {
        base.Show();
        _newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        _restartButton.onClick.AddListener(RestartButtonClicked);
    }

    private void OnNewGameButtonClicked()
    {
        _signalBus.Fire(new StartNewGameSignal());
        _restartButton.interactable = true;
    }

    private void RestartButtonClicked()
    {
        _signalBus.Fire(new RestartSignal());
    }

    public override void Hide()
    {
        base.Hide();
        _newGameButton.onClick.RemoveListener(OnNewGameButtonClicked);
        _restartButton.onClick.RemoveListener(RestartButtonClicked);
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<OpenRegionsSignal>(OnHandle);
        _signalBus.Unsubscribe<WinSignal>(OnHandle);
        _signalBus.Unsubscribe<FailSignal>(OnHandle);
    }
}