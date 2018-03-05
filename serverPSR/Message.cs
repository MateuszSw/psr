using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ProgramowanieSystemówRozproszonych
{
    internal class Message
    {
        public void NewMethod(List<List<double>> partialList, TcpClient client)
        {
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[1000];

            int bytesRead = nwStream.Read(buffer, 0, 1000);

            string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine(dataReceived);
            if (dataReceived == "GET LIST")
            {
                string sendData = String.Join(" ", partialList[1]);
                buffer = Encoding.ASCII.GetBytes(sendData);
                Console.WriteLine("Wyslij odpowiednią listę  : " + sendData);
                nwStream.Write(buffer, 0, sendData.Length);
            }
            else
            {
                List<double> Result = new List<double>();
                Console.WriteLine("Wyniki od klienta");
                Result = dataReceived.Split(' ').Select(x => Double.Parse(x)).ToList<double>();
                Program p = new Program();
                p.PrintElementsList(Result);
                //Console.ReadLine();
            }
            Console.WriteLine("Received : " + dataReceived);
        }
    }
}