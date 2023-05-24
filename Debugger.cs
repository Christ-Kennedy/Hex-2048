using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hex_2048
{
public class Debugger
    {
        public static void Print(string strMsg)
        {
            System.Diagnostics.Debug.Print(strMsg);
        }

        public static string byteToString(byte bytIn)
        {
            int intIn = (int)bytIn;
            return intToString(intIn);
        }

        public static string longToString(long lngIn)
        {
            int intLSInt = (int)lngIn;
            int intMSInt = (int)(lngIn / Math.Pow(2, 32));

            return intToString(intMSInt) + intToString(intLSInt);
        }
        
        public static string ulongToString(ulong lngIn)
        {
            int intLSInt = (int)lngIn;
            int intMSInt = (int)(lngIn / Math.Pow(2, 32));

            return intToString(intMSInt) + intToString(intLSInt);
        }
        
        public static string intToString(int intIn)
        {
            string strRetVal = "";
            for (int intBitCounter = 0; intBitCounter < 32; intBitCounter++)
            {
                char chr = intIn % 2 == 0
                                    ? '0'
                                    : '1';
                strRetVal = chr.ToString() + strRetVal;
                intIn = intIn >> 1;
            }
            return strRetVal;

        }
        
        public static string uintToString(uint uintIn)
        {
            int intIn = (int)uintIn;
            return intToString(intIn);

        }
        
        public static string shortToString(short shIn)
        {
            int intIn = (int)shIn;
            return intToString(intIn);
        }
        
        public static string ushortToString(ushort ushIn)
        {
            int intIn = (int)ushIn;
            return intToString(intIn);
        }
    }
}
