using System;
using System.Collections.Generic;
using System.Text;

namespace NBitcoin
{
	public static class NBitcoinExtensions
	{
		public static bool CanDecrypt(this Key me, string message)
		{
			try
			{
				me.Decrypt(message);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
