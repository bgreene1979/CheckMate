using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Data.SqlClient;
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
    public sealed partial class Home : Page
    {
        public Home()
        {
            this.InitializeComponent();
        }
        public class Mates
        {
            public string id { get; set; }
            public string FName { get; set; }
            public string Lname { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }
            public string ScrName { get; set; }
            public string Ste_ID { get; set; }
            public byte[] Pic { get; set; }
            public string PicPath { get; set; }
        }

        public static Mates ResultMate = new Mates();

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string Lname = txtLastName.Text.ToString();
            string Fname = txtFirstName.Text.ToString();
            string Zip = txtZip.Text.ToString();
            string City = txtCity.Text.ToString();
            string State = txtState.Text.ToString();

            ResultMate = Search (Lname, Fname, City, State);
            if (ResultMate.id is null)
            {
                await new MessageDialog("No results returned, please check your spelling and try again").ShowAsync();

            }
            else
            {
                this.Frame.Navigate(typeof(Results));
            }
        }

        public Mates Search(string Lname, string Fname, string City, string State)
        {
            Mates newMate = new Mates();
            SqlConnection conn = new SqlConnection(MainPage.connString);
            conn.Open();
            string selectSql = "select * from Mate where Fname = '" + Fname + "' and Lname = '" + Lname + "' and City = '" + City + "' and State = '" + State + "'";
            SqlCommand com = new SqlCommand(selectSql, conn);

            if (conn.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    SqlDataReader read = com.ExecuteReader();
                    while (read.Read())
                    {
                        newMate.id = (read["ID"].ToString());
                        newMate.FName = (read["Fname"].ToString());
                        newMate.Lname = (read["Lname"].ToString());
                        newMate.City = (read["City"].ToString());
                        newMate.State = (read["State"].ToString());
                        newMate.Zip = (read["Zip"].ToString());
                        newMate.Pic = read["MatePic"] as byte[] ?? null;
                        newMate.PicPath = (read["MatePicPath"].ToString());
                        
                    }
                    
                }
                catch (Exception e)
                {
                    new MessageDialog(e.Message);
                }
                finally
                {
                    conn.Close();
                }
            }

            return newMate;

        }
    }
}
