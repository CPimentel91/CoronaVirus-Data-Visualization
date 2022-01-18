using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CoronaData
{
    class ViewStateData
    {
        public List<State> Data { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ViewStateData()
        {
            Data = new List<State>();
        }
    }
}
