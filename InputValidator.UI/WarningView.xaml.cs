using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InputValidator.UI
{
    /// <summary>
    /// Interaction logic for WarningView.xaml
    /// </summary>
    public partial class WarningView
    {
        public WarningView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.ViewModel)
                .Where(vm => vm != null)
                .Do(vm => PopulateFromViewModel(vm, disposables))
                .Subscribe()
                .DisposeWith(disposables);
            });
        }

        private void PopulateFromViewModel(Warning vm, CompositeDisposable disposables)
        {
            this.RunScope.Text = vm.Extent.ToString();
        }
    }
}