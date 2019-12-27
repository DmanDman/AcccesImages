using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;

namespace AcccesImagges
{
    public partial class FrmMain : Form
    {
        SubRoutine SubRtn = new SubRoutine();
        Constants consts = new Constants();
        
        public string DBPATH { get; private set; }
        public string DBPATHUNLOAD { get; private set; }
        public string DBNAME { get; private set; }
        public string DBCOMPACT { get; private set; }
        public string DBPWD { get; private set; }

        public FrmMain()
        {
            InitializeComponent();
            SubRtn.ReadIni();
            LblCount.Text = "0";
            LblTotal.Text = "0";
            LblTotalTime.Text = "";
            LblStartTime.Text = "";
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            LblElapsed.Text= "Loading database...";
            LblElapsed.Refresh();

            consts.StartTime = DateTime.Now;
            LblStartTime.Text = consts.StartTime.ToShortTimeString();
            LblStartTime.Refresh();

            BtnConvert.Enabled = false;

            //SubRtn.ConvertImages(PictureBox1, RichTextBox1, PictureBox2);

            SubRtn.ProcessPicTables(PicOrig, LblCount, LblTotal, LblTotalTime, LblElapsed, LblTable);
            
            SubRtn.UpdateTime(consts.StartTime, LblTotalTime);

            SubRtn.ExportTablesToJson(LblElapsed, LblTable);

            BtnConvert.Enabled = true;
        }

        public class PictureDetail
        {
            public int IDPicture { get; set; }
            public string PicName { get; set; }
            public int? IDSurgery { get; set; }
            public string Image { get; set; }
        }

        public class PictureBlank
        {
            public int IDPicture { get; set; }
            public string PicName { get; set; }
            public int? IDSurgery { get; set; }
        }

        public class PictureErr
        {
            public int IDPicture { get; set; }
            public string PicName { get; set; }
            public int? IDSurgery { get; set; }
            public string ImageString { get; set; }
        }

        public class ObjPicture
        {
            public string Name = "images";
            public string[] images;
            public long IDPicture;
            public string PicName;
            public long IDSurgery;
            public string Image;
        }

        public class ObjExport
        {
            public string TableName { get; set; }
            public string Start { get; set; }
            public string End { get; set; }
            public long Count { get; set; }
        }

        public class Constants
        {
            public DateTime StartTime = DateTime.Now;

            public string PicTable = "tblPicture";
            public string PicTable1 = "tblPicture1";
            public string PicTable2 = "tblPicture2";
            public string PicTable3 = "tblPicture3";
            public string PicTable4 = "tblPicture4";

            public string[] PicTableCnt = new string[4] { "tblPicture1", "tblPicture2", "tblPicture3", "tblPicture4" };

            public string[] TblToJson = new string[28] { "tblAutoFill", "tblComplication", "tblCpt", "tblDesc", "tblDescAuto", "tblDiagnosis",
                "tblDoctor", "tblFiscalYear", "tblHospital", "tblInstrumentation", "tblLevel", "tblLocation", "tblPatient", "tblPicture_Desc",
                "tblSex", "tblState", "tblSurgery", "tblSurgery_Complication", "tblSurgery_Cpt", "tblSurgery_Dr", "tblSurgery_Dx", "tblSurgery_Hospital",
                "tblSurgery_Instrumentation", "tblSurgery_Level", "tblSurgery_Location", "tblSurgery_Surgery", "tblSurgeryItems", "tblSurgeryType" };

            public string SystemPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            public string PicFile = "picfile.json";

            public string ConnString = "Provider=Microsoft.ACE.OLEDB.12.0;Data source=";

            //public string ConnStringCompact = "Provider=Microsoft.ACE.OLEDB.12.0; Jet OLEDB:Database Password=xaviergraymalkin;" +
            //                          "Data source=D:\\Users\\dmand\\projects\\AcccesImages\\AcccesImages\\bin\\Debug\\DatabaseUnload\\pars_compact.mdb";
            public string ConnStringCompact = "Provider=Microsoft.ACE.OLEDB.12.0; Jet OLEDB:Database Password=";
                                     //"Data source=D:\\Users\\dmand\\projects\\AcccesImages\\AcccesImages\\bin\\Debug\\DatabaseUnload\\pars_compact.mdb";
        }

        public class SubRoutine
        {
            Constants constants = new Constants();
            IniFile iniFile = new IniFile("pars.ini");

            public string DBPATH { get; private set; }
            public string DBPATHUNLOAD { get; private set; }
            public string DBNAME { get; private set; }
            public string DBCOMPACT { get; private set; }
            public string DBPWD { get; private set; }

