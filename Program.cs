using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PPSFixer
{
    class Program
    {
        static string inputFilePath = @"C:\temp\AODA_1092_20190827_1415.xml";
        static string resultsFilePath = string.Concat(inputFilePath.Substring(0, inputFilePath.Length - 4), "_Result.xml"); 
        static XDocument resultsFile;
        static XDocument inputFile;
        static List<string> errorRecordIDs = new List<string>();
        static int count = 0;

        static void Main(string[] args)
        {
            inputFile = LoadXML(inputFilePath);
            resultsFile = LoadXML(resultsFilePath);
            FindErrorRecords();
            CleanInputFile();
            WriteInputFile();
            Console.WriteLine($"Records fixed: {count}");
            Console.ReadKey();
        }

        static XDocument LoadXML(string filePath)
        {
            return XDocument.Load(filePath);
        }

        static void WriteInputFile()
        {
            inputFile.Save(string.Concat(inputFilePath.Substring(0, inputFilePath.Length - 4), "_Fixed.xml"));
        }

        static void FindErrorRecords()
        {
            //List<XElement> errors = resultsFile.Descendants("detail_record").Descendants("errors_record").Descendants("error_msg").Where(msg => msg.Value == @"Please provide a valid 'AODA Closing Status code(<close_status_A>)'.").ToList();
            //foreach (XElement error in errors)
            //{
            //    errorRecordIDs.Add(error.Parent.Element("record_id").Value);
            //    count++;
            //}
            List<XElement> errors = resultsFile.Descendants("detail_record").Descendants("errors_record").Descendants("error_msg").Where(msg => msg.Value.StartsWith(@"Please provide the value for either all or none of the 'AODA Closing Status code")).ToList();
            foreach (XElement error in errors)
            {
                errorRecordIDs.Add(error.Parent.Element("record_id").Value);
                count++;
            }
        }

        static void CleanInputFile()
        {
            foreach (string errorRecordID in errorRecordIDs)
            {
                XElement errorElement = inputFile.Descendants("detail_record").Where(r => r.Element("record_id").Value == errorRecordID).FirstOrDefault();
                if (errorElement == null)
                {
                    Console.WriteLine($"Found Null: {errorRecordID}");
                }

                errorElement.Element("close_status_A").Remove();
                errorElement.Element("close_status_E").Remove();
                errorElement.Element("close_status_AR").Remove();
                errorElement.Element("close_status_LA").Remove();
                errorElement.Element("close_status_support_group").Remove();
            }
        }
    }
}
