using data_access_layer;

namespace appUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //
            var sql = new MsSqlDataAccessLayer("Server=(localdb)\\MSSQLLocalDB;Database=test;Trusted_Connection=True;");


            var t = await sql.SelectDataAsDataSet($@"
                            select * from master
                            select id as idAsSecondResult from master where id between 2 and 5");
        }
    }
}
