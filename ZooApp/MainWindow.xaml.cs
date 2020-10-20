using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System;
using System.Windows.Controls;
using System.Data.SQLite;
using System.Reflection;

namespace ZooApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Project Name
        string projectName = Assembly.GetExecutingAssembly().GetName().Name.ToString();

        //We must define the sqlConnection part so it creates a new connection when the program starts
        SqlConnection sqlConnection;
        string connectionString;

        //SQLite Connection
        SQLiteConnection sqliteConnection;
        SQLiteCommand sqliteCommand;
        string sqliteConnectionString;

        DbCreation_SQLite sqlite = new DbCreation_SQLite();

        public MainWindow()
        {
            //string connectionString;
            InitializeComponent();
            //We use ConnectionString to Define where to connect to, <using System.Configuration|| We add this in References.
            //Then we write this down and in "" We write the Project Name.Properties.Settings.<In here we write the name of the database> and so forth.
            if (System.IO.Directory.Exists("C:/Users/Arbnor"))
                connectionString = ConfigurationManager.ConnectionStrings["ZooApp.Properties.Settings.ZooAppDbConnectionString"].ConnectionString;
            else
                connectionString = ConfigurationManager.ConnectionStrings["ZooApp.Properties.Settings._UdemySQLConnectionString"].ConnectionString;

            //SQLite
            sqliteConnectionString = System.Environment.CurrentDirectory + "\\DB\\" + projectName;
            sqlite.createDbFile(projectName);
            sqliteConnection = new SQLiteConnection(sqliteConnectionString);

            //SQL
            sqlConnection = new SqlConnection(connectionString);

            //and we initialize it and add the connection string so everytime the connection starts and gets inicialized it knows where to connect to.
            //sqlConnection = new SqlConnection(connectionString);
            ShowZoos();
            ShowAnimals();
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
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void ShowAnimals()
        {
            //Always use Try&Catch functionality for sql connections so if any error should occur it doesnt crash or mix info ;D 
            try
            {
                string query = "select * from Animal";
                //the SqlDataAdapter can be imagined like an interface to make Tables usable by c#-Objects
                //or imagine like exectuing a query with an sql connection and it gets all the data into it so then you can use it to display it or w/e
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable animalLB = new DataTable();
                    sqlDataAdapter.Fill(animalLB);

                    //Which information of zooTable in our DataTable should be shown in our ListBox with the name zooList? 
                    animalShowLB.DisplayMemberPath = "Name";
                    //Which information should be delivered, when an item in our listbox is selected!
                    animalShowLB.SelectedValuePath = "Id";
                    //The referance or connection to the Data the listbox should populate.
                    animalShowLB.ItemsSource = animalLB.DefaultView;
                }
            }
            catch (Exception e)
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
                //MessageBox.Show(e.ToString());
            }
        }
        private void zooList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAssociatedAnimals();
        }
        private void Delete_Zoo(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Zoo where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", zooList.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
        }
        private void Delete_Animal(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Animal where Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalId", animalShowLB.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals();
            }
        }
        private void Add_Zoo(object sender, RoutedEventArgs e)
        {
            try
            {
                string location = TextBox.Text;
                sqliteCommand = new SQLiteCommand("insert into Zoo (Location) values (?)", sqliteConnection);
                sqliteCommand.Parameters.Add(;
                sqliteCommand.ExecuteNonQuery();

                string query = "insert into Zoo values (@Location)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Location", TextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
                TextBox.Clear();
            }
        }
        private void Add_Animal(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Animal values (@Name)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", TextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals();
                TextBox.Clear();
            }
        }
        private void AddAnimalToZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into ZooAnimal values (@AnimalId, @ZooId)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", zooList.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalId", animalShowLB.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
            }
        }
        private void RemoveAnimalToZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from ZooAnimal where AnimalId = @AnimalId and ZooId = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", zooList.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalId", animalList.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
            }
        }
        private void UpdateZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Zoo set Location = @Location where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Location", TextBox.Text);
                sqlCommand.Parameters.AddWithValue("@ZooId", zooList.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
                TextBox.Clear();
            }
        }
        private void UpdateAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Animal set Name = @Name where Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", TextBox.Text);
                sqlCommand.Parameters.AddWithValue("@AnimalId", animalShowLB.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals();
                ShowAssociatedAnimals();
                TextBox.Clear();
            }
        }
    }
}
