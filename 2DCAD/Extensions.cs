using System;
using System.Windows;

namespace _2DCAD
{
    static class Extensions
    {
        static public double DistTo(this Point a, Point b)
        {
            double dx = a.X - b.X, dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        static public double Heading(this Point a, Point b)
        {
            return Math.Atan2(b.Y - a.Y, b.X - a.X);
        }

        static public Point PolarMove(this Point a, double r, double theta)
        {
            return new Point(a.X + r * Math.Cos(theta), a.Y + r * Math.Sin(theta));
        }

        static public Point Mid(this Point a, Point b)
        {
            return new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        }

        static public Point Intersect(Point a, Point b, Point c, Point d)
        {
            double a1 = a.Y - b.Y, b1 = b.X - a.X;
            double c1 = a.X * (b.Y - a.Y) + a.Y * (a.X - b.X);
            double a2 = c.Y - d.Y, b2 = d.X - c.X;
            double c2 = c.X * (d.Y - c.Y) + c.Y * (c.X - d.X);
            double denom = a1 * b2 - a2 * b1;
            return new Point((b1 * c2 - b2 * c1) / denom, (c1 * a2 - c2 * a1) / denom);
        }
    }
}
    
