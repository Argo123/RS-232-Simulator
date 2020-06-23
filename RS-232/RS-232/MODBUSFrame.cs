using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_232
{
    class MODBUSFrame
    {
        public int slaveAdress;

        public string operation;

        public string arguments;

        public string endLine;

        public MODBUSFrame(int address, string op, string ar, string en)
        {
            slaveAdress = address;
            operation = op;
            arguments = ar;
            endLine = en;
        }

        public string MakeFrame()
        {
            return ':' + slaveAdress.ToString() + operation + '.' + arguments + ',' + endLine;
        }
    }
}
