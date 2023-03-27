﻿
namespace Mapbox.Maui;

using System;
using CoreLocation;
using Foundation;
using MapboxCoreMaps;
using MapboxMaps;
using MapboxMapsObjC;
using Microsoft.Maui.Handlers;
using PlatformView = MapboxMaps.MapView;

public partial class MapboxViewHandler
{
    private static void HandleCameraOptionsChanged(MapboxViewHandler handler, IMapboxView view)
    {
        var cameraOptions = view.CameraOptions.ToNative();
        handler.PlatformView.SetCameraTo(cameraOptions);
    }

    private static void HandleScaleBarVisibilityChanged(MapboxViewHandler handler, IMapboxView view)
    {
        handler.PlatformView.OrnamentsOptionsScaleBarVisibility(view.ScaleBarVisibility.ToNative());
    }

    private static void HandleMapboxStyleChanged(MapboxViewHandler handler, IMapboxView view)
    {
        var styleUri = view.MapboxStyle.ToNative();

        if (string.IsNullOrWhiteSpace(styleUri)) return;

        handler.PlatformView.SetStyle(styleUri);
    }

    protected override PlatformView CreatePlatformView()
    {
        var accessToken = string.IsNullOrWhiteSpace(ACCESS_TOKEN)
            ? MapInitOptionsBuilder.DefaultResourceOptions.AccessToken
            : ACCESS_TOKEN;

        MapInitOptions options = MapInitOptionsBuilder
                .Create()
                .AccessToken(accessToken)
                .Build();

        // Perform any additional setup after loading the view, typically from a nib.
        var mapView = MapViewFactory.CreateWithFrame(
            CoreGraphics.CGRect.Empty,
            options
        );

        return mapView;
    }

    protected override void ConnectHandler(PlatformView platformView)
    {
        base.ConnectHandler(platformView);

        (VirtualView as MapboxView)?.InvokeMapReady();
    }
}

public static class AdditionalExtensions
{
    public static TMBOrnamentVisibility ToNative(this OrnamentVisibility value)
    {
        return value switch
        {
            OrnamentVisibility.Adaptive => TMBOrnamentVisibility.Adaptive,
            OrnamentVisibility.Visible => TMBOrnamentVisibility.Visible,
            _ => TMBOrnamentVisibility.Hidden,
        };
    }

    public static string ToNative(this MapboxStyle mapboxStyle)
    {
        return mapboxStyle.BuiltInStyle switch
        {
            MapboxBuiltInStyle.Dark => BuiltInStyles.Dark,
            MapboxBuiltInStyle.Light => BuiltInStyles.Light,
            MapboxBuiltInStyle.Outdoors => BuiltInStyles.Outdoors,
            MapboxBuiltInStyle.MapboxStreets => BuiltInStyles.Streets,
            MapboxBuiltInStyle.Satellite => BuiltInStyles.Satellite,
            MapboxBuiltInStyle.SatelliteStreets => BuiltInStyles.SatelliteStreets,
            MapboxBuiltInStyle.TrafficDay => "mapbox://styles/mapbox/traffic-day-v2",
            MapboxBuiltInStyle.TrafficNight => "mapbox://styles/mapbox/traffic-night-v2",
            _ => mapboxStyle.Uri,
        };
    }

    public static MBMCameraOptions ToNative(this CameraOptions cameraOptions)
    {
        var center = cameraOptions.Center.HasValue
            ? new CLLocation(cameraOptions.Center.Value.X, cameraOptions.Center.Value.Y)
            : null;
        var padding = cameraOptions.Padding.HasValue
            ? new MBMEdgeInsets(
                cameraOptions.Padding.Value.Top,
                cameraOptions.Padding.Value.Left,
                cameraOptions.Padding.Value.Bottom,
                cameraOptions.Padding.Value.Right)
            : null;
        var anchor = cameraOptions.Anchor.HasValue
            ? new MBMScreenCoordinate(
                cameraOptions.Anchor.Value.X,
                cameraOptions.Anchor.Value.Y
                )
            : null;
        var zoom = cameraOptions.Zoom.HasValue
            ? NSNumber.FromFloat(cameraOptions.Zoom.Value)
            : null;
        var bearing = cameraOptions.Bearing.HasValue
            ? NSNumber.FromFloat(cameraOptions.Bearing.Value)
            : null;
        var pitch = cameraOptions.Pitch.HasValue
            ? NSNumber.FromFloat(cameraOptions.Pitch.Value)
            : null;

        return new MBMCameraOptions(
            center,
            padding,
            anchor,
            zoom,
            bearing,
            pitch
        );
    }
}

