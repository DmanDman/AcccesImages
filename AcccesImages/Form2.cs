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

namespace AccesImages
{
    public partial class Form2 : Form
    {
        SubRoutine SubRtn = new SubRoutine();
        Constants consts = new Constants();

        public Form2()
        {
            InitializeComponent();
            LblCount.Text = "0";
            LblTotal.Text = "0";
            LblTotalTime.Text = "";
            LblStartTime.Text = "";
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            DateTime startTime = DateTime.Now;
            LblStartTime.Text = startTime.ToShortTimeString();
            LblStartTime.Refresh();

            BtnConvert.Enabled = false;

            //SubRtn.ConvertImages(PictureBox1, RichTextBox1, PictureBox2);

            SubRtn.ProcessPicTables(consts.PicTable1, PicOrig, LblCount, LblTotal);

            BtnConvert.Enabled = true;

            DateTime endTime = DateTime.Now;
            TimeSpan diff = startTime - endTime;
            LblTotalTime.Text = diff.ToString(@"hh\:mm\:ss");
            LblTotalTime.Refresh();

            //for (int x = 1; x < 5; x++)
            //{
            //    SubRtn.ProcessDB(constants.PicTable + x);
            //}
        }

        public class Doctor
        {
            public string FirstName;
            public string LastName;
            public string Middle;
            public string DrPic;
        }

        public class RootPicture
        {
            //public string Name = "images";
            //public PictureDetail[] PicDetail; //{ get; set; }
            public PictureDetail[] PicDetail;
            //public string[] PicData;
        }

        public class PicData
        {
            public string ImageData; //{ get; set; }
        }

        public class PictureDetail
        {
            public int IDPicture { get; set; }
            public string PicName { get; set; }
            public int? IDSurgery { get; set; }
            public string Image { get; set; }
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

        public class Constants
        {
            public string PicTable = "tblPicture";
            public string PicTable1 = "tblPicture1";
            public string PicTable2 = "tblPicture2";
            public string PicTable3 = "tblPicture3";
            public string PicTable4 = "tblPicture4";

            public string SystemPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            public string PicFile = "picfile.json";

            public string ConnString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                       "Data source=D:\\Users\\dmand\\projects\\AcccesImages\\AcccesImages\\bin\\Debug\\pars_prod_2013.accdb";
        }

        public class SubRoutine
        {
            Constants constants = new Constants();

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

                    while (reader.Read())
                    {
                        Doctor dr = Create(reader);
                        //Console.WriteLine("Dr Last Name: " + dr.LastName + ", " + dr.LastName + ", " + dr.Middle);

                        string drdata = CreateDr(reader, base64Text);
                        //Console.WriteLine("JSON: " + drdata);
                    }

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

            public Doctor Create(OleDbDataReader data)
            {
                Doctor doctor = new Doctor
                {
                    LastName = data["LastName"].ToString(),
                    FirstName = data["FirstName"].ToString(),
                    Middle = data["MiddleInitial"].ToString()
                };

                return doctor;
            }

            //  create Doctor object with data
            public string CreateDr(OleDbDataReader data, string image)
            {
                Doctor Doctor = new Doctor
                {
                    LastName = data["LastName"].ToString(),
                    FirstName = data["FirstName"].ToString(),
                    Middle = data["MiddleInitial"].ToString(),
                    DrPic = image
                };

                string output = JsonConvert.SerializeObject(Doctor);
                return output;
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
                    MessageBox.Show("Failed to read from: " + TableName + "/n" + ex);
                }

                return ds;
            }

