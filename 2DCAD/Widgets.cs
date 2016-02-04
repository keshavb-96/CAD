using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace _2DCAD
{
    class Widget
    {
        public Widget(Drawing dwg, DwgEditor editor)
        {
            Dwg = dwg; Editor = editor;
            editor.MouseLeftButtonDown += MouseDown;
            editor.MouseMove += MouseMove;
        }

        virtual public void MouseMove(object sender, MouseEventArgs e) { }
        virtual public void MouseDown(object sender, MouseButtonEventArgs e) { }
        virtual public void Render(DrawingContext dc) { }

        virtual public void Detach()
        {
            Editor.MouseLeftButtonDown -= MouseDown;
            Editor.MouseMove -= MouseMove;
        }

        protected List<Point> Pts = new List<Point>();
        protected Drawing Dwg;
        protected DwgEditor Editor;
    }


    class ArcMaker : Widget
    {
        public ArcMaker(Drawing dwg, DwgEditor editor)
           : base(dwg, editor)
        {
        }

        public override void MouseDown(object sender, MouseButtonEventArgs e)
        {
            Pts.Add(e.GetPosition(Editor));
            if (Pts.Count == 3)
            {
                Dwg.Shapes.Add(new Arc { A = Pts[0], B = Pts[1], C = Pts[2] });
                Editor.InvalidateVisual();
                Pts.Clear();
            }
        }

        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (Pts.Count == 1 || Pts.Count == 2)
            {
                if (mAdded) Dwg.Shapes.RemoveAt(Dwg.Shapes.Count - 1);
                if (Pts.Count == 1)
                    Dwg.Shapes.Add(new Line { A = Pts[0], B = e.GetPosition(Editor) });
                else
                    Dwg.Shapes.Add(new Arc { A = Pts[0], B = Pts[1], C = e.GetPosition(Editor) });
                Editor.InvalidateVisual();
                mAdded = true;
            }
        }

        bool mAdded;
    }

    class LineMaker : Widget
    {
        public LineMaker(Drawing dwg, DwgEditor editor)
           : base(dwg, editor)
        {
        }

        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (Pts.Count == 1)
            {
                if (mLineAdded) Dwg.Shapes.RemoveAt(Dwg.Shapes.Count - 1);
                Dwg.Shapes.Add(new Line { A = Pts[0], B = e.GetPosition(Editor) });
                Editor.InvalidateVisual();
                mLineAdded = true;
            }
        }

        public override void MouseDown(object sender, MouseButtonEventArgs e)
        {
            Pts.Add(e.GetPosition(Editor));
            if (Pts.Count == 2)
            {
                Dwg.Shapes.Add(new Line { A = Pts[0], B = Pts[1] });
                Editor.InvalidateVisual();
                Pts.Clear();
            }
        }

        bool mLineAdded;
    }


    class Picker : Widget
    {
        public Picker(Drawing dwg, DwgEditor editor) : base(dwg, editor)
        {
            Editor.MouseLeftButtonUp += MouseUp;
        }

        public override void Detach()
        {
            Editor.MouseLeftButtonUp -= MouseUp;
            base.Detach();
        }

        public override void Render(DrawingContext dc)
        {
            if (mMouseIsDown)
            {
                Rect r = new Rect(Pts[0], mptMove);
                Pen pen = new Pen(Brushes.Blue, 2) { DashStyle = DashStyles.Dot };
                dc.DrawRectangle(null, pen, r);
            }
        }

        void MouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            Rect bound = new Rect(Pts[0], mptMove);

            mMouseIsDown = false;
            bool iShift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (!iShift)
                foreach (var s in Dwg.Shapes)
                    s.IsSelected = false;
            foreach (var s in Dwg.Shapes)
                if (s.Contained(bound)) s.IsSelected = true;

            // WRONG: 
            //foreach (var s in Dwg.Shapes)
            //   s.IsSelected = s.Contained (bound);

            Pts.Clear();
            Editor.InvalidateVisual();
        }

        public override void MouseDown(object sender, MouseButtonEventArgs e)
        {
            Pts.Add(e.GetPosition(Editor));
            mMouseIsDown = true;
            Mouse.Capture(Editor, CaptureMode.Element);
        }

        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (mMouseIsDown)
            {
                mptMove = e.GetPosition(Editor);
                Editor.InvalidateVisual();
            }
        }

        Point mptMove;
        bool mMouseIsDown;
    }


    class CircleMaker : Widget
    {
        public CircleMaker(Drawing dwg, DwgEditor editor)
           : base(dwg, editor)
        {
        }

        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (Pts.Count == 1)
            {
                if (mCircleAdded) Dwg.Shapes.RemoveAt(Dwg.Shapes.Count - 1);
                Dwg.Shapes.Add(new Circle { Center = Pts[0], Radius = Pts[0].DistTo(e.GetPosition(Editor)) });
                Editor.InvalidateVisual();
                mCircleAdded = true;
            }
        }

        public override void MouseDown(object sender, MouseButtonEventArgs e)
        {
            Pts.Add(e.GetPosition(Editor));
            if (Pts.Count == 2)
            {
                Dwg.Shapes.Add(new Circle { Center = Pts[0], Radius = Pts[1].DistTo(Pts[0]) });
                Editor.InvalidateVisual();
                Pts.Clear();
            }
        }

        bool mCircleAdded;
    }

    class Circle2PMaker : Widget
    {
        public Circle2PMaker(Drawing dwg, DwgEditor editor)
           : base(dwg, editor)
        {
        }

        //Center -- calculated w.r.t Pts[0] and Pts[1] or e.GetPosition (Editor) using PtBet2Pts method.
        // center is moved to Pts[0] for the point returned from PtBet2Pts Method.
        // Pts[0] and Pts[1] will give diameter of circle. So to convert to radius is done.
        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (Pts.Count == 1)
            {
                if (mCircleAdded) Dwg.Shapes.RemoveAt(Dwg.Shapes.Count - 1);
                Dwg.Shapes.Add(new Circle { Center = (Pts[0].Mid(e.GetPosition(Editor))), Radius = 0.5 * (Pts[0].DistTo(e.GetPosition(Editor))) });
                Editor.InvalidateVisual();
                mCircleAdded = true;
            }
        }

        public override void MouseDown(object sender, MouseButtonEventArgs e)
        {
            Pts.Add(e.GetPosition(Editor));
            if (Pts.Count == 2)
            {
                Dwg.Shapes.Add(new Circle { Center = Pts[0].Mid(Pts[1]), Radius = 0.5 * (Pts[1].DistTo(Pts[0])) });
                Editor.InvalidateVisual();
                Pts.Clear();
            }
        }

        bool mCircleAdded;
    }
}
