using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Challenge_PixelWaves.View
{
    /// <summary>
    /// Logique d'interaction pour PixelGeneratorView.xaml
    /// </summary>
    public partial class PixelGeneratorView : UserControl
    {
        public PixelGeneratorView()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("PixelGeneratorView instanciť");
        }
    }
}

