using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace InputValidator.Samples.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            importer.ViewModel = new UI.CsvImporterViewModel<Output>();
            importer.ViewModel.Parser.Converter = elements => new Output
            {
                Id = int.Parse(elements[0]),
                Description = elements[1],
                Factor = double.Parse(elements[2], System.Globalization.NumberStyles.Any, new CultureInfo(""))
            };
            importer.ViewModel.Parser.ParserRuleSet.AddRange(new[] { CsvHelper.IsValidInt, CsvHelper.NoRule, CsvHelper.IsValidDoubleInvariant });
        }
    }
}