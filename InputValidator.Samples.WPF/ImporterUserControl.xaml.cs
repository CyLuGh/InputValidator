using InputValidator.UI;
using Microsoft.Win32;
using ReactiveUI;
using Splat;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace InputValidator.Samples.WPF
{
    public partial class ImporterUserControl : IEnableLogger
    {
        public ImporterUserControl()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.ViewModel)
                    .Where(x => x != null)
                    .Do(x =>
                    {
                        PopulateFromViewModel(x, disposables);
                        DataGrid.BuildColumns(ViewModel.GetType().GetGenericArguments()[0]);
                    })
                    .Subscribe()
                    .DisposeWith(disposables);
            });
        }

        private void PopulateFromViewModel(CsvImporterViewModel<Output> viewModel, CompositeDisposable disposables)
        {
            this.OneWayBind(viewModel,
                vm => vm.FilePath,
                v => v.TextBoxFilePath.Text)
                .DisposeWith(disposables);

            this.OneWayBind(viewModel,
                vm => vm.HasWarnings,
                v => v.WarningsDisplay.Visibility,
                b => b ? Visibility.Visible : Visibility.Collapsed)
                .DisposeWith(disposables);

            this.OneWayBind(viewModel,
                vm => vm.Warnings,
                v => v.WarningsList.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(viewModel,
                vm => vm.Parsed,
                v => v.DataGrid.ItemsSource)
                .DisposeWith(disposables);

            this.Bind(viewModel,
                vm => vm.Parser.SkipFirstLine,
                v => v.SkipFirstCheck.IsChecked)
                .DisposeWith(disposables);

            this.Bind(viewModel,
                vm => vm.Parser.SkipLastLine,
                v => v.SkipLastCheck.IsChecked)
                .DisposeWith(disposables);

            this.Bind(viewModel,
                vm => vm.Parser.HasTrailingSeparator,
                v => v.TrailingCheck.IsChecked)
                .DisposeWith(disposables);

            this.BindCommand(viewModel,
                vm => vm.PickFileCommand,
                v => v.PickFileButton)
                .DisposeWith(disposables);

            this.BindCommand(viewModel,
                vm => vm.ParseFileCommand,
                v => v.ParseButton)
                .DisposeWith(disposables);

            viewModel.PickFileInteraction.RegisterHandler(ctx =>
            {
                var dlg = new OpenFileDialog
                {
                    DefaultExt = ".csv",
                    Filter = "CSV files (.csv)|*.csv|Text documents (.txt)|*.txt"
                };

                var res = dlg.ShowDialog();
                if (res == true)
                    ctx.SetOutput(dlg.FileName);
                else
                    ctx.SetOutput(string.Empty);
            })
            .DisposeWith(disposables);
        }
    }
}