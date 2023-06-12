using System;
using System.Collections.Generic;
using Polyhydra.Core;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TessellateTest : TestBase
{
    
    [Serializable] public class ShapeSetting
    {
        public ShapeTypes type;
        public Shapes.Method method;
    
        [Range(0, 24)] public float a;
        [Range(0, 4)] public float b;
        [Range(0, 4)] public float c;
        public Vector3 translation = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        [Range(0.01f, 2)] public float uniformScale = 1;
        public Vector3 scale = Vector3.one;
    }

    public List<ShapeSetting> shapeSettings;
    public int sides;
    [Range(0, 2)] public float height;
    public float mergeThreshold;

    public override void Go()
    {
        poly = new PolyMesh();
        foreach (var setting in shapeSettings)
        {
            var shape = GetShape(setting);
            poly.Append(shape);
        }
        Build();
    }

    protected override void ModifyPostOp()
    {
        poly = poly.Tessellate(new OpParams(sides));
        if (height>0) poly = poly.Shell(new OpParams(height));
        if (mergeThreshold > 0)
        {
            int facesBefore = poly.Faces.Count;
            poly.MergeCoplanarFaces(mergeThreshold);
            Debug.Log($"Merged {facesBefore} to {poly.Faces.Count}");
        }
    }

    public PolyMesh GetShape(ShapeSetting s)
    {
        var shape = Shapes.Build(s.type, s.a, s.b, s.c, s.method);
        shape.Transform(s.translation, s.rotation, s.scale * s.uniformScale);
        return shape;
    }
}
