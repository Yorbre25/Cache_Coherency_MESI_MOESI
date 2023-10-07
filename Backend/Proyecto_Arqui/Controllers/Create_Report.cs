using Proyecto_Arqui.Classes;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Proyecto_Arqui.Controllers
{
    public class Create_Report
    {
        public static void create_file(front_end_data exec_data, Transaction_tracker transaction) {
            string fileName = @"C:\Users\Faleivac\Documents\GitHub\Proyecto_I_Arqui_II\Report.txt";

            try
            {
                // Check if file already exists. If yes, delete it.
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file
                using (FileStream fs = File.Create(fileName))
                {
                    // Add some text to file
                    Byte[] title = new UTF8Encoding(true).GetBytes($"Report:\n INV:{exec_data.Report_data["INV"]}\n ReadReq/resp:{exec_data.Report_data["ReadReq"]} \n WriteReq/resp:{exec_data.Report_data["WriteReq"]}\n pe->cache comunication:{transaction.com_types["cache"]}\n pe->pe comunication:{transaction.com_types["pe"]}\n pe->memory comunication:{transaction.com_types["memory"]}\n cost:{transaction.cost}");
                    fs.Write(title, 0, title.Length);
                }

                // Open the stream and read it back.
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

        }
    }
}
