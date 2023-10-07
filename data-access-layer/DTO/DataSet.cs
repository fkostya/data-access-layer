namespace data_access_layer
{
    public class DataSet
    {
        public static DataSet Empty()
        {
            return new DataSet();
        }

        public IEnumerable<Dictionary<string, object>> Result { get; internal set; }

        public Dictionary<string, object> FieldType;
    }
}