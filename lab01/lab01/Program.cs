using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;


namespace lab01
{
    class Program
    {
        /// <summary>
        /// read the rows of input txt file
        /// </summary>
        /// <param name="path"><path of the file/param>
        /// <returns><lines of the txt file/returns>
        public static int intFileLines(string path)
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
        public static void readFileByLines(string path, ref string[] stringData, int rows)
        {
            StreamReader sr = new StreamReader(path);
            for (int i = 0; i < rows; i++)
            {
                stringData[i] = sr.ReadLine();
            }
        }

        /// <summary>
        /// find whether the input character are numbers
        /// </summary>
        /// <param name="s"><a string/param>
        /// <returns></returns>
        public static bool correctInput(string s)
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
        public static void consoleOutput(QrCode qrCode)
        {
            for (int j = 0; j < qrCode.Matrix.Width; j++)
            {
                for (int i = 0; i < qrCode.Matrix.Width; i++)
                {
                    char charToPrint = qrCode.Matrix[i, j] ? '█' : ' ';
                    Console.Write(charToPrint);
                }
                Console.WriteLine();
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
                        int rows = intFileLines(filePath); // read the rows of txt
                        string[] stringData = new string[100]; // string array used for storage
                        readFileByLines(filePath, ref stringData, rows); // read txt data by lines

                        FileStream[] resultFile = new FileStream[rows]; // declear qrcode phote files array
                        string[] photoname = new string[rows]; 

                        QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
                        for (int r = 0; r < rows; r++)
                        {
                            // find whether the input data are numbers
                            if (correctInput(stringData[r]))
                            {
                                photoname[r] = string.Format("{0:D3}", r) + stringData[r].Substring(0, 4) + ".png";
                                resultFile[r] = new FileStream(photoname[r], FileMode.Create);
                                QrCode qrCode = qrEncoder.Encode(stringData[r]);
                                consoleOutput(qrCode);
                                GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(30, QuietZoneModules.Four), Brushes.Black, Brushes.White);
                                renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, resultFile[r]);                                
                            }

                            else
                            {
                                Console.WriteLine("Invalid input of txt.\n'-help' for some help on readme.md");
                                return;
                            }
                        }

                    }
                    
                    else
                    {
                        Console.WriteLine("file cannot be found");
                        return;
                    }
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