            public OleDbCommand DBCommand(string TableName, OleDbConnection con)
            {
                try
                {
                    OleDbCommand cmdOleDb = new OleDbCommand("SELECT TOP 5 " + TableName + ".IDPictures, " +
                        //TableName + ".PicName, " + TableName + ".IDSurgery FROM " + TableName, con);
                        TableName + ".PicName, " + TableName + ".IDSurgery, " + TableName + ".PicBlob FROM " + TableName, con);
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
                RootPicture rootPicture = new RootPicture();
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

            public void ProcessPicTables(string TableName, PictureBox picBox, Label lblCount, Label lblTotal)
            {
                Image imgFromFile = null;
                bool skip = true;

                //  set file path
                string fullpath = @constants.SystemPath + @"\";
                fullpath = fullpath.Substring(6);

                //  set json file
                DeleteFile(fullpath, "picture_js.json");
                File.AppendAllText(fullpath + "picture_js.json", "[\n");

                //  set database connections
                DataSet ds = DSRecords(TableName);

                if (ds == null)
                {
                    return;
                }

                lblTotal.Text = ds.Tables[0].Rows.Count.ToString();

                //  loop through table
                for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                {
                    if (x != 0)
                    {
                        if (skip == true)
                        {
                            File.AppendAllText(fullpath + "picture_js.json", ", ");

                        }
                        else
                        {
                            skip = true;
                        }
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

                    //  read from database, save uncompressed image to file
                    byte[] blobpic = (byte[])ds.Tables[0].Rows[x]["PicBlob"];

                    //  if invalid image, continue
                    if (blobpic.Length == 0)
                    {
                        skip = false;
                        continue;
                    }

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

                    //  read uncompressed image from file
                    try
                    {
                        imgFromFile = Image.FromFile(fullpath + "image_temp.jpg");
                    }
                    catch (Exception ex)
                    {
                        picBox.Image = null;
                        picBox.Refresh();
                        MessageBox.Show("Failed to read uncompressed image from file.\n" + ex.Message, "ProcessPicTables");
                        skip = false;
                        continue;
                    }

                    //  compress image
                    bool isCompressed = DefaultCompresion(imgFromFile, fullpath, "image_temp_compressed.jpg");
                    imgFromFile.Dispose();      //  release file resources

                    if (isCompressed == false)
                    {
                        skip = false;
                        return;
                    }

                    //  read compressed image in as base64
                    string ImageAsBase64 = ImageToBase64(fullpath, "image_temp_compressed.jpg");

                    //  write compressed base64 string to text file
                    File.AppendAllText(fullpath + "base64_comp.txt", ImageAsBase64);

                    //  crete new object
                    PictureDetail picDetal = new PictureDetail
                    {
                        IDPicture = (int)ds.Tables[0].Rows[x]["IDPictures"],
                        PicName = ds.Tables[0].Rows[x]["PicName"].ToString()
                    };
                    var idsurgeryval = ds.Tables[0].Rows[x]["IDSurgery"];

                    if (idsurgeryval.ToString() != "")
                    {
                        picDetal.IDSurgery = (int?)ds.Tables[0].Rows[x]["IDSurgery"];
                    }
                    else
                    {
                        picDetal.IDSurgery = null;
                    }

                    //  TODO - put blob back in json file
                    //picDetal.Image = ImageAsBase64;
                    picDetal.Image = null;

                    //  object to file
                    File.AppendAllText(fullpath + "picture_js.json", JsonConvert.SerializeObject(picDetal));
                    //File.AppendAllText(fullpath + "picture_js.json", ", ");

                    #region Tried This - Delete
                    ////  write array to image
                    //Image imgByte = ByteArrayToImage(blobpic);

                    //if (imgByte == null)
                    //{
                    //    continue;
                    //}

                    ////  compress/save image
                    //MemoryStream ms = new MemoryStream();
                    //MemoryStream ms2 = new MemoryStream();
                    //Image imgConvert = Base64ToImage(base64Text);

                    //if (imgConvert == null)
                    //{
                    //    continue;
                    //}

                    //try
                    //{
                    //    imgConvert.Save(ms2, imgConvert.RawFormat);
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show("Failed to save image to stream ms2: " + ex, "ProcessPicTables");
                    //    continue;
                    //}

                    //try
                    //{
                    //    imgConvert.Save(ms2, ImageFormat.Png);
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show("Failed to save image to stream ms: " + ex, "ProcessPicTables");
                    //    continue;
                    //}
                    #endregion Tried This

                    #region Current Stuff
                    //try
                    //{
                    //    Image imgBase = Base64ToImage(base64Text);

                    //    if (imgBase == null)
                    //    {
                    //        continue;
                    //    }

                    //    MemoryStream msStream = new MemoryStream();
                    //    imgBase.Save(msStream, ImageFormat.Png);
                    //    string imgBaseString = ImgStreamToBase64(imgBase);
                    //    File.AppendAllText(fullpath + "base64_comp.txt", imgBaseString);
                    //    msStream.Close();
                    //    msStream.Dispose();
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show("Failed to write compressed file: " + ex, "ProcessPicTables");
                    //}

                    //try
                    //{
                    //    //ms.Close();
                    //    //ms.Dispose();
                    //    //ms2.Close();
                    //    //ms2.Dispose();
                    //}
                    //catch
                    //{

                    //}
                    #endregion

                    #region Comment this out
                    //////  byte array to image
                    ////Image imgcomp = ByteArrayToImage(blobpic);

                    //////  compress image array
                    ////MemoryStream ms =  CompressStream(imgcomp);

                    //////  memory stream to base64
                    ////string compbase64 = StreamToBase64(ms);
                    ////File.AppendAllText(fullpath + "base64_comp.txt", compbase64);


                    //Image ImageFromString = Base64ToImage(base64Text);

                    ////  save blob to file
                    //Boolean WriteResult = WriteArrayToFile("image_temp.jpg", blobpic, fullpath);

                    //if (WriteResult == false)
                    //{
                    //    return;
                    //}

                    ////  read non-compressed image
                    //byte[] ImageNotCompressed = ReadImageFileToArray(fullpath, "image_temp.jpg");

                    //if (ImageNotCompressed == null)
                    //{
                    //    return;
                    //}

                    ////  convert byte[] to image
                    ////Image ImgFromByte = ByteArrayToImage(blobpic);
                    //Image ImgFromByte = ByteArrayToImage(ImageNotCompressed);

                    //picBox.Image = ImgFromByte;
                    //picBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    //picBox.Refresh();

                    //if (ImgFromByte == null )
                    //{
                    //    return;
                    //}

                    ////  convert array to base64string
                    //base64Text = Convert.ToBase64String(blobpic);

                    ////  compress image
                    //DefaultCompresion(ImgFromByte, fullpath, "image_temp_compressed.jpg");
                    #endregion
                }

                //  close json file
                File.AppendAllText(fullpath + "picture_js.json", "]\n");
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
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not compress image.\n" + ex.Message, "DefaultCompresion");
                    return false;
                }
            }

            public MemoryStream CompressStream(Image img)
            {
                //  "long quality" and "ImageCodecInfo codec" were last 2 parameters
                //EncoderParameters parameters = new EncoderParameters(1);
                //parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

                //var ms = new MemoryStream();
                //img.Save(ms, codec, parameters);
                //return ms;

                try
                {
                    MemoryStream memstream = new MemoryStream();
                    img.Save(memstream, ImageFormat.Png);
                    return memstream;
                    //using (MemoryStream memstream = new MemoryStream())
                    //{
                    //    img.Save(memstream, ImageFormat.Png);
                    //    return memstream;
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to compress memory stream.\n" + ex.Message, "CompressStream");
                    return null;
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
                    OleDbConnection con = new OleDbConnection(constants.ConnString);
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
        }
    }
}
