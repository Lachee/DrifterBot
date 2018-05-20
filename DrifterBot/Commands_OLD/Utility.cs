using DrifterBot.Commands;
using DrifterBot.Helper;
using DrifterBot.Web;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DrifterBot.Commands
{
	public class Utility
	{
		private readonly Regex diceRegex = new Regex(@"(?'count'\d*)[dD](?'sides'\d*)", RegexOptions.Compiled);

		/*
		[Command("ip")]
		[Description("Fetches the current IP address of Hyperlight. Only to be used if DNS failed to update.")]
		public async Task IP(CommandContext ctx)
		{
			await ctx.TriggerTypingAsync();

			string ip = await HTTP.FetchIP();
			var embed = new ResponseBuilder(ctx)
			{
				Author = new DiscordEmbedBuilder.EmbedAuthor()
				{
					Name = "Hyperlight\'s Current IP",
					Url = "http://hyperlight.chickatrice.net/",
					IconUrl = "https://i.imgur.com/KLNIH31.png"
				},
				Description = "Use the DNS address [hyperlight.chickatrice.net](http://hyperlight.chickatrice.net)```" + ip + "```",
				Color = new DiscordColor(12794879),
			};
			await ctx.RespondAsync(embed: embed);
		}

		[Command("raid")]
		public async Task Testing(CommandContext ctx)
		{
			var embed = new DiscordEmbedBuilder();
			embed.Title = "SafetySat";
			embed.ImageUrl = "https://imgs.xkcd.com/comics/safetysat.png";
			await ctx.RespondAsync(embed: embed);
		}
		*/

		[Command("dice")]
		[Description("Gives a random value of a specified dice")]
		public async Task Random(CommandContext ctx, [Description("The shorthand of the number of dice and type of dice.")] string dice = "d6")
		{
			//await ctx.TriggerTypingAsync();

			//Prepare the dice values
			int count = 1;
			int sides = 6;

			//A quick check to account for the defaults
			if (!dice.Equals("d6"))
			{
				//Split the dice up
				var match = diceRegex.Match(dice);
				if (match.Success)
				{
					if (!string.IsNullOrEmpty(match.Groups["count"].Value))
					{
						if (!int.TryParse(match.Groups["count"].Value, out count) || count < 1)
						{
							await ctx.RespondAsync("❌ Invalid dice string. The count must be a `integer` that is greater than 0 (`> 0`)");
							return;
						}
					}

					if (!string.IsNullOrEmpty(match.Groups["sides"].Value))
					{
						if (!int.TryParse(match.Groups["sides"].Value, out sides))
						{
							await ctx.RespondAsync("❌ Invalid dice string. The sides must be a `integer`.");
							return;
						}

						if (sides <= 1)
						{
							await ctx.RespondAsync("🎲 Silly " + ctx.Member.Mention + ", did you forget that a 1 sided dice always returns 1?");
							return;
						}
					}
				}
				else
				{
					await ctx.RespondAsync($"❌ Invalid dice string. Please use the following format: ``` [count]d[sides] ``` `count` defaults to `{count}` and `sides` defaults to `{sides}`. They must be integers.");
					return;
				}
			}

			//calculate teh value
			Random random = new System.Random();

			int tally = 0;
			for (int i = 0; i < count; i++)
				tally += random.Next(1, sides);

			var embed = new ResponseBuilder(ctx)
			{
				Description = $"🎲 Your random number is: `" + tally.ToString() + "`",
				Color = new DiscordColor(255, 96, 96),
			};

			await ctx.RespondAsync(embed: embed);
		}
	}
}
