﻿using Computer_Graphics_LR2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
public class Point3D
{
    public float X;
    public float Y;
    public float Z;

    public Point3D(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}

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

public class Renderer
{
    private int width;
    private int height;
    private Color[] frameBuffer;
    private int[] zBuffer;

    public Renderer(int width, int height)
    {
        this.width = width;
        this.height = height;
        frameBuffer = new Color[width];
        zBuffer = new int[width];

        //Инициализация Z-буфера значением -беск.
        for (int i = 0; i < width; i++)
        {
            zBuffer[i] = int.MinValue;
        }
    }
    public void Render(List<Polygon> polygons, Graphics g, Rectangle frameRect, Rectangle zRect)
    {
        GetMaxZ(polygons);
        for (int y = 0; y < height; y++)
        {
            //Сброс буферов для текущей строки
            for (int x = 0; x < width; x++)
            {
                frameBuffer[x] = Color.White; //Фоновый цвет
                zBuffer[x] = int.MinValue; //Минимальное значение (-беск.)
            }

            for (int x = 0; x < width; x++)
            {
                foreach (var polygon in polygons)
                {
                    if (isPointInsidePolygon(polygon, new Point(x, y)))
                    {
                        int z = (int)polygon.GetZ(x, y);

                        if (z > zBuffer[x])
                        {
                            zBuffer[x] = z; //Обновление значения Z в z-буфере
                            frameBuffer[x] = polygon.Color; //Обновление цвета в буфере кадра
                        }
                    }
                }
            }
            Draw(g, frameRect, zRect, y);
        }
       
    }
    public bool isPointInsidePolygon(Polygon polygon, Point p)
    {
        int n = polygon.Vertices.Length;
        bool inside = false;
        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            Point3D pi = polygon.Vertices[i];
            Point3D pj = polygon.Vertices[j];

            //Проверяет, пересекает ли луч от точки p сторону многоугольника
            if ((pi.Y > p.Y) != (pj.Y > p.Y) &&
                    (p.X < (pj.X - pi.X) * (p.Y - pi.Y) / (pj.Y - pi.Y) + pi.X))
            {
                inside = !inside;
            }
        }
        return inside;
    }

    private float maxZ = 0;

    private void GetMaxZ(List<Polygon> polygons)
    {
        foreach (var polygon in polygons)
        {
            foreach(var pol in polygon.Vertices)
            {
                if (pol.Z > maxZ)
                    maxZ = pol.Z;
            }
        }
    }
    public void Draw(Graphics g, Rectangle frameRect, Rectangle zRect, int y)
    {
        //Отрисовка буфера кадра
        for (int x = 0; x < width; x++)
        {
            using (Brush brush = new SolidBrush(frameBuffer[x]))
            {
                g.FillRectangle(brush, frameRect.Left + x, frameRect.Top + y, 1, 1);
            }
        }
       
        //Отрисовка z-буфера (чб) с учетом расстояния
        for (int x = 0; x < width; x++)
        {
            float normalizedZ = (maxZ - zBuffer[x]) / maxZ; //Нормализация
            int grayValue = (int)(255 * (1 - normalizedZ)); //Чем ближе, тем светлее
            grayValue = Math.Clamp(grayValue, 0, 255); //Ограничение значений от 0 до 255

            using (Brush brush = new SolidBrush(Color.FromArgb(grayValue, grayValue, grayValue)))
            {
                g.FillRectangle(brush, zRect.Left + x, zRect.Top + y, 1, 1); // Рисуем только одну строку
            }
        }
    }
}
