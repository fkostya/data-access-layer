using System.Collections;
using System.Data.Common;

namespace data_access_layer
{
    public class DataSet : IEnumerable
    {
        public static DataSet Empty()
        {
            return new DataSet();
        }

        public void Add(Dictionary<string, object> row)
        {
            if(row != null)
            {
                rows.Add(row);
            }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IList<Dictionary<string, object>> rows { get; set; }

        public Dictionary<string, DbColumn> Columns { get; set; }

        public DataSet()
        {
            Columns = new Dictionary<string, DbColumn>();
            rows = new List<Dictionary<string, object>>();
        }
    }
}