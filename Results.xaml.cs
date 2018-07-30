using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
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
using Windows.UI.Popups;
using Windows.Storage;
using System.Net.Http;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CheckMate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Results : Page
    {
        public Results()
        {

            this.InitializeComponent();
            txtResults.Text = Home.ResultMate.FName + " " + Home.ResultMate.Lname;
            Search(Home.ResultMate.id);
        }

        public class Rate
        {
            public string Mate_ID { get; set; }
            public string User_ID { get; set; }
            public string Ctgy_ID { get; set; }
            public string Ratings { get; set; }
        }

        //public Rate[] FinalRate = new Rate[5]("1", "2", "3", "4"); 
        public Rate relRate = new Rate();
        public Rate respRate = new Rate();
        public Rate faithRate = new Rate();
        public Rate crazyRate = new Rate();
        public Rate crimRate = new Rate();


        public void Search(string Mate_ID)
        {
            //FinalRate;
            relRate.Mate_ID = Home.ResultMate.id;
            ConnectAsync();
            LoadMatePic();
            LoadImages(relRate, respRate, faithRate, crazyRate, crimRate);
        }

        public async void ConnectAsync()
        {

            SqlConnection conn = new SqlConnection(MainPage.connString);
            conn.Open();

            relRate.Ratings = await GetRelationshipRatings(conn);
            respRate.Ratings = await GetResponsibilityRatings(conn);
            faithRate.Ratings = await GetFaithfulnessRatings(conn);
            crazyRate.Ratings = await GetCrazyRatings(conn);
            crimRate.Ratings = await GetCriminalRatings(conn);

            //conn.Close();

        }

        public async Task<string> GetRelationshipRatings(SqlConnection conn)
        {
            relRate.Ctgy_ID = "1";
            int Average = 0;
            string selectRelRatings = "select* from Rate where Mate_ID = " + Home.ResultMate.id + " and Ctgy_ID = 1";
            SqlCommand com = new SqlCommand(selectRelRatings, conn);
            List<string> relRating = new List<string>();

            try
            {
                SqlDataReader read = com.ExecuteReader();
                while (read.Read())
                {
                    relRating.Add(read["Rating"].ToString());
                }
                read.Close();

            }
            catch (Exception e)
            {
                new MessageDialog(e.Message);
            }

            Average = await GetAverage(relRating);
            return Average.ToString();

        }

        public async Task<string> GetResponsibilityRatings(SqlConnection conn)
        {
            int Average = 0;
            string selectRespRatings = "select* from Rate where Mate_ID = " + Home.ResultMate.id + " and Ctgy_ID = 2";
            SqlCommand com = new SqlCommand(selectRespRatings, conn);
            List<string> respRating = new List<string>();

            try
            {
                SqlDataReader read = com.ExecuteReader();
                while (read.Read())
                {
                    respRating.Add(read["Rating"].ToString());
                }
                read.Close();

            }
            catch (Exception e)
            {
                new MessageDialog(e.Message);
            }

            Average = await GetAverage(respRating);
            return Average.ToString();

        }

        public async Task<string> GetFaithfulnessRatings(SqlConnection conn)
        {
            int Average = 0;
            string selectFaithRatings = "select* from Rate where Mate_ID = " + Home.ResultMate.id + " and Ctgy_ID = 3";
            SqlCommand com = new SqlCommand(selectFaithRatings, conn);
            List<string> faithRating = new List<string>();

            try
            {
                SqlDataReader read = com.ExecuteReader();
                while (read.Read())
                {
                    faithRating.Add(read["Rating"].ToString());
                }
                read.Close();

            }
            catch (Exception e)
            {
                new MessageDialog(e.Message);
            }

            Average = await GetAverage(faithRating);
            return Average.ToString();

        }

        public async Task<string> GetCrazyRatings(SqlConnection conn)
        {
            int Average = 0;
            string selectCrazyRatings = "select* from Rate where Mate_ID = " + Home.ResultMate.id + " and Ctgy_ID = 4";
            SqlCommand com = new SqlCommand(selectCrazyRatings, conn);
            List<string> crazyRating = new List<string>();

            try
            {
                SqlDataReader read = com.ExecuteReader();
                while (read.Read())
                {
                    crazyRating.Add(read["Rating"].ToString());
                }
                read.Close();

            }
            catch (Exception e)
            {
                new MessageDialog(e.Message);
            }

            Average = await GetAverage(crazyRating);
            return Average.ToString();

        }

        public async Task<string> GetCriminalRatings(SqlConnection conn)
        {
            int Average = 0;
            string selectCriminalRatings = "select* from Rate where Mate_ID = " + Home.ResultMate.id + " and Ctgy_ID = 5";
            SqlCommand com = new SqlCommand(selectCriminalRatings, conn);
            List<string> criminalRating = new List<string>();

            try
            {
                SqlDataReader read = com.ExecuteReader();
                while (read.Read())
                {
                    criminalRating.Add(read["Rating"].ToString());
                }
                read.Close();

            }
            catch (Exception e)
            {
                new MessageDialog(e.Message);
            }

            Average = await GetAverage(criminalRating);
            return Average.ToString();

        }

        public async Task<int> GetAverage(List<string> Rating)
        {
            int singleRating = 0;
            int final = 0;

            for (int x = 0; x < Rating.Count; x++)
            {
                singleRating += Int32.Parse(Rating[x]);
            }

            final = singleRating / Rating.Count;

            return final;

        }

        private void LoadImages(Rate relRate, Rate respRate, Rate faithRate, Rate crazyRate, Rate crimRate)
        {
            string relImgPath = "";
            string respImgPath = "";
            string faithImgPath = "";
            string crazyImgPath = "";
            string crimImgPath = "";

            switch (relRate.Ratings)
            {
                case "1":
                    relImgPath = "/assets/1star.jpg";
                    break;
                case "2":
                    relImgPath = "/assets/2star.jpg";
                    break;
                case "3":
                    relImgPath = "/assets/3star.jpg";
                    break;
                case "4":
                    relImgPath = "/assets/4star.jpg";
                    break;
                case "5":
                    relImgPath = "/assets/5star.jpg";
                    break;
                default:
                    relImgPath = "/assets/6star.jpg";
                    break;
            }

            switch (respRate.Ratings)
            {
                case "1":
                    respImgPath = "/assets/1star.jpg";
                    break;
                case "2":
                    respImgPath = "/assets/2star.jpg";
                    break;
                case "3":
                    respImgPath = "/assets/3star.jpg";
                    break;
                case "4":
                    respImgPath = "/assets/4star.jpg";
                    break;
                case "5":
                    respImgPath = "/assets/5star.jpg";
                    break;
                default:
                    respImgPath = "/assets/6star.jpg";
                    break;
            }

            switch (faithRate.Ratings)
            {
                case "1":
                    faithImgPath = "/assets/1star.jpg";
                    break;
                case "2":
                    faithImgPath = "/assets/2star.jpg";
                    break;
                case "3":
                    faithImgPath = "/assets/3star.jpg";
                    break;
                case "4":
                    faithImgPath = "/assets/4star.jpg";
                    break;
                case "5":
                    faithImgPath = "/assets/5star.jpg";
                    break;
                default:
                    faithImgPath = "/assets/6star.jpg";
                    break;
            }

            switch (crazyRate.Ratings)
            {
                case "1":
                    crazyImgPath = "/assets/1star.jpg";
                    break;
                case "2":
                    crazyImgPath = "/assets/2star.jpg";
                    break;
                case "3":
                    crazyImgPath = "/assets/3star.jpg";
                    break;
                case "4":
                    crazyImgPath = "/assets/4star.jpg";
                    break;
                case "5":
                    crazyImgPath = "/assets/5star.jpg";
                    break;
                default:
                    crazyImgPath = "/assets/6star.jpg";
                    break;
            }

            switch (crimRate.Ratings)
            {
                case "1":
                    crimImgPath = "/assets/1star.jpg";
                    break;
                case "2":
                    crimImgPath = "/assets/2star.jpg";
                    break;
                case "3":
                    crimImgPath = "/assets/3star.jpg";
                    break;
                case "4":
                    crimImgPath = "/assets/4star.jpg";
                    break;
                case "5":
                    crimImgPath = "/assets/5star.jpg";
                    break;
                default:
                    crimImgPath = "/assets/6star.jpg";
                    break;
            }

            imgRelationship.Source = new BitmapImage(new Uri(base.BaseUri, @relImgPath));
            imgResponsibility.Source = new BitmapImage(new Uri(base.BaseUri, @respImgPath));
            imgFaithfulness.Source = new BitmapImage(new Uri(base.BaseUri, @faithImgPath));
            imgCrazy.Source = new BitmapImage(new Uri(base.BaseUri, @crazyImgPath));
            imgCriminal.Source = new BitmapImage(new Uri(base.BaseUri, @crimImgPath));
           
        }

        public async void LoadMatePic()
        {

            using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
            {
                // Writes the image byte array in an InMemoryRandomAccessStream
                // that is needed to set the source of BitmapImage.
                using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(Home.ResultMate.Pic);
                    await writer.StoreAsync();
                }

                var image = new BitmapImage();
                await image.SetSourceAsync(ms);
                imgMatePic.Source = image;
            }
        }

        private void hplSearch_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Home));
        }
    }
}
