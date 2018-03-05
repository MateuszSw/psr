using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProgramowanieSystemówRozproszonych
{
    public class Program
    {
        private int number_of_clients = 3;
        
        private int Timeout = 1000 * 10; //10 sek
        private List<double> ResultAll = new List<double>();
        private TcpClient[] clientArray = new TcpClient[3];
        private bool?[] workedDataSets = new bool?[3];

        private static void Main(string[] args)
        {
            string numbersPathFile = "../../liczbyDużo.txt";
            Program p = new Program();
            Thread thread = new Thread(()=> p.run(numbersPathFile));
            
            thread.Start();
            Console.ReadLine();

        }

        public void run(string file)
        {
            List<double> table = GetNumbersFromFile(file);
            Console.WriteLine("ilosc elementow w pliku " + table.Count);
            List<List<double>> partialList = PartialList(table, this.clientArray.Length);
            Console.WriteLine(this.clientArray.Length);
            Server(partialList);
            
        }

        public void Server(List<List<double>> partialList)
        {
            //int idClient = 0;
            const int PORT_NO = 7777;
            TcpListener listener = new TcpListener(IPAddress.Any, PORT_NO);
            listener.Start();
            Console.WriteLine("Nasłuchuj czy połączył się jakiś klient...");

            while (workedDataSets.Any(x => x == null || x == false))
            {
                var client_no = Array.FindIndex(workedDataSets, (x) => x == null);
                
                if (client_no >= 0)
                {
                    clientArray[client_no] = listener.AcceptTcpClient();

                    if (clientArray[client_no].Connected)
                    {
                        workedDataSets[client_no] = false;
                        Console.WriteLine("Połączył się klient o numerze {0}", client_no);
                        Thread clientThread = new Thread(() => SendReceiveDataClient(partialList, clientArray[client_no], client_no));
                        clientThread.Start();
                    }
                }
            }
            new Result().ResultALlFile(ResultAll);
            listener.Stop();
            Console.ReadLine();
        }

        private string readData(TcpClient client)
        {
            NetworkStream nwStream = client.GetStream();
            nwStream.ReadTimeout = Timeout;
            string dataReceived = string.Empty;
            int bytesRead = default(int);
            bool TimeoutRaised = false;
            do
            {
                TimeoutRaised = false;
                try
                {
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    bytesRead = nwStream.Read(buffer, 0, (int)client.ReceiveBufferSize);
                    dataReceived += Encoding.ASCII.GetString(buffer, 0, bytesRead);
                }
                catch (IOException)
                {
                    TimeoutRaised = true;
                    if (!client.Connected)
                        throw new Exception("Klient rozłączył się");
                }
            } while ((!TimeoutRaised && bytesRead == client.ReceiveBufferSize) || (TimeoutRaised && client.Connected));
            return dataReceived;
        }

        private void sendData(TcpClient client, string sendData)
        {
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = buffer = Encoding.ASCII.GetBytes(sendData);
            nwStream.Write(buffer, 0, buffer.Length);
        }

        private void SendReceiveDataClient(List<List<double>> partialList, TcpClient client, int client_no)
        {
            try
            {
                string dataReceived = readData(client);
                Console.WriteLine(dataReceived);
                List<double> Result = new List<double>();
                if (dataReceived == "GET LIST")
                {
                    sendData(client, String.Join(" ", partialList[client_no]));
                }
                else
                    throw new Exception("Nieprawidłowa sekwencja");
                dataReceived = readData(client);
                Console.WriteLine("Wyniki od klienta");
                Console.WriteLine("Received : " + dataReceived);
                Result = dataReceived.Split(' ').Select(x => Double.Parse(x)).ToList<double>();
                ResultAll.AddRange(Result);
                workedDataSets[client_no] = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                workedDataSets[client_no] = null;
            }
            finally
            {
                client.Close();
                clientArray[client_no] = null;
            }
        }

        public List<double> GetNumbersFromFile(string pathFile)
        {
            List<double> numbers;
            using (StreamReader sr = new StreamReader(pathFile))
            {
                String Line = sr.ReadToEnd();
                int countSpace = Line.Count(x => x == ' ');
                Console.WriteLine(countSpace);
                numbers = ParseStringToList(Line);
            }
            return numbers;
        }

        private static List<double> ParseStringToList(string Line)
        {
            //var DecimalSeparator = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
            //var OldChar = DecimalSeparator == "." ? "," : ".";
            //var NewChar = OldChar == "." ? "," : ".";
            return Line.Split(' ').Select(x => Convert.ToDouble(x)).ToList<double>();
        }

        public List<double> PrintElementsList(List<double> list)
        {
            Console.WriteLine();
            list.ForEach(x => Console.Write(x + " "));
            Console.WriteLine();
            return list;
        }

        public List<List<double>> PartialList(List<double> list, int iloscczesci)
        {
            int count = 0;
            List<List<double>> partsList = new List<List<double>>();

            for (int i = 0; i < iloscczesci; i++)
            {
                partsList.Add(new List<double>());
            }
            

            foreach (var item in list)
            {
                partsList[count % iloscczesci].Add(item);
                //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                count++;
            }

            return partsList;
        }

        //private static List<double> ParseStringToList(string Line)
        //{
        //    var DecimalSeparator = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
        //    var OldChar = DecimalSeparator == "." ? "," : ".";
        //    var NewChar = OldChar == "." ? "," : ".";
        //    return Line.Split(' ').Select(x => double.Parse(x.Replace(OldChar, NewChar))).ToList<double>();
        //}
    }
}