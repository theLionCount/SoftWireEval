using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftWireEvaluation
{
    public class Booking
    {
        public int rowNum { get; set; }
        public int leftSeat { get; set; }
        public int rightSeat { get; set; }

        public static Booking fromString(string line)
        {
            int rownum1 = Convert.ToInt32(line.Split(',')[1].Split(':')[0]);
            int rownum2 = Convert.ToInt32(line.Split(',')[2].Split(':')[0]);

            if (rownum1 != rownum2 || rownum1 >= 100 || rownum1 < 0) return null; //rownum invalid

            int seatnum1 = Convert.ToInt32(line.Split(',')[1].Split(':')[1]);
            int seatnum2 = Convert.ToInt32(line.Split(',')[2].Split(':')[1]);

            if (seatnum2 - seatnum1 >= 5 || seatnum1 >= 50 || seatnum2 >= 50 || seatnum1<0 || seatnum2<0 || seatnum2 < seatnum1) return null; //seatnums invalid

            return new Booking() { rowNum = rownum1, leftSeat = seatnum1, rightSeat = seatnum2 };
        }

        public ulong getBinaryRepresentation()
        {
            return ((ulong)(Math.Pow(2, rightSeat - leftSeat + 1)) - 1UL) << ((byte)leftSeat + 1);
        }
    }
}
