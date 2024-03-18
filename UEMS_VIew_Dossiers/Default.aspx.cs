using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Web;
using System.Security.Principal;

namespace UEMS_VIew_Dossiers
{
    public partial class Default : System.Web.UI.Page
    {
        //string ConnectionString = XCryptEngine.ConnectionStringEncryption.Decrypt(ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString);
        string ConnectionString = ConfigurationManager.ConnectionStrings["uespoir_connectionString_Doc"].ConnectionString;
        string ConnectionString_per = ConfigurationManager.ConnectionStrings["uespoir_connectionString"].ConnectionString;
        public static string sPersonneID = string.Empty;
        //static string inetpubPath = Server.MapPath("~/");

        //string folderPath = "C:\\Users\\Administrator\\source\\repos\\UEMS_VIew_Dossiers\\UEMS_VIew_Dossiers\\UEspoirImages\\0203BC5F-CC1D-426F-8183-4E477CAEE024";

        string folderPath =$"C:\\inetpub\\wwwroot\\UEspoirImages\\{sPersonneID}";
        // private static string[] previousFilePaths;
        static string querryInsert = "Insert Into Documents (PersonneID,TypeDocumentID,path,autres,CreerParUsername) Values(@PersonneID, @TypeDocumentID, @path,@autres,@CreerParUsername)";
    
        List<Dossier> dossiers = new List<Dossier>();
     
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    sPersonneID = Request.QueryString["PersonneID"];
                    Response.Cookies["PersonneID"].Expires = DateTime.Now.AddDays(-1);

                    //sPersonneID = "6EFBAA77-0B0D-4A8C-BC76-124B102D284A";
                    //sPersonneID = "6EFBAA77-0B0D-4A8C-BC76-124B102D284A";
                    /// txtPersonneID.Text = sPersonneID;

                }
                catch (Exception ex)
                {

                }
                //BindGridView();
                BindInfoToGridView();

