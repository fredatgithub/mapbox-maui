﻿
namespace Mapbox.Maui;

using System.Collections;
using CoreLocation;
using Foundation;
using MapboxCoreMaps;
using MapboxMapsObjC;

public static class AdditionalExtensions
{
    internal static NSObject Wrap(this object xvalue)
    {
        if (xvalue is IEnumerable objects)
        {
            var list = new List<NSObject>();
            foreach (var item in objects)
            {
                list.Add(item.Wrap());
            }
            return NSArray.FromNSObjects(list.ToArray());
        }

        return xvalue switch
        {
            bool value => NSNumber.FromBoolean(value),
            long value => NSNumber.FromLong((nint)value),
            double value => NSNumber.FromDouble(value),
            string value => new NSString(value),
            _ => throw new NotSupportedException($"Invalue property type: {xvalue?.GetType()} | {xvalue}")
        };
    }

    internal static NSDictionary<NSString, NSObject> ToPlatformValue(
        this Mapbox.Maui.Styles.RasterDemSource xbuilder
    )
    {
        var properties = new NSDictionary<NSString, NSObject>();
        properties["type"] = xbuilder.Type.Wrap();

        foreach (var property in xbuilder.Properties)
        {
            var xvalue = property.Value.Value.Wrap();
            properties[property.Key] = xvalue;
        }

        return properties;
    }

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

    public static MBMMapDebugOptions ToNative(this DebugOption option)
    {
        return option switch
        {
            DebugOption.TileBorders => MBMMapDebugOptions.TileBorders,
            DebugOption.ParseStatus => MBMMapDebugOptions.ParseStatus,
            DebugOption.Timestamps => MBMMapDebugOptions.Timestamps,
            DebugOption.Collision => MBMMapDebugOptions.Collision,
            DebugOption.StencilClip => MBMMapDebugOptions.StencilClip,
            DebugOption.DepthBuffer => MBMMapDebugOptions.DepthBuffer,
            _ => MBMMapDebugOptions.ModelBounds,
        };
    }

    public static IList<MBMMapDebugOptions> ToNative(this IEnumerable<DebugOption> options)
    {
        return options
            .Select(x => x.ToNative())
            .ToList();
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

        if (center == null &&
            padding == null &&
            anchor == null &&
            zoom == null &&
            pitch == null) return null;

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