            public void ConvertImages(PictureBox picbox, RichTextBox richTextBox, PictureBox picbox2)
            {
                OleDbCommand cmd;

                string base64Text = "";

                //  set file path
                string fullpath = @constants.SystemPath + @"\";
                fullpath = fullpath.Substring(6);

                if (File.Exists(fullpath + "image.jpg"))
                {
                    File.Delete(fullpath + "image.jpg");
                }

                //  define image file as filestream
                FileStream FS1 = new FileStream("image.jpg", FileMode.Create);

                OleDbConnection conn = new OleDbConnection
                {
                    ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                    "Data source= D:\\This PC\\Pars Info\\Rick PC\\rick laptop\\test1.mdb"
                };

                OleDbConnection conn2 = new OleDbConnection
                {
                    ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                   "Data source= D:\\This PC\\Pars Info\\Rick PC\\rick laptop\\pars_prod_2013_pic1.accdb"
                };

                try
                {
                    conn.Open();
                    conn2.Open();

                    //  get picutre dataset for each table
                    DataSet DSPic1 = DSPic(constants.PicTable1, conn2);
                    //DataSet DSPic2 = DSPic(constants.PicTable2, conn2);
                    //DataSet DSPic3 = DSPic(constants.PicTable3, conn2);
                    //DataSet DSPic4 = DSPic(constants.PicTable4, conn2);

                    //  put each picture table into Json
                    ProcessPictures(DSPic1, constants.PicTable1, constants.SystemPath, constants.PicFile);
                    
                    OleDbDataReader reader = null;
                    cmd = new OleDbCommand("select * from tblDoctor", conn);
                    reader = cmd.ExecuteReader();

                    //  read image from DB
                    //OleDbCommand cmd2 = new OleDbCommand("select * from tblPicture where IDPictures = 240", conn);
                    OleDbCommand cmd2 = new OleDbCommand("select * from tblPicture1 where IDPictures = 2875", conn2);
                    DataSet ds = new DataSet();
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd2);
                    da.Fill(ds, "tblPicture1");
                    //Console.WriteLine("IDSurgery: " + ds.Tables["tblPicture"].Rows[0]["IDSurgery"]);

                    //  write image to base64 text file
                    byte[] blob = (byte[])ds.Tables["tblPicture1"].Rows[0]["PicBlob"];
                    FS1.Write(blob, 0, blob.Length);

                    base64Text = Convert.ToBase64String(blob);
                    richTextBox.Text = base64Text;

                    //  compress file stream
                    //picbox2.Image = Base64ToImage(base64Text);
                    DefaultCompresion(picbox2.Image, fullpath, "image2_stream.jpg");
                    
                    //  write blob to image file
                    byte[] blobpic = Convert.FromBase64String(base64Text);
                    File.WriteAllBytes(fullpath + "image2.jpg", blobpic);

                    //  compress to file
                    DefaultCompresion(Image.FromFile("image2.jpg"), fullpath, "image2_compressed.jpg");

                    ////  compress byte array
                    //byte[] compressedBytes;

                    //using (var writer = new StreamWriter(new MemoryStream()))
                    //{
                    //    writer.Write(base64Text);
                    //    writer.Flush();
                    //    writer.BaseStream.Position = 0;

                    //    compressedBytes = Compress(writer.BaseStream, blobpic);

                    //    File.WriteAllBytes(fullpath + "image2_comp.jpg", compressedBytes);
                    //}


                    //FS1.Flush();
                    //FS1.Close();
                    //FS1 = null;

                    //picbox2.Image = Base64ToImage(base64Text);
                    picbox2.SizeMode = PictureBoxSizeMode.StretchImage;
                    picbox2.Refresh();

                    // Delete the file if it exists.
                    string path = @"D:\Users\dmand\projects\AcccesImages\AcccesImages\bin\Debug\VeryLargeBase64File.txt";

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    //  read file image                               
                    //picbox.Image = Image.FromStream(new MemoryStream(blob));
                    //picbox.Image = Image.FromFile("image.jpg");
                    //picbox.SizeMode = PictureBoxSizeMode.StretchImage;
                    //picbox.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to connect to data source.\n" + ex.Message, "ConvertImages");
                }
                finally
                {
                    
                }

                conn.Close();
                conn2.Close();

                //  compress image
                //DefaultCompresion(Image.FromFile("image.jpg"), fullpath, "compressed.jpg");

                FS1.Flush();
                FS1.Close();
            }

            public Image Base64ToImage(string base64String)
            {
                try
                {
                    // Convert base 64 string to byte[]
                    byte[] imageBytes = Convert.FromBase64String(base64String);

                    MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                    Image image = Image.FromStream(ms, true);
                    return image;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to write string to file.\n" + ex.Message, "Base64ToImage");
                    return null;
                }
            }