                findStudentName(sPersonneID);
            }

        }

        private void findStudentName(string studentID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString_per))
                {
                    connection.Open();
                    string query = String.Format("SELECT Nom, Prenom From Personnes where PersonneID = '{0}'", studentID);

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                lblInfoEtudiant.Text += reader["Prenom"].ToString() + " " + reader["Nom"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.ForeColor = Color.Red;
            }
        }
        //private void BindGridView()
        //{

        //    using (SqlConnection connection = new SqlConnection(ConnectionString))
        //    {
        //        connection.Open();
        //        string query = String.Format("SELECT D.DocumentID, P.Nom, P.Prenom, D.Donnees, T.Description FROM Personnes AS P JOIN Documents AS D ON P.PersonneID = D.PersonneID JOIN TypeDocument AS T ON D.TypeDocumentID = T.TypeDocumentID WHERE P.PersonneID = '{0}'", sPersonneID);

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {

        //                while (reader.Read())
        //                {
        //                    byte[] imageData = (byte[])reader["Donnees"];
        //                    string base64String = Convert.ToBase64String(imageData);
        //                    dossiers.Add(new Dossier
        //                    {
        //                        documentId = Int32.Parse(reader["DocumentID"].ToString()),
        //                        ImageData = "data:image/png;base64," + PdfToImage(imageData), //"data:image/jpeg;base64," + base64String, // Add a property for image data in your Dossier class
        //                        description = reader["Description"].ToString()
        //                    });
        //                }
        //            }
        //        }

        //        GridView1.DataSource = dossiers;
        //        GridView1.DataBind();
        //    }
        //}


        private static System.Drawing.Image ConvertByteArrayToImage(byte[] byteArray)
        {
            try
            {
                if (byteArray == null || byteArray.Length == 0)
                {
                    // Handle the case where the byte array is null or empty
                    return null;
                }

                using (MemoryStream memoryStream = new MemoryStream(byteArray))
                {
                    // Try to create an Image object from the byte array
                    System.Drawing.Image image = System.Drawing.Image.FromStream(memoryStream);

                    // Check if the image is in a valid format
                    if (image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.MemoryBmp.Guid)
                    {
                        // The image is in an invalid format (e.g., MemoryBmp)
                        return null;
                    }

                    return image;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                // Console.WriteLine($"Error converting byte array to image: {ex.Message}");
                return null;
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName == "Preview")
            {

               
                // Get the index of the row where the button was clicked
                int rowIndex = Convert.ToInt32(e.CommandArgument);

                GridViewRow row = GridView1.Rows[rowIndex];
                Label lblPath = (Label)row.FindControl("lblPath");
                string path = lblPath.Text;

                Response.Redirect(string.Format("PreviewPage.aspx?path={0}", path));

            }
            if (e.CommandName == "addNew")
            {
                 DropDownList dropDownFromFooter = (DropDownList)GridView1.FooterRow.FindControl("ddlDescription");

                
            
                // Find controls within the GridView row
                FileUpload fileUploadFooter = (FileUpload)GridView1.FooterRow.FindControl("fileUploadID");

                if (dropDownFromFooter.SelectedValue == string.Empty || dropDownFromFooter.SelectedIndex == 0)
                {
                    lblError.Text = "Choisissez le type de fichier...";
                    lblError.ForeColor = Color.Red;
                    return;
                }
                if (fileUploadFooter.PostedFile != null && fileUploadFooter.PostedFile.ContentLength > 0)
                {
                    // The FileUpload control has a file
                    string fileName = Path.GetFileName(fileUploadFooter.FileName);
                    string file_name = dropDownFromFooter.SelectedItem.Text + ".pdf";
                    int typeDoc = dropDownFromFooter.SelectedIndex;
                    // string tempFilePath = Path.Combine(Server.MapPath("~\\"), fileName);
                    string tempFilePath = @"C:\inetpub\wwwroot\Folder_to_save_temporaly\\" + fileName;

                    fileUploadFooter.SaveAs(tempFilePath);

                     string filePath = Path.Combine(folderPath, file_name);
                    if (File.Exists(filePath))
                    {
                           lblError.Text = "File already exists.Please updating this file..";
                           lblError.ForeColor = Color.Red;
                           File.Delete(tempFilePath);

                    }
                    else
                    {
                        
                        byte[] fileBytes = File.ReadAllBytes(tempFilePath);

                        File.WriteAllBytes(filePath, fileBytes);
                        string newPath;
                        DeleteAllExceptLastFour(filePath, out newPath);
                        

                        InsertIntoDatabase(ConnectionString, querryInsert, sPersonneID, typeDoc, newPath);
                        // Call a function which take the file path as an arguent then insert it to 
                        //database

                        File.Delete(tempFilePath);

                        



                        BindInfoToGridView();
                        Response.Redirect(string.Format("Default.aspx?PersonneID={0}", sPersonneID));
                        lblError.Text = "Row Added successfully.";
                        lblError.ForeColor = Color.Green;


                    }


                    

                }
                else
                {
                    lblError.Text = "Choisissez un fichier...";
                    lblError.ForeColor = Color.Red;
                }
            }
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {

            GridView1.EditIndex = e.NewEditIndex;
            BindInfoToGridView();

        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            lblError.Text = string.Empty;
            //// Accessing the updated values
            ///
            string id = GridView1.DataKeys[e.RowIndex].Values["documentId"].ToString();

            int documentID = Int32.Parse(id);
            // You can now use the 'id' and 'name' variables as needed.

            GridViewRow editingRow = GridView1.Rows[e.RowIndex];

            string findOldPath= "C:\\inetpub\\wwwroot\\";
           
            Label lblFilePath = GridView1.Rows[e.RowIndex].FindControl("lblPath") as Label;
            string oldFilePath = lblFilePath.Text.Replace("..", "");
            oldFilePath = findOldPath + oldFilePath;
           // Find the FileUpload control in the editing row
           FileUpload fileUpload = (FileUpload)editingRow.FindControl("fileUploadID");
            DropDownList ddlDescription = (DropDownList)editingRow.FindControl("ddlDescription");

            if (ddlDescription.SelectedValue == string.Empty || ddlDescription.SelectedIndex == 0)
            {
                lblError.Text = "Choisissez le type de fichier...";
                lblError.ForeColor = Color.Red;
                return;
            }
            else
            {
                if (fileUpload.PostedFile != null && fileUpload.PostedFile.ContentLength > 0)
                {
                    // The FileUpload control has a file
                    string fileName = Path.GetFileName(fileUpload.FileName);

                    //string tempFilePath = Path.Combine(Server.MapPath("~\\"), fileName);
                    string tempFilePath = @"C:\inetpub\wwwroot\Folder_to_save_temporaly" + fileName;

                    fileUpload.SaveAs(tempFilePath);
                  
                    byte[] fileBytes = File.ReadAllBytes(tempFilePath);

                    string file_name = ddlDescription.SelectedItem.Text + ".pdf";

                    string filePath = Path.Combine(folderPath, file_name);
                  
                    File.WriteAllBytes(filePath, fileBytes);

                    //  string[] info = filePath.Split('\\');
                    //  List<string> infoList = new List<string>(info);



                    string newPath;
                    DeleteAllExceptLastFour(filePath, out newPath);
                   
                    if (oldFilePath!=null & newPath != null) {

                       File.Delete(oldFilePath);
                       File.WriteAllBytes(filePath, fileBytes);

                    }


                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();

                        // Create the SQL UPDATE statement
                        string updateQuery = "UPDATE Documents SET path = @path , TypeDocumentID = @TypeDocumentID , CreerParUsername= @CreerParUsername  WHERE DocumentID = @DocumentID";

                        using (SqlCommand command = new SqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@path", newPath);
                            command.Parameters.AddWithValue("@TypeDocumentID", Int32.Parse(ddlDescription.SelectedValue));
                            command.Parameters.AddWithValue("@DocumentID", documentID);
                            command.Parameters.AddWithValue("@autres", String.Empty);
                            command.Parameters.AddWithValue("@CreerParUsername", Environment.UserName);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                lblError.Text = "Row updated successfully.";
                                lblError.ForeColor = Color.Green;
                                File.Delete(tempFilePath);
                                BindInfoToGridView();
                                Response.Redirect(string.Format("Default.aspx?PersonneID={0}", sPersonneID));
                            }
                            else
                            {
                                lblError.Text = "No rows were updated.";
                                lblError.ForeColor = Color.Red;
                            }

                        }
                    }
                    // Cancel the edit mode
                    GridView1.EditIndex = -1;

                    BindInfoToGridView();
                }
                else
                {
                    lblError.Text = "Vous devez choisir un fichier";
                    lblError.ForeColor = Color.Red;
                    return;
                }
            }

        }

       

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;

            BindInfoToGridView();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            // Rebind your data to refresh the GridView with the new page
            BindInfoToGridView();
       
        }

        private DataTable GetDataFromDatabase(String query)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    dt = new DataTable();
                    adapter.Fill(dt);

                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Erreur de connection, voir un technicien";
                lblError.ForeColor = Color.Red;
            }

            return dt;
        }

        //protected void AjouterDossiers(byte[] fileBytes, string TypeDocument)
        //{
        //    using (SqlConnection connection = new SqlConnection(ConnectionString))
        //    {
        //        connection.Open();

        //        // Create the SQL INSERT statement
        //        string insertQuery = "INSERT INTO Documents (PersonneID,TypeDocumentID, Donnees, CreeParUsername) VALUES (@PersonneID,@TypeDocumentID, @Donnees, @CreeParUsername)";

        //        using (SqlCommand command = new SqlCommand(insertQuery, connection))
        //        {
        //            command.Parameters.AddWithValue("@PersonneID", sPersonneID);
        //            command.Parameters.AddWithValue("@Donnees", fileBytes);
        //            command.Parameters.AddWithValue("@TypeDocumentID", TypeDocument);
        //            command.Parameters.AddWithValue("@CreeParUsername", Environment.UserName);

        //            int rowsAffected = command.ExecuteNonQuery();

        //            if (rowsAffected > 0)
        //            {
        //                lblError.Text = "Row updated successfully.";
        //                lblError.ForeColor = Color.Green;
        //                BindInfoToGridView();
        //                Response.Redirect(string.Format("Default.aspx?PersonneID={0}", sPersonneID));
        //            }
        //            else
        //            {
        //                lblError.Text = "No rows were updated.";
        //                lblError.ForeColor = Color.Red;
        //            }

        //        }
        //    }
        //}

        public String PdfToImage(byte[] pdfBytes)
        {
            //// Replace this with your actual PDF byte array
            //string base64String = string.Empty;

            //// Create an instance of Bytescout.PDFRenderer.RasterRenderer object and register it.
            //RasterRenderer renderer = new RasterRenderer();
            //renderer.RegistrationName = "demo";
            //renderer.RegistrationKey = "demo";

            //try
            //{
            //    // Load PDF document from byte array
            //    using (MemoryStream pdfStream = new MemoryStream(pdfBytes))
            //    {
            //        // Load the document without closing the MemoryStream
            //        renderer.LoadDocumentFromStream(pdfStream);

            //        // Assume you want to display the first page
            //        int pageIndex = 0;

            //        using (MemoryStream memoryStream = new MemoryStream())
            //        {
            //            // Render document page and save to memory stream
            //            renderer.Save(memoryStream, RasterImageFormat.PNG, pageIndex, 300);

            //            // Convert the MemoryStream to a byte array
            //            byte[] pngBytes = memoryStream.ToArray();

            //            // Convert the byte array to a Base64 string
            //            base64String = Convert.ToBase64String(pngBytes);


            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    lblError.Text = "Error: " + ex.Message;
            //}
            //finally
            //{
            //    // Cleanup
            //    renderer.Dispose();
            //}

            //return base64String;
            return null;

        }


        //static System.Drawing.Image ConvertPdfToImage(byte[] pdfBytes)
        //{
        //    using (MemoryStream pdfStream = new MemoryStream(pdfBytes))
        //    {
        //        using (PdfReader pdfReader = new PdfReader(pdfStream))
        //        {
        //            using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
        //            {
        //                // Extract text from the first page (adjust as needed)
        //                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
        //                string text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), strategy);

        //                // For simplicity, let's just create an image with the extracted text
        //                Bitmap image = new Bitmap(800, 600);
        //                using (Graphics g = Graphics.FromImage(image))
        //                {
        //                    g.DrawString(text, SystemFonts.DefaultFont, Brushes.Black, new PointF(10, 10));
        //                }

        //                return image;
        //            }
        //        }
        //    }
        //}
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.EmptyDataRow)
                {
                    // Get the DropDownList in the edit mode
                    DropDownList ddlDescription = (DropDownList)e.Row.FindControl("ddlDescriptionEmptyFooter");

                    // Check if the DropDownList is found
                    if (ddlDescription != null)
                    {
                        // Populate the DropDownList with data from the database
                        ddlDescription.DataSource = GetDataFromDatabase("Select * from TypeDocument"); // Implement this method to fetch data
                        ddlDescription.DataTextField = "Description"; // Set the appropriate field name
                        ddlDescription.DataValueField = "TypeDocumentID"; // Set the appropriate field name              
                        ddlDescription.DataBind();

                        // Optionally, set a default value
                        ddlDescription.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Choisir le type de fichier", "-1"));
                        ddlDescription.SelectedValue = "-1"; // Set the default value
                    }
                }
                else
                {
                    // Get the DropDownList in the edit mode
                    DropDownList ddlDescription = (DropDownList)e.Row.FindControl("ddlDescription");

                    // Check if the DropDownList is found
                    if (ddlDescription != null)
                    {
                        // Populate the DropDownList with data from the database
                        ddlDescription.DataSource = GetDataFromDatabase("Select * from TypeDocument"); // Implement this method to fetch data
                        ddlDescription.DataTextField = "Description"; // Set the appropriate field name
                        ddlDescription.DataValueField = "TypeDocumentID"; // Set the appropriate field name              
                        ddlDescription.DataBind();

                        // Optionally, set a default value
                        ddlDescription.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Choisir le type de fichier", "-1"));
                        ddlDescription.SelectedValue = "-1"; // Set the default value
                    }
                }

            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.ForeColor = Color.Red;
            }
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {

            FileUpload fileUploadfromFooterEmpty = (FileUpload)GridView1.Controls[0].Controls[1].FindControl("fileUploadEmptyFooter");
            DropDownList dropDownfromFooterEmpty = (DropDownList)GridView1.Controls[0].Controls[1].FindControl("ddlDescriptionEmptyFooter");
            if (dropDownfromFooterEmpty.SelectedValue == string.Empty || dropDownfromFooterEmpty.SelectedIndex == 0)
            {
                lblError.Text = "Choisissez le type de fichier...";
                lblError.ForeColor = Color.Red;
                return;
            }
            if (fileUploadfromFooterEmpty.PostedFile != null && fileUploadfromFooterEmpty.PostedFile.ContentLength > 0)
            {
                // The FileUpload control has a file
                string fileName = Path.GetFileName(fileUploadfromFooterEmpty.FileName);
                string tempFilePath = @"C:\inetpub\wwwroot\Folder_to_save_temporaly" + fileName;

                //string tempFilePath = Path.Combine(Server.MapPath("~\\"), fileName);
                fileUploadfromFooterEmpty.SaveAs(tempFilePath);

                byte[] fileBytes = File.ReadAllBytes(tempFilePath);

               
                File.Delete(tempFilePath);
            }
            else
            {
                lblError.Text = "Choisissez un fichier...";
                lblError.ForeColor = Color.Red;
            }
        }
        public class Dossier
        {
            public int index { get; set; }
            public int documentId { get; set; }
            public string Path { get; set; }
            public string newPath { get; set; }
            public string newTypeDocument { get; set; }
            public string description { get; set; }
            public string createParUser { get; set; }
        }
        private void BindInfoToGridView()
        {

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = String.Format("SELECT D.DocumentID,D.Path, D.Path, T.Description FROM Documents AS D JOIN TypeDocument AS T ON D.TypeDocumentID = T.TypeDocumentID WHERE PersonneID = '{0}'", sPersonneID);

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        
                        int i = 0;

                        while (reader.Read())
                        {
                            dossiers.Add(new Dossier
                            {
                                documentId = Int32.Parse(reader["DocumentID"].ToString()),
                                Path = reader["Path"].ToString(),
                                description = reader["Description"].ToString(),
                                index = i
                            });

                            i++;
                        }
                    }
                }

                GridView1.DataSource = dossiers;
                GridView1.DataBind();
            }
        }

        static string[] GetAllChildInFolder(string masterFolder)
        {
            // Check if the folder exists
            if (Directory.Exists(masterFolder))
            {
                // Retrieve all file paths in the folder
                string[] childFolder = Directory.GetDirectories(masterFolder);

               return childFolder;
            }
            else
            {
                Console.WriteLine("Master Folder not found.");
                return new string[0];
            }
        }

        

        static void InsertIntoDatabase(string connectionString, string query_insert, string PersonneID, int TypeDocument, string path)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query_insert, connection))

                    {

                        command.Parameters.AddWithValue("@PersonneID", PersonneID);
                        command.Parameters.AddWithValue("@TypeDocumentID", TypeDocument);
                        command.Parameters.AddWithValue("@path", path);
                        command.Parameters.AddWithValue("@autres", String.Empty);
                       command.Parameters.AddWithValue("@CreerParUsername", Environment.UserName);
                        command.ExecuteNonQuery();




                    }
                }

                Console.WriteLine("Data Insert successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error data not save into the database: {ex.Message}");
            }
        }

        static void DeleteAllExceptLastFour(string filePath, out string newPath)
        {
            string[] info = filePath.Split('\\');

            // Initialize the newPath
            newPath = "";

            // Check if the array has more than four elements
            if (info.Length > 4)
            {
                // Remove elements except the last four
                List<string> infoList = new List<string>(info);
                infoList.RemoveRange(0, infoList.Count - 4);
                info = infoList.ToArray();

                // Reconstruct the newPath
                newPath = $"{info[1]}\\{info[2]}\\{info[3]}";
            }

        }
          
    }
}
    
