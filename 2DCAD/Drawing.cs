using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;

namespace _2DCAD
{
    class Shape
    {
        public Color Color = Colors.Black;
        public bool IsSelected = false;

        protected Pen MakePen()
        {
            Pen pen;
            if (IsSelected) pen = new Pen(Brushes.Blue, 1);
            else {
                Brush br = new SolidColorBrush(Color);
                br.Freeze();
                pen = new Pen(br, 1);
            }
            pen.Freeze();
            return pen;
        }

        public virtual void Draw(DrawingContext dc) { }
        public virtual bool Contained(Rect r) { return false; }
    }

    class Line : Shape
    {
        public Point A, B;

        public override void Draw(DrawingContext dc)
        {
            dc.DrawLine(MakePen(), A, B);
        }

        public override bool Contained(Rect r)
        {
            return r.Contains(A) && r.Contains(B);
        }
    }

    class Arc : Shape
    {
        public Point A, B, C;

        public override void Draw(DrawingContext dc)
        {
            Point p1 = A.Mid(B), p2 = B.Mid(C);

            //dc.DrawLine (pen, A, B);
            //dc.DrawLine (pen, B, C);
            double h1 = A.Heading(B), h2 = B.Heading(C);
            double hp1 = h1 + Math.PI / 2, hp2 = B.Heading(C) + Math.PI / 2;

            //dc.DrawEllipse (Brushes.Blue, null, p1, 2, 2);
            //dc.DrawEllipse (Brushes.Blue, null, p2, 2, 2);

            Point p3 = p1.PolarMove(500, hp1);
            Point p4 = p2.PolarMove(500, hp2);
            //dc.DrawLine (pen, p1, p3);
            //dc.DrawLine (pen, p2, p4);

            Point cen = Extensions.Intersect(p1, p3, p2, p4);
            //dc.DrawEllipse (Brushes.Green, null, cen, 5, 5);
            double rad = cen.DistTo(A);

            double turn = h2 - h1;
            if (turn < -Math.PI) turn += 2 * Math.PI;
            else if (turn > Math.PI) turn -= 2 * Math.PI;
            SweepDirection sweep = (turn < 0) ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
            bool largeArc = Math.Abs(turn) > Math.PI / 2;

            StreamGeometry sg = new StreamGeometry();
            using (StreamGeometryContext ctx = sg.Open())
            {
                ctx.BeginFigure(A, false, false);
                ctx.ArcTo(C, new Size(rad, rad), 0, largeArc, sweep, true, false);
            }
            sg.Freeze();
            dc.DrawGeometry(null, MakePen(), sg);
        }

        public override bool Contained(Rect r)
        {
            return r.Contains(A) && r.Contains(B) && r.Contains(C);
        }
    }

    class Circle : Shape
    {
        public Point Center;
        public double Radius;

        public override void Draw(DrawingContext dc)
        {
            dc.DrawEllipse(null, MakePen(), Center, Radius, Radius);
        }

        public override bool Contained(Rect r)
        {
            Rect bound = new Rect(new Point(Center.X - Radius, Center.Y - Radius), new Point(Center.X + Radius, Center.Y + Radius));
            return r.Contains(bound);
        }
    }

    class Drawing
    {
        public List<Shape> Shapes = new List<Shape>();
    }
}
