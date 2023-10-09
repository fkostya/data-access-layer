using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Data.Common;

namespace data_access_layer
{
    public class MsSqlDataSet : IEnumerable<Dictionary<string, object>>
    {
        private IList<Dictionary<string, object>> internalList;
        public Dictionary<string, DbColumn> Columns { get; private set; }

        public MsSqlDataSet()
        {
            Columns = new Dictionary<string, DbColumn>();
            internalList = new List<Dictionary<string, object>>();
        }

        public static MsSqlDataSet Empty
        {
            get
            {
                return new MsSqlDataSet();
            }
        }

        public void Add(Dictionary<string, object> row)
        {
            if(row != null)
            {
                internalList.Add(row);
            }
        }

        public void AddColumn(DbColumn column)
        {
            if(column != null)
            {
                Columns.Add(column.ColumnName, column);
            }
        }

        public IEnumerator<Dictionary<string, object>> GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}