using System;
using System.IO;

namespace SoftWireEvaluation
{
    class Program
    {
        public static readonly int rows = 100;
        public static readonly int columns = 50;
        public static readonly int maxBookingSize = 5;

        static bool[,] seats = new bool[rows, columns]; //for not insane solutions;
        static ulong[] binarySeats = new ulong[rows];   //for bitwise solution;

        static void Main(string[] args)
        {
            Console.Write("Filename: ");
            string fileName = Console.ReadLine();

            using StreamReader sr = new StreamReader(fileName);
            int rejected = 0;
            int rejectedBitwise = 0;

            initializeSeats();

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine().Replace("(","").Replace(")","");
                var booking = Booking.fromString(line);

                if (booking != null && validateBooking(booking)) book(booking); //normal solution
                else rejected++;

                if (booking != null && checkAvaibilityBitwise(booking)) binarySeats[booking.rowNum] = getBookedLineBitwise(booking); //bitwise solution
                else rejectedBitwise++;
            }

            Console.WriteLine("Rejected bookings: ");
            Console.WriteLine(rejected);
            Console.WriteLine(rejectedBitwise);
        }

        #region third solution with bitwise magic, just cause I think it might be faster, and I had more than enough time to spare;

        static void initializeSeats()
        {
            for (int i = 0; i < rows; i++)
            {
                binarySeats[i] = 1UL | (1UL << ((byte)columns + 1));
            }
        }

        static ulong getBookedLineBitwise(Booking booking)
        {
            return binarySeats[booking.rowNum] | booking.getBinaryRepresentation(); 
        }

        static bool checkAvaibilityBitwise(Booking booking)
        {
            ulong line = booking.getBinaryRepresentation();
            ulong avaible = binarySeats[booking.rowNum] & line;
            
            if (avaible != 0) return false;

            ulong bookedLine = getBookedLineBitwise(booking);
            ulong patternCheck = ~bookedLine & (bookedLine << 1) & (bookedLine >> 1);
            
            if (patternCheck != 0) return false;
            
            return true;
        }

        #endregion

        #region second, more modular solution

        static void book(Booking booking)
        {
            for (int i = booking.leftSeat; i <= booking.rightSeat; i++)
            {
                seats[booking.rowNum, i] = true;
            }
        }

        static bool validateBooking(Booking booking)
        {
            return validateLeftSide(booking) && validateRightSide(booking) && validateAvaibility(booking);
        }

        static bool validateLeftSide(Booking booking)
        {
            if (booking.leftSeat == 1 && !seats[booking.rowNum, 0]) return false;
            if (booking.leftSeat >= 2 && !seats[booking.rowNum, booking.leftSeat - 1] && seats[booking.rowNum, booking.leftSeat - 2]) return false;
            return true;
        }

        static bool validateRightSide(Booking booking)
        {
            if (booking.rightSeat == columns - 2 && !seats[booking.rowNum, columns - 1]) return false;
            if (booking.rightSeat <= columns - 3 && !seats[booking.rowNum, booking.rightSeat + 1] && seats[booking.rowNum, booking.rightSeat + 2]) return false;
            return true;
        }

        static bool validateAvaibility(Booking booking)
        {
            for (int i = booking.leftSeat; i <= booking.rightSeat; i++)
            {
                if (seats[booking.rowNum, i]) return false;
            }
            return true;
        }

        #endregion

        #region initial naive solution

        static bool processSeatsInitial(string line) //Naive solution, done in 15 or so minutes;
        {
            int rownum1 = Convert.ToInt32(line.Split(',')[1].Split(':')[0]);
            int rownum2 = Convert.ToInt32(line.Split(',')[2].Split(':')[0]);
            
            if (rownum1 != rownum2 || rownum1 >= 100) return true;

            int seatnum1 = Convert.ToInt32(line.Split(',')[1].Split(':')[1]);
            int seatnum2 = Convert.ToInt32(line.Split(',')[2].Split(':')[1]);

            if (seatnum2 - seatnum1 >= 5 || seatnum1>=50 || seatnum2>=50) return true;

            if (seatnum1 == 1 && !seats[rownum1, 0]) return true;
            if (seatnum1 >= 2 && !seats[rownum1, seatnum1 - 1] && seats[rownum1, seatnum1 - 2]) return true;

            if (seatnum2 == 48 && !seats[rownum1, 49]) return true;
            if (seatnum2 <= 47 && !seats[rownum1, seatnum2 + 1] && seats[rownum1, seatnum2 + 2]) return true;

            for (int i = seatnum1; i <= seatnum2; i++)
            {
                if (seats[rownum1, i]) return true;
            }

            for (int i = seatnum1; i <= seatnum2; i++)
            {
                seats[rownum1, i] = true;
            }

            return false;
        }

        #endregion
    }
}
