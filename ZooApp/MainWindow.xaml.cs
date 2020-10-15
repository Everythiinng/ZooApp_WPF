using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace ZooApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //We must define the sqlConnection part so it creates a new connection when the program starts
        SqlConnection sqlConnection;

        public MainWindow()
        {
            InitializeComponent();
            //We use ConnectionString to Define where to connect to, <using System.Configuration|| We add this in References.
            //Then we write this down and in "" We write the Project Name.Properties.Settings.<In here we write the name of the database> and so forth.
            string connectionString = ConfigurationManager.ConnectionStrings["ZooApp.Properties.Settings._UdemySQLConnectionString"].ConnectionString;

            //and we initialize it and add the connection string so everytime the connection starts and gets inicialized it knows where to connect to.
            sqlConnection = new SqlConnection(connectionString);
            ShowZoos();
        }

        private void ShowZoos()
        {
            //Always use Try&Catch functionality for sql connections so if any error should occur it doesnt crash or mix info ;D 
            try
            {
                string query = "select * from Zoo";
                //the SqlDataAdapter can be imagined like an interface to make Tables usable by c#-Objects
                //or imagine like exectuing a query with an sql connection and it gets all the data into it so then you can use it to display it or w/e
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();
                    sqlDataAdapter.Fill(zooTable);

                    //Which information of zooTable in our DataTable should be shown in our ListBox with the name zooList? 
                    zooList.DisplayMemberPath = "Location";
                    //Which information should be delivered, when an item in our listbox is selected!
                    zooList.SelectedValuePath = "Id";
                    //The referance or connection to the Data the listbox should populate.
                    zooList.ItemsSource = zooTable.DefaultView;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        //This is how we show associated data
        private void ShowAssociatedAnimals()
        {
            //Always use Try&Catch functionality for sql connections so if any error should occur it doesnt crash or mix info ;D 
            try
            {
                //The @ZooId is a parameter that we use inside the query so that parameter can change in what we want to
                string query = "select * from Animal a inner join ZooAnimal za on a.Id = za.AnimalId where za.ZooId = @ZooId";


                //We use the SqlCommand because we can edit or add parameters into the query and we can do this using SqlCommand NOT SqlDataAdapter 
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);


                //the SqlDataAdapter can be imagined like an interface to make Tables usable by c#-Objects
                //or imagine like exectuing a query with an sql connection and it gets all the data into it so then you can use it to display it or w/e
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    //Now in here we modify the parameter that we added up into the query with the selection of ListBox in the app.
                    sqlCommand.Parameters.AddWithValue("@ZooId", zooList.SelectedValue);


                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);


                    //Which information of zooTable in our DataTable should be shown in our ListBox with the name zooList? 
                    animalList.DisplayMemberPath = "Name";
                    //Which information should be delivered, when an item in our listbox is selected!
                    animalList.SelectedValuePath = "Id";
                    //The referance or connection to the Data the listbox should populate.
                    animalList.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void zooList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAssociatedAnimals();
        }
    }
}
