using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaData
{
    class ViewDailyDeaths
    {
        public string Name { get; set; }
        public int Deaths { get; set; }
        public int Cases { get; set; }
        public ViewDailyDeaths(string name, string cases, string deaths)
        {
            int temp1;
            int temp2;
            if (int.TryParse(deaths, out temp1))
            {
                Deaths = temp1;
            }
            else
            {
                Deaths = 9999;
            }

            if (int.TryParse(cases, out temp2))
            {
                Cases = temp2;
            }
            else
            {
                Cases = 9999;
            }
        }

    }
}
