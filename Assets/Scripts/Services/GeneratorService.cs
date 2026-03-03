using System;
using System.Collections.Generic;
using UnityEngine;
using VoronatorSharp;
using Zenject;
using Vector2 = UnityEngine.Vector2;
using Random = UnityEngine.Random;

public class GeneratorService : IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly GenerateConfigs _generateConfigs;

    private Vector2 _mapSize => _generateConfigs.MapSize;
    private int _totalSites => _generateConfigs.TotalSites;
    private int _centralDensity => _generateConfigs.CentralDensity;

    public GeneratorService(
        SignalBus signalBus,
        GenerateConfigs generateConfigs)
    {
        _signalBus = signalBus;
        _generateConfigs = generateConfigs;

        InitSignals();
    }

    private void InitSignals()
    {
        _signalBus.Subscribe<StartNewGameSignal>(NewGame);
    }

    private void NewGame()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        VoronatorSharp.Vector2[] sites = GenerateClusteredPoints(_totalSites, _centralDensity);

        var vor = new Voronator(sites, VoronatorSharp.Vector2.zero, new VoronatorSharp.Vector2(_mapSize.x, _mapSize.y));

        List<RegionDataModel> regions = new List<RegionDataModel>();

        for (int i = 0; i < sites.Length; i++)
        {
            var verts = vor.GetClippedPolygon(i);
            if (verts == null || verts.Count < 3) continue;

            var neighbors = new List<int>(vor.ClippedNeighbors(i));

            regions.Add(new RegionDataModel
            {
                Id = i,
                Vertices = verts.ToUniVectors2(),
                NeighborIds = neighbors,
                Center = new Vector2(sites[i].x, sites[i].y),
            });
        }

        _signalBus.Fire(new RegionsGeneratedSignal() 
        {
            Regions = regions,
        });
    }

    private VoronatorSharp.Vector2[] GenerateClusteredPoints(int total, int centralPercent)
    {
        List<VoronatorSharp.Vector2> points = new List<VoronatorSharp.Vector2>();
        int centralCount = (int)(total * centralPercent / 100f);

        Rect centralRect = new Rect(_mapSize.x * 0.15f, _mapSize.y * 0.15f,
                                   _mapSize.x * 0.7f, _mapSize.y * 0.7f);

        for (int i = 0; i < centralCount; i++)
        {
            points.Add(new VoronatorSharp.Vector2(
                Random.Range(centralRect.x, centralRect.xMax),
                Random.Range(centralRect.y, centralRect.yMax)
            ));
        }

        for (int i = centralCount; i < total; i++)
        {
            points.Add(new VoronatorSharp.Vector2(
                Random.Range(0f, _mapSize.x),
                Random.Range(0f, _mapSize.y)
            ));
        }

        return points.ToArray();
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<StartNewGameSignal>(NewGame);
    }
}