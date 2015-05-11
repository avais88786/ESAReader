using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectOTOMCollapse
{
    public partial class Form1 : Form
    {
        private BindingSource bindingSource1 = new BindingSource();

        public Form1()
        {
            InitializeComponent();

            dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;

        }


        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = bindingSource1;
            string query = File.ReadAllText("query.txt");
            string reference = txtBoxReference.Text;
            
            if (string.IsNullOrEmpty(reference))
                return;

            GetData(query, reference);
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            var gridColumns = dataGridView1.Columns;
            var gridRows = dataGridView1.Rows;

            gridColumns.Cast<DataGridViewColumn>().ToList().ForEach(column =>
            {
                if (column.HeaderText.Equals("Exchange Type") || column.HeaderText.Equals("Transaction Type"))
                {
                    column.DefaultCellStyle.Font = new Font("Arial", 10.5F);
                }

            });



            int currentId = 0;
            int runningRiskId = 0;
            Color color = GetRandomConsoleColor();
            foreach (DataGridViewRow row in gridRows)
            {
                runningRiskId = (int)row.Cells["RiskId"].Value;
                string transactionType = row.Cells["Transaction Type"].Value.ToString();

                //if (currentId == 0)
                //    currentId = (int)row.Cells["RiskId"].Value;

                //if (currentId == runningRiskId)
                //    row.Cells.Cast<DataGridViewCell>()
                //        .ToList().ForEach(cel => cel.Style.ForeColor = color);
                //else
                //{
                //    color = GetRandomConsoleColor();
                if (transactionType.Contains("MTA"))
                    row.Cells.Cast<DataGridViewCell>()
                        .ToList().ForEach(cel => cel.Style.ForeColor = Color.Brown);
                else
                    row.Cells.Cast<DataGridViewCell>()
                        .ToList().ForEach(cel => cel.Style.ForeColor = Color.Blue);
                
                //    currentId = runningRiskId;
                //}

                if (row.Cells["Exchange Type"].Value.ToString().Contains("Response"))
                {
                    row.Cells["Exchange Type"].Style.ForeColor = Color.Green;
                    row.Cells["Transaction Type"].Style.ForeColor = Color.Green;
                    

                }

                if (row.Cells["Exchange Type"].Value.ToString().Contains("Request"))
                {
                    row.Cells["Exchange Type"].Style.ForeColor = Color.Red;
                    row.Cells["Transaction Type"].Style.ForeColor = Color.Red;
                }

                


            }
            

        }

        private void GetData(string query,string reference)
        {
            try
            {
                query = query.Replace("REPLACEME", reference);

                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connectionString);

                // Create a command builder to generate SQL update, insert, and 
                // delete commands based on selectCommand. These are used to 
                // update the database.
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);

                // Populate a new data table and bind it to the BindingSource.
                DataTable table = new DataTable();
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table);
                bindingSource1.DataSource = table;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Occured: " + ex.Message);
            }
        }

        private void dataGridView1_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            Clipboard.SetText(dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString());
        }


        private static Random _random = new Random();
        private static Color GetRandomConsoleColor()
        {

            KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            KnownColor randomColorName = names[_random.Next(names.Length)];
            Color randomColor = Color.FromKnownColor(randomColorName);
            return randomColor;
        }

    }
}
