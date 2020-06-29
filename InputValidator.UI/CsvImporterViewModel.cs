using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace InputValidator.UI
{
    public class CsvImporterViewModel<T> : ReactiveObject, IActivatableViewModel, IEnableLogger
    {
        [Reactive] public string FilePath { get; set; }
        [Reactive] public ReadOnlyCollection<T> Parsed { get; set; }
        [Reactive] public ReadOnlyCollection<Warning> Warnings { get; set; }

        public bool HasWarnings { [ObservableAsProperty] get; }

        public CsvInput<T> Parser { get; }

        public ReactiveCommand<Unit, Results<T>> ParseFileCommand { get; private set; }

        public ReactiveCommand<Unit, string> PickFileCommand { get; private set; }

        public Interaction<Unit, string> PickFileInteraction { get; }
            = new Interaction<Unit, string>(RxApp.MainThreadScheduler);

        public ViewModelActivator Activator { get; }

        public CsvImporterViewModel()
        {
            Activator = new ViewModelActivator();
            Parser = new CsvInput<T>();

            PickFileInteraction.RegisterHandler(ctx => ctx.SetOutput(string.Empty));
            InitializeCommands(this);

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.Warnings)
                    .Select(o => o?.Count > 0)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToPropertyEx(this, x => x.HasWarnings)
                    .DisposeWith(disposables);
            });
        }

        private static void InitializeCommands(CsvImporterViewModel<T> csvImporterViewModel)
        {
            csvImporterViewModel.ParseFileCommand = ReactiveCommand.CreateFromObservable(() =>
                Observable.Start(() => csvImporterViewModel.Parser.ParseFile(csvImporterViewModel.FilePath)),
                csvImporterViewModel.WhenAnyValue(x => x.FilePath)
                    .Select(path => !string.IsNullOrWhiteSpace(path) && File.Exists(path)));
            csvImporterViewModel.ParseFileCommand
                .Where(r => r != null)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(r =>
                {
                    csvImporterViewModel.Parsed = r.Parsed;
                    csvImporterViewModel.Warnings = r.Warnings;
                });

            csvImporterViewModel.PickFileCommand = ReactiveCommand.CreateFromObservable(() =>
                csvImporterViewModel.PickFileInteraction.Handle(Unit.Default));
            csvImporterViewModel.PickFileCommand
                .Subscribe(path => csvImporterViewModel.FilePath = path);
        }
    }
}