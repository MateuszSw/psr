using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProgramowanieSystemówRozproszonychKlient
{
    internal class Klient
    {
        

        
        

        private static void Main(string[] args)
        {
            Klient c = new Klient();

            Stopwatch watch = new Stopwatch();
            watch.Start();
            c.Client();
            
            watch.Stop();
            Console.WriteLine($"Execution time: {watch.ElapsedMilliseconds}ms.)");

            Console.ReadLine();
        }

        public void Client()
        {
            string textToSend = "GET LIST";
            const string SERVER_IP = "192.168.1.93";
            //string SERVER_IP1 = Console.ReadLine();
            const int PORT_NO = 7777;
            TcpClient client = new TcpClient(SERVER_IP, PORT_NO);
            try
            {
                var recived_data = sendData(client, textToSend, true);
                string result = DoMath(ParseStringToList(recived_data));
                Console.WriteLine(result);
                
                sendData(client, result);
                
                Console.WriteLine("Wyślij wynik dodawania odejmowania mnożenia dzielenia : " + result);
                Console.ReadLine();
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex);
            }
            finally
            {
                client.Close();
            }
            Console.ReadLine();
        }

        private static List<double> ParseStringToList(string Line)
        {
            var DecimalSeparator = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
            var oldChar = DecimalSeparator == "." ? "," : ".";
            var newChar = oldChar == "." ? "," : ".";
            return Line.Split(' ').Select(x => double.Parse(x.Replace(oldChar, newChar))).ToList<double>();
        }

        private string DoMath(List<double> listNumbers)
        {
            List<double> Result = new List<double>();
            double add =  AddElementsList(listNumbers);
            double multiply =  MultipleElementsList(listNumbers);
            Result.Add(add);
            Result.Add( SubtractElementsList(listNumbers));
            Result.Add(multiply);
            Result.Add(Divivsion(add, multiply));
            return string.Join(" ", Result.ToArray());
        }

        private string sendData(TcpClient client, string data, bool read_response = false)
        {
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(data);
            NetworkStream nwStream = client.GetStream();
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
            nwStream.Flush();
            if (read_response)
            {
                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesRead = nwStream.Read(bytesToRead, 0, bytesToRead.Length);
                if (bytesRead == 0)
                {
                    Console.WriteLine("cos poszło nie tak");
                }
                return Encoding.ASCII.GetString(bytesToRead);
            }
            else return String.Empty;
        }

        public List<double> PrintElementsList(List<double> list)
        {
            Console.WriteLine();
            list.ForEach(x => Console.Write(x + " "));
            Console.WriteLine();
            return list;
        }

        public double AddElementsList(List<double> list)
        {
            double sum = 0;
            foreach (var item in list)
            {
                sum += item;
            }
            return sum;
        }

        public static double SubtractElementsList(List<double> list)
        {
            double sub = 0;

            foreach (var item in list)
            {
                sub = sub - item;
            }
            return sub;
        }

        public static double MultipleElementsList(List<double> list)
        {
            double mul = 1;
            double temp;

            Parallel.ForEach(list, item =>
            {
                if (item == 0)
                {
                    item = 0.1;
                }
                if (Double.IsInfinity(mul) || mul == 0)
                {
                    mul = 1;
                }
                mul = mul * item;
                double roundTo = Math.Pow(10, 3);
                double resultResult = Math.Truncate(mul * roundTo) / roundTo;
                mul = resultResult;
                
            });

            //foreach (var item in list)
            //{
            //    mul = mul * item;
            //    Console.WriteLine(mul);
            //}

            return mul;
        }

        public double Divivsion(double add, double multiply)
        {
            double div = 0;
            if (multiply >= add)
            {
                div = multiply / add;
            }
            else
            {
                div = add / multiply;
            }
            return div;
        }
    }
}