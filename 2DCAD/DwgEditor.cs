using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace _2DCAD
{
    class DwgEditor : UserControl
    {
        public DwgEditor(Drawing dwg)
        {
            Dwg = dwg;
            Mode = EDwgMode.Line;
            ClipToBounds = true;
            Focusable = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                for (int i = Dwg.Shapes.Count - 1; i >= 0; i--)
                    if (Dwg.Shapes[i].IsSelected)
                        Dwg.Shapes.RemoveAt(i);
                this.InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            Focus();
            dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, ActualWidth, ActualHeight));

            foreach (var shape in Dwg.Shapes)
                shape.Draw(dc);
            mCurrentWidget.Render(dc);
        }

        Drawing Dwg;

        public EDwgMode Mode
        {
            get { return mMode; }
            set
            {
                mMode = value;
                if (mCurrentWidget != null) mCurrentWidget.Detach();
                switch (mMode)
                {
                    case EDwgMode.Line: mCurrentWidget = new LineMaker(Dwg, this); break;
                    case EDwgMode.Arc: mCurrentWidget = new ArcMaker(Dwg, this); break;
                    case EDwgMode.Circle: mCurrentWidget = new CircleMaker(Dwg, this); break;
                    case EDwgMode.Circle2P: mCurrentWidget = new Circle2PMaker(Dwg, this); break;
                    case EDwgMode.Pick: mCurrentWidget = new Picker(Dwg, this); break;
                }
            }
        }
        EDwgMode mMode;
        Widget mCurrentWidget;
    }

    enum EDwgMode
    {
        Line,
        Arc,
        Circle,
        Circle2P,
        Pick,
    }
}
