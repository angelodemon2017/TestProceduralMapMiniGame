using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(MeshCollider), typeof(MeshFilter), typeof(MeshRenderer))]
public class RegionView : MonoBehaviour
{
    [SerializeField] private Config _config;
    [SerializeField] private MeshCollider _meshCollider;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;

    private RegionsHandlerModel _regionsHandlerModel;
    private ConfigService _configService;
    private SignalBus _signalBus;

    [SerializeField] private Text _text;
    [SerializeField] private Canvas _canvas;
    private Region _region;
    
    private int _id => _region.Id;
    private bool _isOpen => _region.IsOpen;

    public MeshFilter MeshFilter => _meshFilter;

    private void OnValidate()
    {
        _meshCollider = GetComponent<MeshCollider>();
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    [Inject]
    private void Construct(
        ConfigService configService,
        SignalBus signalBus,
        RegionsHandlerModel regionsHandlerModel)
    {
        _configService = configService;
        _signalBus = signalBus;
        _regionsHandlerModel = regionsHandlerModel;

        InitSignals();
    }

    private void InitSignals()
    {
        _signalBus.Subscribe<FailSignal>(OnHandle);
    }

    private void OnHandle(FailSignal fail)
    {
        if (!_isOpen)
        {
            ApplyMaterial(_configService.GetFailRegionMaterial());
        }
    }

    public void InitRegion(Region _region)
    {
        this._region = _region;
        _meshCollider.sharedMesh = _meshFilter.mesh;
        _canvas.transform.position = this._region.Center;
        _text.text = $"{_id}";

        OnUpdated();
    }

    public void OnMouseEnter()
    {
        _signalBus.Fire(new RegionFocusSignal(_id));
    }

    public void OnMouseExit()
    {
        _signalBus.Fire(new RegionUnFocusSignal(_id));
    }

    public void OnMouseDown()
    {
        _signalBus.Fire(new RegionClickSignal(_id));
    }

    public void OnUpdated()
    {
        ApplyMaterial(_regionsHandlerModel.CurrentId == _id ? _configService.GetCurrentRegionMaterial() :
           _region.IsOpen ? _configService.GetActiveRegionMaterial() :
           _region.IsFocused ? _configService.GetFocusedMaterial() :
           _configService.GetStandartMaterial());
    }

    private void ApplyMaterial(Material material)
    {
        _meshRenderer.material = material;
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<FailSignal>(OnHandle);
    }
}