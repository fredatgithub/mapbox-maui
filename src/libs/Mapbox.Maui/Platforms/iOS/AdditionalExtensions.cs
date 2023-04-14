﻿
namespace Mapbox.Maui;

using System.Collections;
using CoreLocation;
using Foundation;
using Mapbox.Maui.Expressions;
using Mapbox.Maui.Styles;
using MapboxCoreMaps;
using MapboxMapsObjC;

public static class AdditionalExtensions
{
    internal static TMBTerrain ToPlatformValue(
        this Terrain xvalue
    )
    {
        var result = new TMBTerrain(xvalue.SourceId);

        switch (xvalue.Exaggeration.Value)
        {
            case DslExpression expression:
                result.Exaggeration = TMBValue.Expression(expression.ToPlatformValue());
                break;
            case double doubleValue:
                result.Exaggeration = TMBValue.Constant(doubleValue.Wrap());
                break;
        }

        return result;
    }

    internal static TMBLayerPosition ToPlatformValue(
        this LayerPosition xvalue
    )
    {
        return xvalue.Enum switch
        {
            LayerPositionEnum.Above => TMBLayerPosition.Above,
            LayerPositionEnum.At => TMBLayerPosition.At,
            LayerPositionEnum.Below => TMBLayerPosition.Below,
            _ => TMBLayerPosition.Unowned,
        };
    }

    internal static TMBStyleTransition ToPlatformValue(this StyleTransition xvalue)
    {
        return new TMBStyleTransition(xvalue.Duration, xvalue.Delay);
    }

    internal static NSObject Wrap(this object xvalue)
    {
        var result = xvalue switch
        {
            bool value => NSNumber.FromBoolean(value),
            int value => NSNumber.FromLong((nint)value),
            long value => NSNumber.FromLong((nint)value),
            float value => NSNumber.FromDouble(value),
            double value => NSNumber.FromDouble(value),
            string value => new NSString(value),
            Color value => new NSString(value.ToRgbaString()),
            IStringEnum value => new NSString(value.Value),
            IPropertyValue value => value.Value is DslExpression expression1
                    ? NSArray.FromNSObjects(expression1
                        .ToObjects()
                        .Select(x => x.Wrap())
                        .ToArray()
                    )
                    : value.Value.Wrap(),
            _ => (NSObject)null
        };

        if (result != null) return result;

        if (xvalue is DslExpression expression)
        {
            return NSArray.FromNSObjects(
                expression
                    .ToObjects()
                    .Select(x => x.Wrap())
                    .ToArray()
                );
        }

        if (xvalue is IDictionary<string, object> dict)
        {
            var list = new NSMutableDictionary<NSString, NSObject>();
            foreach (var item in dict)
            {
                list[item.Key] = item.Value.Wrap();
            }
            return new NSDictionary<NSString, NSObject>(list.Keys, list.Values);
        }

        if (xvalue is IEnumerable objects)
        {
            var list = new List<NSObject>();
            foreach (var item in objects)
            {
                list.Add(item.Wrap());
            }
            return NSArray.FromNSObjects(list.ToArray());
        }

        throw new NotSupportedException($"Invalue property type: {xvalue?.GetType()} | {xvalue}");
    }

    internal static NSDictionary<NSString, NSObject> ToPlatformValue(
        this MapboxLayer xvalue
    )
    {
        var properties = new NSMutableDictionary<NSString, NSObject>();
        foreach (var property in xvalue.Properties)
        {
            var propertyValue = property.Value.Wrap();
            properties[property.Key] = propertyValue;
        }

        return new NSDictionary<NSString, NSObject>(
            properties.Keys,
            properties.Values
        );
    }

    internal static NSDictionary<NSString, NSObject> ToPlatformValue(
        this MapboxSource source
    )
    {
        // TODO Add volatile properties
        var properties = new NSMutableDictionary<NSString, NSObject>();
        foreach (var property in source.Properties)
        {
            var xvalue = property.Value.Wrap();
            properties[property.Key] = xvalue;
        }

        return new NSDictionary<NSString, NSObject>(
            properties.Keys,
            properties.Values
        );
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
        return mapboxStyle.Value;
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

