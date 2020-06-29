using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace InputValidator.Samples.WPF
{
    public static class DataGridHelper
    {
        public static void BuildColumns(this DataGrid dataGrid, Type type)
            => BuildColumns(dataGrid, type.GetProperties().Select(p => p.Name).ToArray());

        public static void BuildColumns(this DataGrid dataGrid, string[] properties)
        {
            dataGrid.AutoGenerateColumns = false;
            dataGrid.Columns.Clear();

            foreach (var property in properties)
            {
                var column = new DataGridTextColumn
                {
                    Header = property,
                    IsReadOnly = true,
                    Binding = new Binding(property),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                };

                dataGrid.Columns.Add(column);
            }
        }
    }
}