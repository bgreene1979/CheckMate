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




// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CheckMate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public class Users
        {
            public string id { get; set; }
            public string FullName { get; set; }
            public string UserID { get; set; }
            public string Password { get; set; }
            public string paid { get; set; }
        }


        public static string connString = "Data Source=(local);Initial Catalog=CheckMate;Integrated Security=True";


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = await Connect(txtUserName.Text.ToString()); 

            if(isValid == true)
            {
                this.Frame.Navigate(typeof(Home));
            }
        }

        private async Task<bool> Login(string UserName, SqlConnection con)
        {
            Users logUser = new Users();
            string selectSql = "select * from Users where login = '" + UserName + "'";
            SqlCommand com = new SqlCommand(selectSql, con);

            bool Valid = false;

            try
            {
                SqlDataReader read = com.ExecuteReader();
                while (read.Read())
                {
                   //List<Users> User;
                   logUser.Password = (read["password"].ToString());
                    logUser.paid = (read["paid"].ToString());
                }


                if(logUser.Password == txtPassword.Password.ToString())
                {
                    if (logUser.paid == "T")
                    {
                        Valid = true;
                    }
                    else
                    {
                        await new MessageDialog("Account not up to date, if you feel you have gotten this message in error, please email bgreene734@gmail.com").ShowAsync();
                    }                    
                }
                else
                {
                    await new MessageDialog("Wrong User name or password please try again").ShowAsync();
                    txtPassword.Password = "";
                }
 
            }
            catch(Exception e)
            {
               new MessageDialog(e.Message);
            }
            finally
            {
                con.Close();
            }

            return Valid;

        }

        private async Task<bool> Connect(string UserName)
        {
            bool isValid = false;
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            if (conn.State == System.Data.ConnectionState.Open)
            {
                isValid = await Login(UserName, conn);
            }

            return isValid;


        }

        private void txtPassword_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            /*if (e. == Keys.Enter)
            {
                Button_Click(this, new RoutedEventArgs());
            }*/
        }

        private void hplRegister_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Register));
        }

        private void hplAddMate_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Add_Mate));
        }
    }
}
