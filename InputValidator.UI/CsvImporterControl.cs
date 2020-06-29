using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace InputValidator.UI
{
    public abstract class CsvImporterControl<T> : ReactiveUserControl<CsvImporterViewModel<T>>
    {
    }
}