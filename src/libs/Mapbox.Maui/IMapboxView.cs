﻿namespace MapboxMaui;

using System.Windows.Input;
using MapboxMaui.Annotations;
using MapboxMaui.Styles;

public partial interface IMapboxView : IView
{
    CameraOptions CameraOptions { get; set; }
    MapboxStyle MapboxStyle { get; set; }
    Point? MapCenter { get; set; }
    float? MapZoom { get; set; }

    OrnamentVisibility ScaleBarVisibility { get; set; }

    DebugOption[] DebugOptions { get; set; }

    IEnumerable<MapboxSource> Sources { get; set; }

    Terrain Terrain { get; set; }

    Light Light { get; set; }

    IEnumerable<MapboxLayer> Layers { get; set; }

    IAnnotationController AnnotationController { get; }
}

partial interface IMapboxView { 
    event EventHandler MapReady;
    ICommand MapReadyCommand { get; set; }

    event EventHandler StyleLoaded;
    ICommand StyleLoadedCommand { get; set; }

    event EventHandler MapLoaded;
    ICommand MapLoadedCommand { get; set; }
}

public interface IAnnotationController
{
    public IPolygonAnnotationManager CreatePolygonAnnotationManager(string id, LayerPosition layerPosition);
    public ICircleAnnotationManager CreateCircleAnnotationManager(string id, LayerPosition layerPosition);
    //public IPointAnnotationManager CreatePointAnnotationManager(string id, LayerPosition layerPosition);
    //public IPolylineAnnotationManager CreatePolylineAnnotationManager(string, LayerPosition layerPosition);
}
