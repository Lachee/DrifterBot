using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DrifterBot.Web
{
	class HTTP
	{
		const string IP_API = "https://api.ipify.org/?format=text";

		/// <summary>
		/// Fetches a string from a URL
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static  async Task<string> FetchString(string url)
		{
			using (WebClient client = new WebClient())
			{
				return await client.DownloadStringTaskAsync(url);
			}
		}

		/// <summary>
		/// Fetches the public IP of this machine
		/// </summary>
		/// <returns></returns>
		public static async Task<string> FetchIP()
		{
			return await FetchString(IP_API);
		}
	}
}
