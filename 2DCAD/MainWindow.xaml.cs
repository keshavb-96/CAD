using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace _2DCAD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            mContentArea.Child = mEditor = new DwgEditor(new Drawing());

            mFileExit.Click += (s, e) => Close();
            foreach (MenuItem item in mModes.Items.OfType<MenuItem>())
                item.Click += (s, e) => SetMode(item);
        }

        void SetMode(MenuItem mi)
        {
            foreach (var item in mModes.Items.OfType<MenuItem>())
                item.IsChecked = false;

            EDwgMode mode;
            if (Enum.TryParse<EDwgMode>(mi.Tag.ToString(), true, out mode))
            {
                mEditor.Mode = mode;
                mi.IsChecked = true;
            }
        }

        DwgEditor mEditor;
    }
}
