namespace CentralStation.Client.Shared.DataGrid
{
    public class DataColumnDefinition<TRow>
    {
        public DataColumnDefinition(string columnName, Func<TRow, object?> select)
        {
            ColumnName = columnName;
            Select = select;
        }

        public string ColumnName { get; set; } = String.Empty;
        public Func<TRow, object?> Select { get; set; } = _ => null;
    }
}
