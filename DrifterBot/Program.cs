using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus.Net.WebSocket;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;

namespace DrifterBot
{
	class Program
	{
		static void Main(string[] args)
		{
			MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public static Drifter bot;
		public static DiscordClient discord;
		public static VoiceNextClient voice;
		static async Task MainAsync(string[] args)
		{
			//DO this tutorial next: https://dsharpplus.emzi0767.com/articles/commandsnext.html
			discord = new DiscordClient(new DiscordConfiguration
			{
				Token = System.IO.File.ReadAllText("bot.key"),
				TokenType = TokenType.Bot,
				UseInternalLogHandler = true,
				LogLevel = LogLevel.Debug
			});

			discord.SetWebSocketClient<WebSocket4NetClient>();
			voice = discord.UseVoiceNext();

			await discord.ConnectAsync();

			bot = new Drifter(discord);
			await bot.Initialize();

			await Task.Delay(-1);
		}
	}
}
