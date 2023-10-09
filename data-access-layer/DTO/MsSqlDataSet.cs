using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Data.Common;

namespace data_access_layer
{
    public class MsSqlDataSet
    {
        private IList<Dictionary<string, object>> internalList;
        public Dictionary<string, DbColumn> Columns { get; private set; }
        public string DataSetName {  get; private set; }

        public MsSqlDataSet()
        {
            Columns = new Dictionary<string, DbColumn>();
            internalList = new List<Dictionary<string, object>>();
            DataSetName = Guid.NewGuid().ToString();
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
    }
}