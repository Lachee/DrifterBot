using DrifterBot.Helper;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using RconSharp;
using RconSharp.Net45;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrifterBot.Commands
{
	public class Minecraft
	{
		private static Rcon RCON = new Rcon("hyperlight", 25575, "ourverysecretpassword");

		[Command("list")]
		[Description("Lists all the players that are currently on the server")]
		public async Task List(CommandContext ctx)
		{
			await Execute(ctx, "list");
		}

		[Command("kill")]
		[Description("Kills a player")]
		public async Task Kill(CommandContext ctx, [Description("The player to kill")] string player)
		{
			await ExecuteSafe(ctx, "kill", player);
		}

		[Command("give")]
		[Description("Gives a minecraft user x amount of times")]
		public async Task GiveItem(CommandContext ctx, [Description("The player to give items too")] string player, [Description("The item to give")] string item, [Description("Number of said items")] int count)
		{
			await ctx.TriggerTypingAsync();
			if (!ctx.Member.IsOwner)
			{
				await ctx.RespondAsync("I am sorry, but only the owner of the server can execute this command directly. Please use one of the other `minecraft` commands.");
				return;
			}

			string command = "give {0} {1} {2}";

			int tally = count;
			while (tally > 0)
			{
				int amount = tally;
				if (amount > count) amount = count;
				if (amount > 64) amount = 64;

				await RCON.Execute(string.Format(command, player, item, amount));

				tally -= amount;
			}

			//Create the embed
			var embed = new ResponseBuilder(ctx)
			{
				Author = new DiscordEmbedBuilder.EmbedAuthor()
				{
					Name = "Hyperlight\'s Current IP",
					Url = "http://hyperlight.chickatrice.net/",
					IconUrl = "https://i.imgur.com/KLNIH31.png"
				},
				Description = "Finished giving " + player + " " + count + " " + item + "s",
				Color = new DiscordColor(12794879),
			};

			//Send it away!
			await ctx.RespondAsync(embed: embed);
		}

		[Command("rcon")]
		[Description("Executes a RCON command.")]
		public async Task ExecuteSafe(CommandContext ctx, [Description("The command to execute")] string command, params string[] args)
		{
			if (!ctx.Member.IsOwner)
			{
				await ctx.RespondAsync("I am sorry, but only the owner of the server can execute this command directly. Please use one of the other `minecraft` commands.");
				return;
			}

			await Execute(ctx, command, args);
		}

		public async Task Execute(CommandContext ctx, [Description("The command to execute")] string command, params string[] args)
		{
			await ctx.TriggerTypingAsync();

			//Build the command
			StringBuilder builder = new StringBuilder();
			builder.Append(command).Append(" ");

			for (int i = 0; i < args.Length; i++)			
				builder.Append(args[i]).Append(" ");

			//Execute the rcon
			RconResponse response = await RCON.Execute(builder.ToString());

			//Prepare the description
			bool hasErrored = false;
			string description = "RCON executed ";

			//HAve we actually got a message?
			if (!string.IsNullOrEmpty(response.Message))
			{
				description += "and the server responded with: ``` " + response.Message + " ```";
			}
			else
			{
				description += "and the server had a empty response.";
			}

			//Have we errored?
			if (!string.IsNullOrEmpty(response.Error))
			{
				description += "\n\nThe rcon ran into a error while processing the command: ``` " + response.Error + " ```";
				hasErrored = true;
			}

			//Create the embed
			var embed = new ResponseBuilder(ctx)
			{
				Author = new DiscordEmbedBuilder.EmbedAuthor()
				{
					Name = "Hyperlight\'s Current IP",
					Url = "http://hyperlight.chickatrice.net/",
					IconUrl = "https://i.imgur.com/KLNIH31.png"
				},
				Description = description,
				Color = hasErrored ? new DiscordColor(255, 96, 96) : new DiscordColor(12794879),
			};

			//Send it away!
			await ctx.RespondAsync(embed: embed);
		}
	}
}
