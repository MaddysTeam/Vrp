using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business.Utils
{

	public class IDCardCheck
	{

		#region [ Fields ]

		/// <summary>
		///   Represent the special address code
		/// </summary>
		private static int[] addressCodes = { 11, 12, 13, 14, 15, 21, 22, 23, 31, 32, 33, 34, 35, 36, 37, 41, 42, 43, 44, 45, 46, 50, 51, 52, 53, 54, 61, 62, 63, 64, 65, 71, 81, 82 };

		/// <summary>
		/// ISO 7064:1983, MOD 11-2 Code Checking System
		/// </summary>
		private static int[] Wi = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };

		/// <summary>
		/// ISO 7064:1983, MOD 11-2 Code Checking System Allowed check number.
		/// </summary>
		private static string[] AllowedCheckCodes = { "1", "0", "x", "9", "8", "7", "6", "5", "4", "3", "2" };

		private const int ADDRESS_CODE_LENGHT = 6;
		private const int BIRTHDAY_CODE_LENGHT = 8;
		private const int SERIAL_CODE_LENGTH = 3;
		private const int VALID_LENGTH = 18;

		#endregion

		public static bool CheckIdentityCode(string value, out DateTime birthday)
		{
			int strLen = value.Length;
			birthday = DateTime.Now;

			if (strLen != 18)
				return false;

			string code;

			// 地址码
			code = value.Substring(0, ADDRESS_CODE_LENGHT);
			if (!CheckAddressCode(code))
				return false;

			// 日期码
			code = value.Substring(ADDRESS_CODE_LENGHT, BIRTHDAY_CODE_LENGHT);
			if (!CheckBirthDayCode(code, out birthday))
				return false;

			// 顺序码
			code = value.Substring(ADDRESS_CODE_LENGHT + BIRTHDAY_CODE_LENGHT, SERIAL_CODE_LENGTH);
			if (!CheckSerialCode(code))
				return false;


			if (!CheckSum(value))
				return false;

			return true;
		}

		private static bool CheckAddressCode(string addr_value)
		{
			int value;

			if (Int32.TryParse(addr_value, out value))
			{
				value /= 10000;

				int end = addressCodes.Length - 1;
				int start = 0;
				int mid = (end - start) / 2;

				while (end >= start)
				{
					if (value > addressCodes[mid])
					{
						start = mid + 1;
					}
					else if (value < addressCodes[mid])
					{
						end = mid - 1;
					}
					else
					{

						return true;
					}
					mid = (end - start) / 2 + start;
				}
			}

			return false;
		}

		private static bool CheckBirthDayCode(string value, out DateTime birthday)
		{
			string birth = value.Substring(0, 4) + "-" + value.Substring(4, 2) + "-" + value.Substring(6, 2);
			if (DateTime.TryParse(birth, out birthday))
			{
				DateTime lowbound = new DateTime(1870, 1, 1);
				DateTime upperbound = new DateTime(2010, 12, 31);
				if (birthday >= lowbound && birthday <= upperbound)
				{
					return true;
				}
			}
			return false;
		}

		private static bool CheckSerialCode(string serial)
		{
			int iSerial;
			if (Int32.TryParse(serial, out iSerial) && iSerial != 0)
			{
				return true;
			}
			return false;
		}

		private static bool CheckSum(string value)
		{
			string check = value.Substring(17, 1);
			char[] Ai = value.ToCharArray();

			int val = 0;

			for (int i = 0; i < 17; i++)
			{
				val += Wi[i] * (Ai[i] - '0');
			}
			if (check.ToLower() != AllowedCheckCodes[val % 11])
			{
				return false;
			}

			return true;
		}

	}

}