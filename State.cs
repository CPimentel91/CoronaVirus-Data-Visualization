using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CoronaData
{
    public class State 
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public int TotalDeaths { get; set; }
        public int TotalCases { get; set; }
        public int ConfirmedDeaths { get; set; }
        public int ConfirmedCases { get; set; }
        public int Population { get; set; }
        public SolidColorBrush Color { get; set; }
        public double PerCapitaDeaths { get; set; }

        public State(string name, string deaths, string cases)
        {
            Name = name;
            int temp1;
            int temp2;
            if (int.TryParse(deaths, out temp1))
            {
                ConfirmedDeaths = temp1;
            }
            else
            {
                ConfirmedDeaths = 9999;
            }

            if (int.TryParse(cases, out temp2))
            {
                ConfirmedCases = temp2;
            }
            else
            {
                ConfirmedCases = 9999;
            }

        }

       

    }
}
