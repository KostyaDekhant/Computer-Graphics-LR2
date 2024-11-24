using Computer_Graphics_LR2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;


public class Polygon
{
    public Point3D[] Vertices { get; }
    public Color Color { get; }

    public Polygon(Point3D[] vertices, Color color)
    {
        Vertices = vertices;
        Color = color;
    }

    public (float a, float b, float c, float d) GetPlaneEquation()
    {
        int size = Vertices.Length;
        float a = 0, b = 0, c = 0, d = 0;

        //Метод Ньюэла
        for (int i = 0; i < size; i++)
        {
            int j = (i == size - 1) ? 0 : i + 1;
            a += (Vertices[i].Y - Vertices[j].Y) * (Vertices[i].Z + Vertices[j].Z);
            b += (Vertices[i].Z - Vertices[j].Z) * (Vertices[i].X + Vertices[j].X);
            c += (Vertices[i].X - Vertices[j].X) * (Vertices[i].Y + Vertices[j].Y);
        }

        //Используем последнюю вершину для вычисления d
        d = -(a * Vertices[size - 1].X + b * Vertices[size - 1].Y + c * Vertices[size - 1].Z);

        return (a, b, c, d);
    }

    public float GetZ(float x, float y)
    {
        var (a, b, c, d) = GetPlaneEquation();
        if (c == 0) return 0;
        return -(a * x + b * y + d) / c;
    }
}


