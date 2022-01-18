using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CoronaData
{
    public class ViewUnitedStatesData 
    {
        public int ConfirmedDeaths { get; set; }
        public int ConfirmedCases { get; set; }



        public ViewUnitedStatesData(string cases, string deaths)
        {

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
