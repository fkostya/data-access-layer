namespace data_access_layer.Model
{
    public class DbStats
    {
        public string Database { get; set; } = "";
        public DateTime ExecutionStart { get; set; } = DateTime.Now;
        public DateTime ExecutionEnd { get; set; }
    }
}