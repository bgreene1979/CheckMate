using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Storage;
using System.Net.Http;
using Newtonsoft.Json;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CheckMate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Add_Mate : Page
    {
        public Add_Mate()
        {
            this.InitializeComponent();
        }

        public class Mate
        {
            public string ID { get; set; }
            public string Fname { get; set; }
            public string Lname { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }

        }

        public class Rate
        {
            public string Mate_ID { get; set; }
            public string User { get; set; }
            public string Ctgy_ID { get; set; }
            public string Rating { get; set; }

        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            Mate NewMate = new Mate
            {   
                Fname = txtFirstName.Text,
                Lname = txtLname.Text,
                City = txtCity.Text,
                State = txtState.Text,
                Zip = txtZip.Text
            };

            Rate RelRate = new Rate
            {
                Ctgy_ID = "1",
                Rating = rtnRelationship.Value.ToString()
            };

            Rate RespRate = new Rate
            {
                Ctgy_ID = "2",
                Rating = rtnRelationship.Value.ToString()

            };

            Rate FaithRate = new Rate
            {
                Ctgy_ID = "3",
                Rating = rtnFaithfulness.Value.ToString()
            };

            Rate CrazyRate = new Rate
            {
                Ctgy_ID = "4",
                Rating = rtnCrazy.Value.ToString()

            };

            Rate CrimRate = new Rate
            {
                Ctgy_ID = "5",
                Rating = rtnCriminal.Value.ToString()

            };

            //check fields
            bool validFields = await CheckMateData();

            //check if mate exists if it does then update, if not then add
            Mate CurrentMate = new Mate();
            
            CurrentMate = await CheckForMate(NewMate);

            if(CurrentMate.ID is null)
            {
                //insert mate 
                NewMate.ID = await InsertMate(NewMate);
                //insert Relationship Rating
                await InsertRate(RelRate, NewMate);

                //insert Responsiblity Rating
                await InsertRate(RespRate, NewMate);

                //insert Faithfulness Rating
                await InsertRate(FaithRate, NewMate);

                //insert Crazy Rating
                await InsertRate(CrazyRate, NewMate);

                //insert Criminal Rating
                await InsertRate(CrimRate, NewMate);


            }
            else
            {
                //insert Relationship Rating
                await InsertRate(RelRate, CurrentMate);

                //insert Responsiblity Rating
                await InsertRate(RespRate, CurrentMate);

                //insert Faithfulness Rating
                await InsertRate(FaithRate, CurrentMate);

                //insert Crazy Rating
                await InsertRate(CrazyRate, CurrentMate);

                //insert Criminal Rating
                await InsertRate(CrimRate, CurrentMate);
            }


        }

        private async Task<bool> CheckMateData()
        {
            bool validFields = true;
            int Number = 0;

            if(txtFirstName.Text.Length == 0)
            {
                MessageDialog messageDialog = new MessageDialog("First Name Required", "CheckMate");
                await messageDialog.ShowAsync();
                validFields = false;
            }

            if(txtLname.Text.Length == 0)
            {
                MessageDialog messageDialog = new MessageDialog("Last Name Required", "CheckMate");
                await messageDialog.ShowAsync();
                validFields = false;
            }

            if (txtCity.Text.Length == 0)
            {
                MessageDialog messageDialog = new MessageDialog("City Required", "CheckMate");
                await messageDialog.ShowAsync();
                validFields = false;
            }

            if (txtState.Text.Length == 0)
            {
                MessageDialog messageDialog = new MessageDialog("State Required", "CheckMate");
                await messageDialog.ShowAsync();
                validFields = false;
            }

            if (int.TryParse(txtZip.Text, out Number))
            {
                MessageDialog messageDialog = new MessageDialog("Zip Code Must Be Numeric", "CheckMate");
                await messageDialog.ShowAsync();
                validFields = false;
            }

            return validFields;

        }

        private async Task<Mate> CheckForMate(Mate newMate)
        {
            Mate CurrentMate = new Mate(); 

            try
            {
                string connectionString = "Data Source=(local);Initial Catalog=CheckMate;Integrated Security=True";
                SqlConnection searchconn = new SqlConnection(connectionString);
                searchconn.Open();
                string insertSql = "select * from Mate where Fname = '" + newMate.Fname + "' and Lname = '" + newMate.Lname + "' and City = '" + newMate.City + "' and State = '" + newMate.State + "'";
                SqlCommand com = new SqlCommand(insertSql, searchconn);
                com.ExecuteNonQuery();

                if (searchconn.State == System.Data.ConnectionState.Open)
                {
                    try
                    {
                        SqlDataReader read = com.ExecuteReader();
                        while (read.Read())
                        {
                            CurrentMate.ID = (read["ID"].ToString());
                            CurrentMate.Fname = (read["Fname"].ToString());
                            CurrentMate.Lname = (read["Lname"].ToString());
                            CurrentMate.City = (read["City"].ToString());
                            CurrentMate.State = (read["State"].ToString());
                            CurrentMate.Zip = (read["Zip"].ToString());
                            //newMate.Pic = read["MatePic"] as byte[] ?? null;
                        }

                    }
                    catch (Exception e)
                    {
                        new MessageDialog(e.Message);
                    }
                    finally
                    {
                        searchconn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                MessageDialog messageDialog2 = new MessageDialog("An Error Occurred: " + e.Message, "CheckMate");
                await messageDialog2.ShowAsync();
            }

            return CurrentMate;
        }

        private async Task<string> InsertMate(Mate newMate)
        {
            //bool inserted = false;

            try
            {
                string connectionString = "Data Source=(local);Initial Catalog=CheckMate;Integrated Security=True";
                SqlConnection insertconn = new SqlConnection(connectionString);
                insertconn.Open();
                string insertSql = "insert into Mate (ID, Fname, Lname, City, State, Zip) Values ((select max(ID) + 1 from Mate), '" + newMate.Fname + "', '" + newMate.Lname + "', '" + newMate.City + "', '" + newMate.State + "' , '" + newMate.Zip + "')";
                SqlCommand com = new SqlCommand(insertSql, insertconn);
                com.ExecuteNonQuery();

                newMate = await CheckForMate(newMate);


                MessageDialog messageDialog = new MessageDialog("Thanks for Adding a User ", "CheckMate");
                await messageDialog.ShowAsync();

                insertconn.Close();
                //inserted = true;
            }
            catch (Exception e)
            {
                MessageDialog messageDialog2 = new MessageDialog("An Error Occurred: " + e.Message, "CheckMate");
                await messageDialog2.ShowAsync();
            }

            return newMate.ID;
        }

        private async Task<bool> InsertRate(Rate newRate, Mate newMate)
        {
            bool inserted = false;

            try
            {
                string connectionString = "Data Source=(local);Initial Catalog=CheckMate;Integrated Security=True";
                SqlConnection insertconn = new SqlConnection(connectionString);
                insertconn.Open();
                string insertSql = "insert into Rate (Mate_ID, Ctgy_ID, Rating) Values ('" + newMate.ID + "', '" + newRate.Ctgy_ID + "', '" + newRate.Rating + "') ";
                SqlCommand com = new SqlCommand(insertSql, insertconn);
                com.ExecuteNonQuery();

                insertconn.Close();
                inserted = true;
            }
            catch (Exception e)
            {
                MessageDialog messageDialog2 = new MessageDialog("An Error Occurred: " + e.Message, "CheckMate");
                await messageDialog2.ShowAsync();
            }

            return inserted;
        }

    }
}
