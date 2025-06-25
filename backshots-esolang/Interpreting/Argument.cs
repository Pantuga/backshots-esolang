using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackshotsEsolang.Interpreting
{
    public class Argument()
    {
        public override string ToString()
        {
            return "No Value";
        }
    }
    public class ArgumentDouble(double val) : Argument()
    {
        public readonly double Value = val;

        public override string ToString()
        {
            if (double.IsInfinity(Value)) return "+/- Infinity";
            if (double.IsNaN(Value)) return "NaN";
            return Value.ToString();
        }
    }

    public class ArgumentArray(Argument[] val) : Argument()
    {
        public readonly Argument[] Value = val;

        public override string ToString()
        {
            StringBuilder sb = new("[");
            for (int i = 0; i < Value.Length; i++)
            {
                if(i > 0) sb.Append(", ");
                sb.Append(Value[i].ToString());
            }
            sb.Append(']');

            return sb.ToString();
        }
    }
}
