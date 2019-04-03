using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using MySql.Data.MySqlClient;

namespace lab01
{
    class Program
    {
        /// <summary>
        /// read the rows of input txt file
        /// </summary>
        /// <param name="path"><path of the file/param>
        /// <returns><lines of the txt file/returns>
        public static int IntFileLines(string path)
        {
            int lines = 0;  // lines of txt file
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamReader sr = new StreamReader(fs);
            while (sr.ReadLine() != null)
            {
                lines++;
            }
            fs.Close();
            sr.Close();

            return lines;
        }

        /// <summary>
        /// read each line of input txt and pass the data to stringData[] by reference
        /// </summary>
        /// <param name="path"><path of the file/param>
        /// <param name="stringData"><string arrays used for storeage/param>
        /// <param name="rows"><lines of the txt file/param>
        public static void ReadFileByLines(string path, ref string[] stringData, int rows)
        {
            StreamReader sr = new StreamReader(path);
            for (int i = 0; i < rows; i++)
            {
                stringData[i] = sr.ReadLine();
            }
        }

        /// <summary>
        /// find whether the input character are valid (numbers)
        /// </summary>
        /// <param name="s"><the string to be checked/param>
        /// <returns></returns>
        public static bool CorrectInput(string s)
        {
            char[] charArray = s.ToCharArray();
            bool correctInput = true;
            for (int i = 0; i < s.Length; i++)
            {
                if (charArray[i] <= '9' && charArray[i] >= '0')
                {
                    
                }
                else
                {
                    correctInput = false;
                }
            }
            return correctInput;
        }

        /// <summary>
        /// print qrcode in the console
        /// </summary>
        /// <param name="qrCode"><qrcode/param>
        public static void ConsoleOutput(QrCode qrCode)
        {
            for (int j = 0; j < qrCode.Matrix.Width; j++)
            {
                for (int i = 0; i < qrCode.Matrix.Width; i++)
                {
                    char charToPrint = qrCode.Matrix[i, j] ? '█' : ' ';
                    Console.Write(charToPrint);
                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }
        
        /// <summary>
        /// create qrcode photoes in .png format and output in console
        /// </summary>
        /// <param name="rows"><number of pictures to be created/param>
        /// <param name="stringData"><string array to store data/param>
        public static void PhotoOutput(int rows, string[] stringData)
        {
            FileStream[] resultFile = new FileStream[rows]; // declear qrcode phote files array
            string[] photoname = new string[rows];

            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
            for (int r = 0; r < rows; r++)
            {
                // if the input data are valid (numbers), create photoes
                if (CorrectInput(stringData[r]))
                {
                    photoname[r] = string.Format("{0:D3}", r) + stringData[r].Substring(0, 4) + ".png";
                    resultFile[r] = new FileStream(photoname[r], FileMode.Create);
                    QrCode qrCode = qrEncoder.Encode(stringData[r]);
                    ConsoleOutput(qrCode);
                    GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(30, QuietZoneModules.Four), Brushes.Black, Brushes.White);
                    renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, resultFile[r]);
                }

                // if the input data are invalid, throw an exception and return
                else
                {
                    Console.WriteLine("Invalid input of txt(sql) data.\n'-help' for some help on readme.md");
                    return;
                }
            }
        }

