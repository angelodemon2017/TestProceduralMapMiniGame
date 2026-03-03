using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class RegionsViewController : MonoBehaviour
{
    private DiContainer _container;
    private SignalBus _signalBus;
    private GenerateConfigs _generateConfigs;

    private Dictionary<int, RegionView> _cashRegionsView = new Dictionary<int, RegionView>();

    [Inject]
    private void Constructor(
        DiContainer container,
        SignalBus signalBus,
        GenerateConfigs generateConfigs)
    {
        _container = container;
        _signalBus = signalBus;
        _generateConfigs = generateConfigs;

        InitSignals();
    }

    private void InitSignals()
    {
        _signalBus.Subscribe<RegionsGeneratedSignal>(OnHandle);
        _signalBus.Subscribe<UpdateRegionSignal>(OnHandle);
    }

    private void OnHandle(RegionsGeneratedSignal regionsGenerated)
    {
        _cashRegionsView.Clear();
        ClearPreviousChildren();

        foreach (var region in regionsGenerated.Regions)
        {
            if (region.Vertices == null || region.Vertices.Count < 3)
                continue;

            var rv = _container.Resolve<RegionView>();
            var go = rv.gameObject;

            go.transform.rotation = Quaternion.identity;

            go.transform.SetParent(transform, true);

            go.name = $"Region_{region.Id:000}";

            var mesh = ProceduralRegion.CreateRegionMesh(region.Vertices, region.Center, 0f, _generateConfigs.RegionInset);
            rv.MeshFilter.mesh = mesh;
            rv.InitRegion(region);

            _cashRegionsView.Add(region.Id, rv);
        }
    }

    private void OnHandle(UpdateRegionSignal regionClick)
    {
        if (_cashRegionsView.TryGetValue(regionClick.Id, out RegionView region))
        {
            region.OnUpdated();
        }
    }

    private void ClearPreviousChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<RegionsGeneratedSignal>(OnHandle);
        _signalBus.Unsubscribe<UpdateRegionSignal>(OnHandle);
    }
}