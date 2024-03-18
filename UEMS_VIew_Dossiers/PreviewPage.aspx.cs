

using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System;
using System.Collections.Generic;


namespace UEMS_VIew_Dossiers
{
    public partial class PreviewPage : System.Web.UI.Page
    {

       //string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
        string ConnectionString = ConfigurationManager.ConnectionStrings["uespoir_connectionString_Doc"].ConnectionString;
        string path = "";
        protected void Page_Load(object sender, EventArgs e)

        {
            if (!IsPostBack)
            {
                // Retrieve the image URL from the session variable
                 path = Request.QueryString["path"].ToString().Replace("..", "");
                 path = "C:\\inetpub\\wwwroot\\" + path.Replace("..", "");


                if(File.Exists(path))
                {
                    byte[] imageBytes = File.ReadAllBytes(path);
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "inline; filename=output.pdf");

                    Response.BinaryWrite(imageBytes);
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    Response.Clear();
                    Response.Write("Path non trouvé...");
                }

            }

        }

        //public void selectImageFromDatabase(string DocumentID)
        //{
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(ConnectionString))
        //        {
        //            connection.Open();
        //            string query = String.Format("SELECT Path From Documents Where DocumentID={0}", DocumentID);

        //            using (SqlCommand command = new SqlCommand(query, connection))
        //            {
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {

        //                        byte[] imageData = (byte[])reader["Path"];
        //                        string base64String = Convert.ToBase64String(imageData);

        //                        // Set the image URL and dimensions
        //                        Image1.ImageUrl = "data:image/jpeg;base64," + base64String;
        //                        Image1.Style["width"] = "100%"; // Set the width as needed
        //                        Image1.Style["height"] = "auto"; // Maintain aspect ratio

        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write("Erreur de connection...!" + ex.Message);
        //    }
        //}




        //public void SlectData()
        //{
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(ConnectionString))
        //        {
        //            connection.Open();
        //            string query = String.Format("SELECT Donnees From Documents Where DocumentID={0}", DocumentID);

        //            using (SqlCommand command = new SqlCommand(query, connection))
        //            {
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {

        //                        byte[] imageData = (byte[])reader["Donnees"];
        //                        Response.ContentType = "application/pdf";
        //                        Response.AddHeader("Content-Disposition", "inline; filename=output.pdf");

        //                        Response.BinaryWrite(imageData);
        //                        Response.Flush();
        //                        Response.End();
        //                        //string base64String = Convert.ToBase64String(imageData);

        //                        //// Set the image URL and dimensions
        //                        //Image1.ImageUrl = "data:image/jpeg;base64," + base64String;
        //                        //Image1.Style["width"] = "100%"; // Set the width as needed
        //                        //Image1.Style["height"] = "auto"; // Maintain aspect ratio

        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write("Erreur de connection...!" + ex.Message);
        //    }



        //}




    }
}
