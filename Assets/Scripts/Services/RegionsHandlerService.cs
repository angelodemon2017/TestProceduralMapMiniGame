using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class RegionsHandlerService : IDisposable
{
    private readonly RegionsHandlerModel _regionsHandlerModel;
    private readonly GenerateConfigs _generateConfigs;
    private readonly SignalBus _signalBus;

    private int _currentId => _regionsHandlerModel.CurrentId;
    private int _centerId = -1;
    private List<Region> _regions = new List<Region>();
    private Dictionary<int, Region> _cashRegions = new Dictionary<int, Region>();

    private Vector2 _mapSize => _generateConfigs.MapSize;

    public RegionsHandlerService(
        RegionsHandlerModel regionsHandlerModel,
        GenerateConfigs generateConfigs,
        SignalBus signalBus)
    {
        _regionsHandlerModel = regionsHandlerModel;
        _generateConfigs = generateConfigs;
        _signalBus = signalBus;

        InitSignals();
    }

    private void InitSignals()
    {
        _signalBus.Subscribe<RestartSignal>(OnHandle);
        _signalBus.Subscribe<RegionFocusSignal>(OnHandle);
        _signalBus.Subscribe<RegionUnFocusSignal>(OnHandle);
        _signalBus.Subscribe<RegionClickSignal>(OnHandle);
        _signalBus.Subscribe<RegionsGeneratedSignal>(OnHandle);
    }

    private void OnHandle(RegionFocusSignal regionFocus)
    {
        if (_currentId == -1 || regionFocus.Id == _currentId)
            return;

        var focusRegion = _cashRegions[regionFocus.Id];

        if (!focusRegion.IsOpen && !focusRegion.IsFocused &&
            focusRegion.IsNeighbor(_currentId))
        {
            focusRegion.IsFocused = true;
             _signalBus.Fire(new UpdateRegionSignal(regionFocus.Id));
        }
    }

    private void OnHandle(RegionUnFocusSignal regionUnFocus)
    {
        var focusRegion = _cashRegions[regionUnFocus.Id];

        if (!focusRegion.IsOpen && focusRegion.IsFocused)
        {
            focusRegion.IsFocused = false;
            _signalBus.Fire(new UpdateRegionSignal(regionUnFocus.Id));
        }
    }

    private void OnHandle(RegionClickSignal regionClick)
    {
        int id = regionClick.Id;

        if (id == _currentId)
        {
            return;
        }

        var prevId = _currentId;

        var focusRegion = _cashRegions[id];

        if (!focusRegion.IsOpen &&
            (_currentId == -1 || focusRegion.IsNeighbor(_currentId)))
        {
            _regionsHandlerModel.CurrentId = id;
            focusRegion.Activate();
            _signalBus.Fire(new UpdateRegionSignal(_currentId));
        }

        if (prevId != -1)
        {
            _signalBus.Fire(new UpdateRegionSignal(prevId));
        }

        UpdateOpensRegions();
        CheckWinRules();
    }

    private void CheckWinRules()
    {
        if (_regions.Any(r => !r.IsOpen))
        {
            var curRegion = _cashRegions[_currentId];
            foreach (var neighborId in curRegion.NeighborIds)
            {
                var neighbor = _cashRegions[neighborId];
                if (!neighbor.IsOpen)
                {
                    return;
                }
            }
            _signalBus.Fire(new FailSignal());
        }
        else
        {
            _signalBus.Fire(new WinSignal());
        }
    }

    private void OnHandle(RegionsGeneratedSignal regionsGenerated)
    {
        _regions.Clear();
        _regions.AddRange(regionsGenerated.Regions);
        _cashRegions.Clear();
        regionsGenerated.Regions.ForEach(r => _cashRegions.Add(r.Id, r));

        CalcCenter();
        UpdateOpensRegions();
    }

    private void OnHandle(RestartSignal restart)
    {
        _regionsHandlerModel.CurrentId = _centerId;
        foreach (var region in _regions)
        {
            region.Restart();
            _signalBus.Fire(new UpdateRegionSignal(region.Id));
        }

        _cashRegions[_centerId].Activate();
        _signalBus.Fire(new UpdateRegionSignal(_currentId));
        UpdateOpensRegions();
    }

    private void UpdateOpensRegions()
    {
        _signalBus.Fire(new OpenRegionsSignal(_regions.Count(r => r.IsOpen), _regions.Count));
    }

    private void CalcCenter()
    {
        _regionsHandlerModel.CurrentId = -1;
        _centerId = -1;
        Vector2 mapCenter = new Vector2(_mapSize.x / 2f, _mapSize.y / 2f);

        Region mostCentral = null;
        float minDistance = float.MaxValue;

        foreach (var region in _regions)
        {
            float dist = Vector2.Distance(region.Center, mapCenter);
            if (dist < minDistance)
            {
                minDistance = dist;
                mostCentral = region;
            }
        }

        if (mostCentral != null)
        {
            _regionsHandlerModel.CurrentId = mostCentral.Id;
            _centerId = mostCentral.Id;
            mostCentral.Activate();
            _signalBus.Fire(new UpdateRegionSignal(_currentId));
        }
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<RestartSignal>(OnHandle);
        _signalBus.Unsubscribe<RegionFocusSignal>(OnHandle);
        _signalBus.Unsubscribe<RegionUnFocusSignal>(OnHandle);
        _signalBus.Unsubscribe<RegionClickSignal>(OnHandle);
        _signalBus.Unsubscribe<RegionsGeneratedSignal>(OnHandle);
    }
}