            public string ImageToBase64(string path, string filename)
            {
                using (Image image = Image.FromFile(path + filename))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        string base64String = Convert.ToBase64String(imageBytes);

                        image.Dispose();
                        return base64String;
                    }
                }
            }

            public string ImgStreamToBase64(Image img)
            {
                try
                {
                    using (Image image = img)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, image.RawFormat);
                            byte[] imageBytes = ms.ToArray();
                            string base64String = Convert.ToBase64String(imageBytes);

                            image.Dispose();
                            return base64String;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to convert stream to base64.\n" + ex.Message, "ImgStreamToBase64");
                    return null;
                }
            }

            //  create picture object with data
            public string CreateObjPic(DataSet data, string image)
            {
                DataSet ds = new DataSet();
                string picdata = null;

                ObjPicture objPicture = new ObjPicture
                {
                    //IDPicture = data.Tables["IDPictures"].ToString();
                };

                return picdata;
            }

            //  return picture dataset
            public DataSet DSPic(string TableName, OleDbConnection con)
            {
                DataSet ds = new DataSet();

                try
                {
                    //OleDbCommand cmdOleDb = new OleDbCommand("select * from " + TableName + " ", con);
                    OleDbCommand cmdOleDb = new OleDbCommand("select TOP 5 " + TableName + ".* from " + TableName, con);
                    ds = new DataSet();
                    OleDbDataAdapter da = new OleDbDataAdapter(cmdOleDb);
                    da.Fill(ds, TableName);
                    cmdOleDb.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to read from: " + TableName + "/n" +  ex);
                }
                
                return ds;
            }

            public OleDbCommand DBCommand(string TableName, OleDbConnection con)
            {
                try
                {
                    OleDbCommand cmdOleDb = new OleDbCommand("SELECT TOP 5 " + TableName + ".IDPictures, " +
                        //TableName + ".PicName, " + TableName + ".IDSurgery FROM " + TableName, con);
                        TableName + ".PicName, " + TableName + ".IDSurgery, " + TableName + ".PicBlob FROM " + TableName , con);
                        //OleDbCommand cmdOleDb = new OleDbCommand("select TOP 10 " + TableName + ".* from " + TableName, con);

                    return cmdOleDb;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to read from: " + TableName + "\n" + ex.Message, "OleDbCommand");
                    return null;
                }
            }

            public void ProcessPictures(DataSet ds, string TableName, string path, string filename)
            {
                PictureDetail pictureDetail = new PictureDetail();
                string[] PicArray = new string[ds.Tables[0].Rows.Count];

                ObjPicture objPicture = new ObjPicture();
                string fullpath = @path + @"\" + filename;
                fullpath = fullpath.Substring(6);
                long xCntr = 1;

                // delete existing
                if (File.Exists(fullpath))
                {
                    File.Delete(fullpath);
                }

                string output2 = JsonConvert.SerializeObject(ds, Formatting.Indented);
                File.AppendAllText(fullpath, output2);
            }

            public void ProcessDB(string TableName)
            {
                Constants constants = new Constants();
                SubRoutine sub = new SubRoutine();
                DataSet dsCount = new DataSet();
                DataSet dsProcess = new DataSet();
                OleDbCommand dbCmd = new OleDbCommand();
                OleDbDataAdapter dbAd = new OleDbDataAdapter();
                string base64Text = "";
                
                OleDbConnection conn2 = new OleDbConnection
                {
                    //ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                    //                   "Data source= D:\\This PC\\Pars Info\\Rick PC\\rick laptop\\pars_prod_2013_pic1.accdb"
                    ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                       "Data source=D:\\Users\\dmand\\projects\\AcccesImages\\AcccesImages\\bin\\Debug\\pars_prod_2013.accdb"
                };

                OleDbCommandBuilder builder = new OleDbCommandBuilder(dbAd);

                try
                {
                    conn2.Open();
                    dbCmd = sub.DBCommand(TableName, conn2);
                    dbAd = new OleDbDataAdapter(dbCmd);
                    dbAd.Fill(dsCount, TableName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to process dataset.\n" + ex.Message, "ProcessDB");
                }

                //  set file path
                string fullpath = @constants.SystemPath + @"\";
                fullpath = fullpath.Substring(6);

                try
                {
                    double FirstHalf = 0;
                    double SecondHalf = 0;
                    double ReadCount = 0;
                    double StartCount = 0;
                    int StartCountInt = 0;
                    int OddNumber = 0;
                    
                    //  get record count of table
                    double RowCount = dsCount.Tables[0].Rows.Count;

                    // delete existing
                    for (int xInt = 1; xInt < 3; xInt++)
                    {
                        if (File.Exists(fullpath + @constants.PicTable + xInt + ".json"))
                        {
                            File.Delete(fullpath + @constants.PicTable + xInt + ".json");
                        }
                    }

                    //  set variables for even or odd record count
                    if (RowCount % 2 == 0)
                    {
                        FirstHalf = RowCount / 2;
                        SecondHalf = RowCount / 2;
                        ReadCount = FirstHalf;
                    }
                    else
                    {
                        FirstHalf = Math.Truncate(RowCount / 2);
                        SecondHalf = Math.Round(RowCount / 2);

                        if (FirstHalf + SecondHalf != RowCount)
                        {
                            SecondHalf = SecondHalf + 1;
                        }

                        ReadCount = FirstHalf;
                        OddNumber = -1;
                    }

                    DataSet ds = new DataSet();
                    //dbAd.Fill(ds, (int)StartCount, (int)ReadCount, @constants.PicTable + xInt);
                    dbAd.Fill(ds, (int)StartCount, (int)ReadCount, TableName);

                    //  loop through tables
                    //for (int xInt = 1; xInt < 3; xInt++)
                    //for (int xInt = 1; xInt < dsCount.Tables[0].Rows.Count; xInt++)
                    for (int xInt = 1; xInt < 3; xInt++)
                        {
                        //DataSet ds = new DataSet();
                        //dbAd.Fill(ds, (int)StartCount, (int)ReadCount, @constants.PicTable + xInt);

                        //for (int yInt = 0; yInt < ds.Tables[0].Rows.Count; yInt++)
                        for (StartCount = 0; StartCount < ReadCount; StartCount++)

                            StartCountInt = (int)StartCount;
                            {
                            //  delete image
                            if (File.Exists(fullpath + "image_temp.jpg"))
                            {
                                File.Delete(fullpath + "image_temp.jpg");
                            }

                            if (File.Exists(fullpath + "image_temp_compressed.jpg"))
                            {
                                File.Delete(fullpath + "image_temp_compressed.jpg");
                            }

                            //  save image to file
                            byte[] blobpic = (byte[])ds.Tables[0].Rows[StartCountInt]["PicBlob"];
                            File.WriteAllBytes(fullpath + "image_temp.jpg", blobpic);

                            //  compress image
                            base64Text = Convert.ToBase64String(blobpic);
                            Image img = Base64ToImage(base64Text);

                            //  get image
                            Image imgFromFile = Image.FromFile(fullpath + "image_temp.jpg");
                            DefaultCompresion(imgFromFile, fullpath, "image_temp_compressed.jpg");
                            imgFromFile.Dispose();      //  release file resources
                            
                            //  read compressed image in as base64
                            string ImageAsBase64 = ImageToBase64(fullpath, "image_temp_compressed.jpg");


                            //  update ds image filed with new compressed base64
                            DataTable picTable = ds.Tables[0];
                            OleDbCommand cmdUpdate = new OleDbCommand
                            {
                                Connection = conn2,
                                //CommandText = "UPDATE tblPicture1 SET PicName = 'me' WHERE IDPictures = " + ds.Tables[0].Rows[yInt]["IDPictures"]
                                CommandText = "UPDATE " + TableName + " SET PicBlob = '" + ImageAsBase64 + "' WHERE IDPictures = " + ds.Tables[0].Rows[StartCountInt]["IDPictures"]
                            };
                            cmdUpdate.ExecuteNonQuery();

                            ////  serialize json
                            //string output2 = JsonConvert.SerializeObject(ds, Formatting.Indented);

                            ////  write to file
                            //File.AppendAllText(fullpath + TableName + "_" + xInt + ".json", output2);

                            ////  set second half
                            //ReadCount = SecondHalf;
                            //StartCount = SecondHalf + OddNumber;

                            //  images to base64
                            //sub.ProcessPictures(ds, TableName, constants.SystemPath, constants.PicFile);

                            ds.Dispose();
                        }

                        //  serialize json
                        string output2 = JsonConvert.SerializeObject(ds, Formatting.Indented);

                        //  write to file
                        File.AppendAllText(fullpath + TableName + "_" + xInt + ".json", output2);


                        ////  set second half
                        ReadCount = SecondHalf;
                        StartCount = SecondHalf + OddNumber;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to process dataset.\n" + ex.Message);
                }
            }

            public DataSet DSTop(string TableName, OleDbConnection con, long RecCount)
            {
                DataSet ds = new DataSet();
                DataSet dsFive = new DataSet();

                try
                {
                    OleDbCommand cmdOleDb = new OleDbCommand("select TOP " + 5 + " " + TableName + ".* from " + TableName, con);
                    //OleDbCommand cmdOleDb = new OleDbCommand("select TOP " + RecCount + " tblPicture1.IDPictures, tblPicture1.PicName, tblPicture1.IDSurgery from " + TableName, con);
                    //OleDbCommand cmdOleDb = new OleDbCommand("select TOP 20 PERCENT " + TableName + ".* from " + TableName, con);
                    ds = new DataSet();
                    OleDbDataAdapter da = new OleDbDataAdapter(cmdOleDb);
                    //da.Fill(ds, TableName);

                    //  return 5             
                    da.Fill(ds, 0, 5, TableName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to get records.\n" + ex.Message, "DSTop");
                }

                return ds;
            }

            public void ProcessPicTables(PictureBox picBox, Label lblCount, Label lblTotal, Label lblElapsedTime, Label lblLoading, Label lblTable)
            {
                //  if compacting database do not export
                try
                {
                    if (DBCompacting() == true)
                    {
                        lblTable.Text = "Database is compacting now.  Can not export data.";
                        lblLoading.Text = "";
                        lblLoading.Refresh();
                        lblTable.Refresh();
                        return;
                    }
                }
                catch
                {

                }

                Image imgFromFile = null;
                bool skip = false;
                long RecCount = 0;

                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                DateTime dateTimeUtc = DateTime.UtcNow;
                long dateTimeUnix = ((DateTimeOffset)dateTimeUtc).ToUnixTimeSeconds();
                dtDateTime = dtDateTime.AddSeconds(dateTimeUnix).ToLocalTime();

                string fullDate = dateTimeUnix.ToString();

                //  set file path
                string fullpath = @constants.SystemPath + @"\";
                fullpath = fullpath.Substring(6);


                //  "fix" last line before appending
                if (File.Exists(fullpath + "export_history.json"))
                {
                    var tempFile = Path.GetTempFileName();
                    var linesToKeep = File.ReadLines(fullpath + "export_history.json").Where(l => l != "]}");

                    File.WriteAllLines(tempFile, linesToKeep);
                    File.Delete(fullpath + "export_history.json");
                    File.Move(tempFile, fullpath + "export_history.json");
                    File.AppendAllText(fullpath + "export_history.json", "],\n");
                    File.AppendAllText(fullpath + "export_history.json", "\"" + fullDate + "\":[\n");
                }
                else
                {
                    File.AppendAllText(fullpath + "export_history.json", "{ \"" + fullDate + "\":[\n");
                }

                //  loop through all picture databases
                for (int y = 3; y > -1; y--)
                {
                    string dateStart = DateTime.Now.ToShortDateString() + " " +
                                       DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);

                    //  set json file
                    DeleteFile(fullpath, constants.PicTableCnt[y] + ".json");
                    File.AppendAllText(fullpath + constants.PicTableCnt[y] + ".json", "[\n");

                    //  set database connections
                    DataSet dspic = DSRecords(constants.PicTableCnt[y]);
                    lblLoading.Text = "Elapsed:";
                    lblLoading.Refresh();

                    if (dspic == null)
                    {
                        return;
                    }

                    lblTotal.Text = dspic.Tables[0].Rows.Count.ToString();
                    lblTable.Text = "Table: " + constants.PicTableCnt[y];
                    lblTable.Refresh();
                    lblTotal.Refresh();
                    RecCount = dspic.Tables[0].Rows.Count;

                    //  loop through table
                    for (int x = 0; x < dspic.Tables[0].Rows.Count; x++)
                    {
                        //  update time every x records
                        if (x % 10 == 0)
                        {
                            UpdateTime(constants.StartTime, lblElapsedTime);
                        }

                        lblCount.Text = (x + 1).ToString();
                        lblCount.Refresh();

                        //  delete files
                        DeleteFile(fullpath, "image_temp.jpg");
                        DeleteFile(fullpath, "image_temp_compressed.jpg");
                        DeleteFile(fullpath, "base64_un.txt");
                        DeleteFile(fullpath, "base64_comp.txt");

                        picBox.Image = null;
                        picBox.Refresh();

                        //  check for null image
                        if ((dspic.Tables[0].Rows[x]["PicBlob"].ToString() == null) || (dspic.Tables[0].Rows[x]["PicBlob"].ToString() == ""))
                        {
                            WriteBlank(dspic, x, null, fullpath);
                            skip = false;
                            continue;
                        }

                        //  read from database, save uncompressed image to file
                        byte[] blobpic = (byte[])dspic.Tables[0].Rows[x]["PicBlob"];

                        //  convert uncompressed array to base64, write to text file
                        string base64Text = Convert.ToBase64String(blobpic);
                        File.AppendAllText(fullpath + "base64_un.txt", base64Text);

                        //  write uncompressd to image file
                        File.WriteAllBytes(fullpath + "image_temp.jpg", blobpic);

                        //  convert to base64 to allow writting to file
                        base64Text = Convert.ToBase64String(blobpic);
                        Image img = Base64ToImage(base64Text);
                        picBox.Image = img;
                        picBox.Refresh();
                        img.Dispose();

                        //  read uncompressed image from file
                        try
                        {
                            imgFromFile = Image.FromFile(fullpath + "image_temp.jpg");
                        }
                        catch (Exception ex)
                        {
                            picBox.Image = null;
                            picBox.Refresh();
                            WriteErr(dspic, x, base64Text, fullpath);

                            MessageBox.Show("Failed to read uncompressed image from file.\n" + ex.Message, "ProcessPicTables");
                            skip = false;
                            continue;
                        }

                        //  compress image
                        bool isCompressed = DefaultCompresion(imgFromFile, fullpath, "image_temp_compressed.jpg");
                        imgFromFile.Dispose();      //  release file resources

                        if (isCompressed == false)
                        {
                            WriteErr(dspic, x, base64Text, fullpath);
                            skip = false;
                            return;
                        }

                        //  read compressed image in as base64
                        string ImageAsBase64 = ImageToBase64(fullpath, "image_temp_compressed.jpg");

                        //  write compressed base64 string to text file
                        File.AppendAllText(fullpath + "base64_comp.txt", ImageAsBase64);

                        //  crete new object
                        PictureDetail picDetail = new PictureDetail
                        {
                            IDPicture = (int)dspic.Tables[0].Rows[x]["IDPictures"],
                            PicName = dspic.Tables[0].Rows[x]["PicName"].ToString()
                        };
                        var idsurgeryval = dspic.Tables[0].Rows[x]["IDSurgery"];

                        if (idsurgeryval.ToString() != "")
                        {
                            picDetail.IDSurgery = (int?)dspic.Tables[0].Rows[x]["IDSurgery"];
                        }
                        else
                        {
                            picDetail.IDSurgery = null;
                        }

                        //  TODO - put blob back in json file
                        //picDetal.Image = ImageAsBase64;
                        picDetail.Image = null;

                        //  object to file
                        File.AppendAllText(fullpath + constants.PicTableCnt[y] + ".json", JsonConvert.SerializeObject(picDetail));
                    }    
                    //  end loop through table

                    //  close recordset
                    dspic.Clear();
                    dspic.Dispose();


                    string dateEnd = DateTime.Now.ToShortDateString() + " " +
                                     DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);

                    //  create export file
                    ObjExport objExport = new ObjExport
                    {
                        TableName = constants.PicTableCnt[y],
                        Start = dateStart,
                        End = dateEnd,
                        Count = RecCount
                    };

                    string exportData = JsonConvert.SerializeObject(objExport, Formatting.Indented);

                    //  write to file
                    if (y != -1)
                    {
                        if (skip == true)
                        {
                            File.AppendAllText(fullpath + "export_history.json", ",\n");
                        }
                        else
                        {
                            skip = true;
                        }
                    }
                   
                    File.AppendAllText(fullpath + "export_history.json", exportData);
                }
                //  end loop through all picture databases

                //  clear labels
                lblTable.Text = "Table: ";
                lblTotal.Text = "";
                lblCount.Text = "";
                lblTable.Refresh();
                lblTotal.Refresh();
                lblCount.Refresh();

                File.AppendAllText(fullpath + "export_history.json", "\n]}");
            }

            public void ExportTablesToJson(Label lblLoading, Label lblTable)
            {
                //  export data tables to json files
                ExportTablesToJsonDS(constants.TblToJson, lblLoading, lblTable);
            }

            public void ExportTablesToJsonDS(string[] TableName, Label lblLoading, Label lblTable)
            {
                //  if compacting database do not export
                try
                {
                    if(DBCompacting() ==true)
                    {
                        lblTable.Text = "Database is compacting now.  Can not export data.";
                        lblLoading.Text = "";
                        lblLoading.Refresh();
                        lblTable.Refresh();
                        return;
                    }
                }
                catch
                {

                }

                try
                {
                    bool skip = true;
                    long RecordCount = 0;

                    //  set file path
                    string fullpath = @constants.SystemPath + @"\";
                    fullpath = fullpath.Substring(6);

                    //  connect to database
                    OleDbDataAdapter dbAd = new OleDbDataAdapter();
                    OleDbConnection con = new OleDbConnection(constants.ConnString + DBPATH + "\\" + DBNAME);
                    con.Open();

                    DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    DateTime dateTimeUtc = DateTime.UtcNow;
                    long dateTimeUnix = ((DateTimeOffset)dateTimeUtc).ToUnixTimeSeconds();
                    dtDateTime = dtDateTime.AddSeconds(dateTimeUnix).ToLocalTime();

                    string fullDate = dateTimeUnix.ToString();

                    //  "fix" last line before appending
                    if (File.Exists(fullpath + "export_history.json"))
                    {
                        var tempFile = Path.GetTempFileName();
                        var linesToKeep = File.ReadLines(fullpath + "export_history.json").Where(l => l != "]}");

                        File.WriteAllLines(tempFile, linesToKeep);
                        File.Delete(fullpath + "export_history.json");
                        File.Move(tempFile, fullpath + "export_history.json");
                        File.AppendAllText(fullpath + "export_history.json", "],\n");
                        File.AppendAllText(fullpath + "export_history.json", "\"" + fullDate + "\":[\n");
                    }
                    else
                    {
                        File.AppendAllText(fullpath + "export_history.json", "{ \"" + fullDate + "\":[\n");
                    }
                    
                    //  get tables
                    for (int x = 0; x < TableName.Length ; x++)
                    {
                        string dateStart = DateTime.Now.ToShortDateString() + " " +
                                           DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                       
                        //  load table to dataset
                        DataSet ds = new DataSet();
                        OleDbCommand dbCmd = new OleDbCommand("SELECT * FROM " + TableName[x], con);

                        dbAd = new OleDbDataAdapter(dbCmd);
                        dbAd.Fill(ds, TableName[x]);
                        Console.WriteLine("Record Count: " + ds.Tables[0].Rows.Count + " " + ds.Tables[0].TableName.ToString());
                        lblTable.Text = "Record Count: " + ds.Tables[0].Rows.Count + " " + ds.Tables[0].TableName.ToString();
                        lblTable.Refresh();
                        RecordCount = ds.Tables[0].Rows.Count;

                        string json = JsonConvert.SerializeObject(ds, Formatting.Indented);

                        //  delete existing
                        if (File.Exists(fullpath + TableName[x] + ".json"))
                        {
                            File.Delete(fullpath + TableName[x] + ".json");
                        }

                        //  write to file
                        File.AppendAllText(fullpath + TableName[x] + ".json", json);

                        string dateEnd = DateTime.Now.ToShortDateString() + " " +
                                         DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);

                        //  create export file
                        ObjExport objExport = new ObjExport
                        {
                            TableName = TableName[x],
                            Start = dateStart,
                            End = dateEnd,
                            Count = ds.Tables[0].Rows.Count
                        };

                        string exportData = JsonConvert.SerializeObject(objExport, Formatting.Indented);

                        //  write to file
                        if (x != 0)
                        {
                            if (skip == true)
                            {
                                File.AppendAllText(fullpath + "export_history.json", ",\n");
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                        File.AppendAllText(fullpath + "export_history.json", exportData);
                    }
                    //  close json file
                    File.AppendAllText(fullpath + "export_history.json", "\n]}");

                    //  close database connection
                    dbAd.Dispose();
                    con.Close();
                    con.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to open database.\n" + ex.Message, "ExportTablesToJsonDS");
                }
            }

            public bool DefaultCompresion(Image original, string ImagePath, string ImageName)
            {
                try
                {
                    MemoryStream ms = new MemoryStream();

                    original.Save(ms, ImageFormat.Png);
                    //original.Save(ms, ImageFormat.Jpeg);  // jpeg image format does not compress
                    Bitmap compressed = new Bitmap(ms);
                    string fileOutPng = Path.Combine(ImagePath, ImageName);
                    compressed.Save(fileOutPng, ImageFormat.Jpeg);

                    ms.Flush();
                    ms.Close();
                    compressed.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not compress image.\n" + ex.Message, "DefaultCompresion");
                    return false;
                }
            }

            public string StreamToBase64(Stream stream)
            {
                //byte[] bytes;
                //using (MemoryStream memoryStream = new MemoryStream())
                //{
                //    stream.CopyTo(memoryStream);
                //    bytes = memoryStream.ToArray();
                //}

                //string base64 = Convert.ToBase64String(bytes);
                //return base64;
                //return new MemoryStream(Encoding.UTF8.GetBytes(base64));

                byte[] bytes;

                try
                {
                    MemoryStream memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                    string base64 = Convert.ToBase64String(bytes);
                    return base64;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to write stream to string.\n" + ex.Message, "StreamToBase64");
                    return null;
                }
            }

            public DataSet DSRecords(string TableName)
            {
                try
                {
                    OleDbDataAdapter dbAd = new OleDbDataAdapter();
                    DataSet ds = new DataSet();
                    OleDbConnection con = new OleDbConnection(constants.ConnString + DBPATH + "\\" + DBNAME);
                    con.Open();
                   
                    OleDbCommand dbCmd = new OleDbCommand("SELECT " + TableName + ".IDPictures, " +
                        TableName + ".PicName, " +
                        TableName + ".IDSurgery, " +
                        TableName + ".PicBlob " +
                        "FROM " + TableName +
                        " ORDER BY " + TableName + ".IDPictures", con);

                    //OleDbCommand dbCmd = new OleDbCommand("SELECT TOP 40 " + TableName + ".IDPictures, " +
                    //TableName + ".PicName, " + TableName + ".IDSurgery FROM " + TableName, con);
                    //TableName + ".PicName, " + TableName + ".IDSurgery, " + TableName + ".PicBlob FROM " + TableName, con);
                    //OleDbCommand cmdOleDb = new OleDbCommand("select TOP 10 " + TableName + ".* from " + TableName, con);
                    //OleDbCommand dbCmd = new OleDbCommand("select tblPicture1.* from tblPicture1 where tblPicture1.IDPictures = 287 ", con);

                    dbAd = new OleDbDataAdapter(dbCmd);
                    dbAd.Fill(ds, TableName);
                    Console.WriteLine("Record Count: " + ds.Tables[0].Rows.Count);
                    return ds;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to open database.\n" + ex.Message, "DSRecords");
                    return null;
                }
            }

            public bool DBCompacting()
            {
                DataSet ds = new DataSet();

                try
                {
                    OleDbDataAdapter dbAd = new OleDbDataAdapter();
                    OleDbConnection con = new OleDbConnection(constants.ConnStringCompact + DBPWD + ";Data source=" + DBPATHUNLOAD + "\\" + DBCOMPACT);
                    con.Open();

                    OleDbCommand dbCmd = new OleDbCommand("SELECT tblCompact.CompactDB FROM tblCompact", con);

                    dbAd = new OleDbDataAdapter(dbCmd);
                    dbAd.Fill(ds, "tblCompact");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to open database.\n" + ex.Message, "DBCompacting");
                    return true;
                }

                if ((bool)ds.Tables[0].Rows[0]["CompactDB"] == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public Image ByteArrayToImage(byte[] byteArrayIn)
            {
                try
                {
                    MemoryStream ms = new MemoryStream(byteArrayIn);
                    Image returnImage = Image.FromStream(ms);
                    return returnImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to write byte array to image.\n" + ex.Message, "ByteArrayToImage");
                    return null;
                }
            }
            
            public byte[] ReadImageFileToArray(string path, string filename)
            {
                try
                {
                    // Load file meta data with FileInfo
                    FileInfo fileInfo = new FileInfo(path + filename);

                    // The byte[] to save the data in
                    byte[] data = new byte[fileInfo.Length];

                    // Load a filestream and put its content into the byte[]
                    using (FileStream fs = fileInfo.OpenRead())
                    {
                        fs.Read(data, 0, data.Length);
                    }

                    return data;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to write image to array.\n" + ex.Message, "ReadImageFileToArray");
                    return null;
                }
            }

            public byte[] ImageToByteArray(Image imageIn)
            {
                try
                {
                    MemoryStream ms = new MemoryStream();
                    imageIn.Save(ms, ImageFormat.Gif);
                    return ms.ToArray();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to write image to array.\n" + ex.Message, "ImageToByteArray");
                    return null;
                }
            }

            public bool WriteArrayToFile(string FileName, byte[] Data, string path)
            {
                BinaryWriter Writer = null;
               
                try
                {
                    // Create a new stream to write to the file
                    Writer = new BinaryWriter(File.OpenWrite(FileName));

                    // Writer raw data                
                    Writer.Write(Data);
                    Writer.Flush();
                    Writer.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to write image to file.\n" + ex.Message, "WriteArrayToFile");
                    return false;
                }

                return true;
            }

            public void DeleteFile(string path, string filename)
            {
                try
                {
                    if (File.Exists(path + filename))
                    {
                        File.Delete(path + filename);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to delete file.\n" + ex.Message, "DeleteFile");
                }
            }

            public async void WriteToFile(string filestring, string filename, string path)
            {
                try
                {
                    using (StreamWriter writer = File.CreateText(path + filename))
                    {
                        await writer.WriteAsync(filestring);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to write string to file.\n" + ex.Message, "WriteToFile");
                }
            }

            public void WriteBlank(DataSet ds, int x, string ImageAsBase64, string fullpath)
            {
                var idsurgeryval = ds.Tables[0].Rows[x]["IDSurgery"];

                DateTime startTime = DateTime.Now;
                File.AppendAllText(fullpath + "picture_blank.json", startTime.ToShortDateString() + " " + startTime.ToShortTimeString() + ", ");
               
                //  blank image
                PictureBlank picBlank = new PictureBlank
                {
                    IDPicture = (int)ds.Tables[0].Rows[x]["IDPictures"],
                    PicName = ds.Tables[0].Rows[x]["PicName"].ToString()
                };

                if (idsurgeryval.ToString() != "")
                {
                    picBlank.IDSurgery = (int?)ds.Tables[0].Rows[x]["IDSurgery"];
                }
                else
                {
                    picBlank.IDSurgery = null;
                }

                //  blank image string to file
                if ((ImageAsBase64 == null) || (ImageAsBase64 == ""))
                {
                    File.AppendAllText(fullpath + 
                        "picture_blank.json",
                        picBlank.IDPicture.ToString() + ", " +
                        picBlank.PicName.ToString() + ", " +
                        picBlank.IDSurgery.ToString() + "\n");
                }
            }

            public void WriteErr(DataSet ds, int x, string ImageAsBase64, string fullpath)
            {
                var idsurgeryval = ds.Tables[0].Rows[x]["IDSurgery"];

                DateTime startTime = DateTime.Now;
                File.AppendAllText(fullpath + "picture_err.json", startTime.ToShortDateString() + " " + startTime.ToShortTimeString() + ", ");

                PictureErr picErr = new PictureErr
                {
                    IDPicture = (int)ds.Tables[0].Rows[x]["IDPictures"],
                    PicName = ds.Tables[0].Rows[x]["PicName"].ToString(),
                    ImageString = ImageAsBase64
                };

                if (idsurgeryval.ToString() != "")
                {
                    picErr.IDSurgery = (int?)ds.Tables[0].Rows[x]["IDSurgery"];
                }
                else
                {
                    picErr.IDSurgery = null;
                }

                //  write to file
                File.AppendAllText(fullpath + "picture_err.json", 
                    picErr.IDPicture + ", " + 
                    picErr.PicName + ", " +
                    picErr.IDSurgery + "\n");
            }

            public void UpdateTime(DateTime start, Label ElapsedTime)
            {
                try
                {
                    DateTime endTime = DateTime.Now;
                    TimeSpan diff = start - endTime;
                    ElapsedTime.Text = diff.ToString(@"hh\:mm\:ss");
                    ElapsedTime.Refresh();
                }
                catch (Exception ex)
                {

                }
            }

            public void ReadIni()
            {
                DBPATH = iniFile.Read("ProductionPath", "Database");
                DBPATHUNLOAD = iniFile.Read("DatabaseUnload", "Database");
                DBNAME = iniFile.Read("ProductionName", "Database");
                DBCOMPACT = iniFile.Read("DatabaseCompactName", "Database");
                DBPWD = iniFile.Read("DatabasePwd", "Database");

                //  todo decrypt password
                DBPWD = "xaviergraymalkin";
            }
        }
    }
}
