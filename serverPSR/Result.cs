using System;
using System.Collections.Generic;

namespace ProgramowanieSystemówRozproszonych
{
    public class Result
    {
        public List<double> ResultAll = new List<double>();

        public void ResultALlFile(List<double> Res)
        {
            double sum = 0;
            double odejmij = 0;
            double dzielenie = 1;
            double mnożenie = 1;

            for (int i = 0; i < Res.Count; i++)
            {
                if (i % 3 == 0)
                {
                    sum += Res[i];
                }
                else if (i % 3 == 1)
                {
                    odejmij = odejmij - Res[i];
                }
                else if (i % 3 == 2)
                {
                    mnożenie *= Res[i];
                }
                else
                {
                    dzielenie += Res[i];
                }
            }
            Console.WriteLine("Ostateczne wyniki działania suma wszystkich liczb to: {0} odejmowanie wszytkich liczb to: {1} iloraz liczb to: {2} ,, suma z dzieleń {3}", sum, odejmij, mnożenie, dzielenie);
            //Thread.Sleep(4000);
        }
    }
}