namespace starikcetin.Eflatun.Expansions
{
    /// <summary>
    /// A special <see cref="DataTable{TRow,TCol,TCell}"/> type where you get the same cell when you swap the row and column. <para/>
    /// For example: When you assign value A to [1-2], you will get value A for both [1-2] and [2-1]. ([1-2] means row 1 and column 2) <para/>
    /// To be able to swap row and column, they have to be the same type. So this class takes a single type for both rows and columns. <para/>
    /// This class does not handle hash collisions at all! Use it with caution.
    /// </summary>
    ///
    /// <remarks>
    /// Implementing this class is really easy. While getting the key before pairing function, we sort the hash values, so it doesn't matter which hash is which object's.
    /// For example, say we have object A and object B. Let hash code of A be 1 and B be 2.
    ///     - CalculateKey (A, B) --> Pair (Max (A,B), NotMax (A,B)) = Pair (2,1).
    ///     - CalculateKey (B, A) --> Pair (Max (B,A), NotMax (B,A)) = Pair (2,1).
    /// As you see, the pair remains same no matter the order of objects.
    /// </remarks>
    public class SymmetricDataTable<TRowAndCol, TCell> : DataTable<TRowAndCol, TRowAndCol, TCell>
    {
        #region Overrides of DataTable<TRow,TCol,TCell>

        protected override long CalculateKey(TRowAndCol row, TRowAndCol col)
        {
            int rowHash = row.GetHashCode();
            int colHash = col.GetHashCode();

            // This is a sorting operation. If colHash is bigger, we swap them. So first parameter is always the bigger one.
            return rowHash >= colHash ? Pair(rowHash, colHash) : Pair(colHash, rowHash);
        }

        #endregion
    }
}
