using System.Collections.Generic;
using Godot;

namespace PixelTerrarium.Model
{
    public class Material
    {
        public enum MaterialType
        {
            Dust,
            Liquid,
            Solid,
            Empty
        }
        public List<MaterialType> Types;
        public List<Color> PossibleColors;
        public Vector2 ColorInPaletteRange;

        public Material(List<MaterialType> types, List<Color> possibleColors)
        {
            Types = types;
            PossibleColors = possibleColors;
        }
    }
}