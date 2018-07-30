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
    public sealed partial class Register : Page
    {
        public Register()
        {
            this.InitializeComponent();
        }

        public class Users
        {
            public string Id { get; set; }
            public string Fname { get; set; }
            public string Lname { get; set; }
            public string Login { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Password2 { get; set; }
            public string Paid { get; set; }
        }

        async private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            bool GoodData = true;
            bool GoodFields = await CheckEmpty();
            

            //create new user object from text boxes
            Users item = new Users
            {
                Email = txtEmail.Text,
                Fname = txtFname.Text,
                Lname = txtLname.Text,
                Login = txtUserName.Text,
                Password = txtPassword.Password.ToString(),
                Password2 = txtPassword2.Password.ToString()
            };

            //check if the login already exists
            bool LoginExists = await CheckUserExists(item);
            //check if email is valid
            bool IsValidEmail = await ValidEmail(item);
            //check email exists
            bool EmailExists = await CheckEmailExists(item);
            //check if passwords match
            bool ArePasswordsMatching = await MatchingPasswords(item);

            GoodData = GoodFields && LoginExists && EmailExists && IsValidEmail && ArePasswordsMatching;


            if (GoodData)
            {
                await Register_User(item);
            }

            btnSubmit.IsEnabled = true;

        }

        private async Task Register_User(Users item)
        {
            try
            {
                //insert user
                await InsertUser(item);

                //clear password fields
                txtPassword.Password = "";
                txtPassword2.Password = "";

                //navigate to login pages
                this.Frame.Navigate(typeof(MainPage));

            }
            catch (Exception e)
            {
                MessageDialog messageDialog = new MessageDialog("An Error Occurred: " + e.Message, "CheckMate");
                await messageDialog.ShowAsync();
            }
        }

        private async Task<bool> InsertUser(Users item)
        {
            bool inserted = false;
            try
            {
                string connectionString = "Data Source=(local);Initial Catalog=CheckMate;Integrated Security=True";
                SqlConnection insertconn = new SqlConnection(connectionString);
                insertconn.Open();
                string insertSql = "insert into Users (ID, Fname, Lname, login, email, password, paid) Values ((select max(ID) + 1 from Users), '" + item.Fname + "', '" + item.Lname + "', '" + item.Login + "', '" + item.Email + "' , '" + item.Password + "', 'T')";
                SqlCommand com = new SqlCommand(insertSql, insertconn);
                com.ExecuteNonQuery();

                MessageDialog messageDialog = new MessageDialog("Thanks for registering " + item.Login, "CheckMate");
                await messageDialog.ShowAsync();

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


        private async Task<bool> CheckEmpty()
        {
            bool validFields = false;

            bool v = txtFname.Text.Length != 0 && txtLname.Text.Length != 0 && txtEmail.Text.Length != 0 && txtUserName.Text.Length != 0 && txtPassword.Password.Length != 0 && txtPassword2.Password.Length != 0;
            if (v)
            {
                validFields = true;
            }
            else
            {
                MessageDialog messageDialog = new MessageDialog("All Fields are Required", "CheckMate");
                await messageDialog.ShowAsync();
                validFields = false;

            }

            return validFields;

        }

        private async Task<bool> CheckUserExists(Users item)
        {
            bool exists = false;
            string existLogin = "";
            string connectionString = "Data Source=(local);Initial Catalog=CheckMate;Integrated Security=True";
            SqlConnection selectconn = new SqlConnection(connectionString);
            selectconn.Open();
            string insertSql = "select * from Users where login = '" + item.Login + "'";
            SqlCommand com = new SqlCommand(insertSql, selectconn);
            com.ExecuteNonQuery();

            if (selectconn.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    SqlDataReader read = com.ExecuteReader();
                    while (read.Read())
                    {
                      existLogin = (read["Login"].ToString());
                    }

                }
                catch (Exception e)
                {
                    new MessageDialog(e.Message);
                }
                finally
                {
                    selectconn.Close();
                }
            }

            if (existLogin.Length > 0)
            {
                exists = true;
                MessageDialog messageDialog = new MessageDialog("This login name is already being used, please try another name", "CheckMate");
                await messageDialog.ShowAsync();
            }

            return exists;

        }

        private async Task<bool> CheckEmailExists(Users item)
        {
            bool exists = false;
            string existEmail = "";
            string connectionString = "Data Source=(local);Initial Catalog=CheckMate;Integrated Security=True";
            SqlConnection selectconn = new SqlConnection(connectionString);
            selectconn.Open();
            string insertSql = "select * from Users where login = '" + item.Email + "'";
            SqlCommand com = new SqlCommand(insertSql, selectconn);
            com.ExecuteNonQuery();

            if (selectconn.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    SqlDataReader read = com.ExecuteReader();
                    while (read.Read())
                    {
                        existEmail = (read["Email"].ToString());
                    }

                }
                catch (Exception e)
                {
                    new MessageDialog(e.Message);
                }
                finally
                {
                    selectconn.Close();
                }
            }

            if (existEmail.Length > 0)
            {
                exists = true;
                MessageDialog messageDialog = new MessageDialog("This email address is already being used, please try another email", "CheckMate");
                await messageDialog.ShowAsync();
            }

            return exists;

        }

        private async Task<bool> ValidEmail(Users item)
        {
            bool validEmail = false;

            if (txtEmail.Text.Contains('@') && txtEmail.Text.Contains('.'))
            {
                validEmail = true;
            }
            else
            {
                MessageDialog messageDialog = new MessageDialog("Please enter a valid email address", "CheckMate");
                await messageDialog.ShowAsync();
                validEmail = false;
            }

            return validEmail;

        }

        private async Task<bool> MatchingPasswords(Users item)
        {
            bool match = false;

            if (txtPassword.Password == txtPassword2.Password)
            {
                match = true;
            }
            else
            {
                MessageDialog messageDialog = new MessageDialog("Passwords do not match", "CheckMate");
                await messageDialog.ShowAsync();
                match = false;
            }

            return match;
        }

        private void hplLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