        /// <summary>
        /// read data from excel
        /// </summary>
        /// <param name="path"><the path of excel file/param>
        /// <param name="stringData"><string array to store data, which is pass by reference/param>
        /// <param name="rows"><the rows of data, which is pass by reference/param>
        public static void ReadDataFromExcel(string path, ref string[] stringData, ref int rows)
        {

            string connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
            using (OleDbConnection conn = new OleDbConnection(connstring))
            {
                conn.Open();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("connection success");
                Console.ResetColor();
                DataTable sheetsName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });
                string firstSheetName = sheetsName.Rows[0][2].ToString(); // get the name of first sheet
                string sql = string.Format("SELECT * FROM [{0}]", firstSheetName);
                OleDbDataAdapter adapter = new OleDbDataAdapter(sql, connstring);
                DataSet set = new DataSet();
                adapter.Fill(set);
                rows = set.Tables[0].Rows.Count;
                DataTable dataTable = set.Tables[0];
                for (int i = 0; i < rows; i++)
                {
                    stringData[i] = dataTable.Rows[i].ItemArray[0].ToString();
                }
                conn.Close();
            }

        }

        /// <summary>
        /// read data from sql server
        /// </summary>
        /// <param name="server"><服务器/param>
        /// <param name="port"><端口号/param>
        /// <param name="database"><数据库/param>
        /// <param name="user"><用户名/param>
        /// <param name="password"></密码/param>
        /// <param name="table"><数据表/param>
        /// <param name="stringData"><string array to store data, which is pass by reference/param>
        /// <param name="rows"><the rows of data, which is pass by reference/param>
        public static void ReadDataFromSQL(string server, string port, string database, string user, string password, string table, ref string[] stringData, ref int rows)
        {
            string conStr = "server=" + server + ";port=" + port + ";database=" + database + ";user=" + user + ";password=" + password + ";";
            MySqlConnection con = new MySqlConnection(conStr);

            // get connected with SQL server
            con.Open();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("connection success");
            Console.ResetColor();

            // get total rows of sql table
            string sql = "select count(*) from "+ table;
            MySqlCommand cmd = new MySqlCommand(sql, con);
            object o = cmd.ExecuteScalar();
            rows = Convert.ToInt32(o.ToString());

            // read data from sql tables
            string operation = "select * from "+ table;
            MySqlCommand myc = new MySqlCommand(operation, con);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("operation success");
            Console.ResetColor();

            MySqlDataReader reader = myc.ExecuteReader();
            for (int i = 0; i < rows; i++)
            {
                reader.Read();
                stringData[i] = reader.GetString("number");  // stringData array is passed by reference 
            }
        }

        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args"><command line arguments/param>
        static void Main(string[] args)
        {
            if (args.Length == 1) // the number of command line arguments is 1
            {
                // find the input command
                string startcommand = args[0];                
                if (startcommand.StartsWith("-f")||startcommand.StartsWith("-F")) //captial insensitive
                {
                    string filePath = args[0].Substring(2);                   
                    FileInfo fileinfo = new FileInfo(filePath);

                    // find whether the file exists
                    if (fileinfo.Exists)
                    {
                        int rows = IntFileLines(filePath); // read the rows of txt
                        string[] stringData = new string[rows]; // string array used for storage
                        ReadFileByLines(filePath, ref stringData, rows); // read txt data by lines
                        PhotoOutput(rows, stringData); //create photoes
                    }
                    
                    else
                    {
                        Console.WriteLine("file cannot be found");
                        return;
                    }
                }

                else if (startcommand == "-SQL") // captial sensitive
                {
                    Console.WriteLine("Enter server: ");
                    string server = Console.ReadLine();
                    Console.WriteLine("Enter port(usually 3306): ");
                    string port = Console.ReadLine();
                    Console.WriteLine("Enter user: ");
                    string user = Console.ReadLine();
                    Console.WriteLine("Enter password: ");
                    string password = Console.ReadLine();
                    Console.WriteLine("Select a database: ");
                    string database = Console.ReadLine();
                    Console.WriteLine("Select a table: ");
                    string table = Console.ReadLine();

                    string[] stringData = new string[100]; // string array used for storage
                    int rows = 0; // number of string
                    ReadDataFromSQL(server, port, database, user, password,table, ref stringData, ref rows);
                    PhotoOutput(rows, stringData);
                }
                
                else if (startcommand.StartsWith("-EXCEL"))
                {
                    string excelPath = startcommand.Substring(6);
                    int rows = 0; // number of string
                    string[] stringData = new string[100]; // string array used for storage                    
                    ReadDataFromExcel(excelPath, ref stringData, ref rows);
                    PhotoOutput(rows, stringData);
                }

                else if (startcommand == "-help") // get help from readme.md, captial sensitive
                {
                    Console.WriteLine("https://github.com/3017218198/lab01");
                    return;
                }

                else // exception (invalid argument)
                {
                    Console.WriteLine("Invalid command.\n'-help' for some help on readme.md");
                    return;
                }
            }

            else // exception (invalid number of command line arguments)
            {
                Console.WriteLine("Invalid input, the number of arguments should be 1.\n'-help' for some help on readme.md");
                return;
            }
        }
    }
}
