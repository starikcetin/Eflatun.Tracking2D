namespace starikcetin.Eflatun.Expansions
{
    /// <summary>
    /// The value type of the internal dictionary of a <see cref="DataTable{TRow,TCol,TCell}"/> class.
    /// </summary>
    public struct DataTableEntry<TRow, TCol, TCell>
    {
        private readonly TRow _row;
        private readonly TCol _col;
        private readonly TCell _cell;

        public TRow Row
        {
            get { return _row; }
        }

        public TCol Col
        {
            get { return _col; }
        }

        public TCell Cell
        {
            get { return _cell; }
        }

        public DataTableEntry(TRow row, TCol col, TCell cell)
        {
            _row = row;
            _col = col;
            _cell = cell;
        }
    }
}
