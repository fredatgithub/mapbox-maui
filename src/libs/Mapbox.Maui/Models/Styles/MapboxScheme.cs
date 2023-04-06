﻿namespace Mapbox.Maui;

public struct MapboxScheme
{
    public string Value { get; }

    private MapboxScheme(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(MapboxScheme schem) => schem.Value;

    /// <summary>
    /// Slippy map tilenames scheme.
    /// </summary>
    public static MapboxScheme Xyz = new MapboxScheme("xyz");

    /// <summary>
    /// OSGeo spec scheme.
    /// </summary>
    public static MapboxScheme Tms = new MapboxScheme("tms");
}

