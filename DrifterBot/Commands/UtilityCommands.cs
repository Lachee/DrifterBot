using DrifterBot.Helper;
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
	public class UtilityCommands
	{
		private readonly Regex DICE_REGEX = new Regex(@"(?'count'\d*)[dD](?'sides'-?\d*)", RegexOptions.Compiled);

		[Command("dice")]
		[Description("Gives a random value of a specified dice")]
		public async Task Random(CommandContext ctx, [Description("The shorthand of the number of dice and type of dice.")] string dice = "d6")
		{
			await ctx.TriggerTypingAsync();

			//Prepare the dice values
			int count = 1;
			int sides = 6;
			bool isNegative = false;

			//A quick check to account for the defaults
			if (!dice.Equals("d6"))
			{
				//Split the dice up
				var match = DICE_REGEX.Match(dice);
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

						if (sides == 1 || sides == -1)
						{
							await ctx.RespondAsync("🎲 Silly " + ctx.Member.Mention + ", did you forget that a 1 sided dice always returns 1?");
							return;
						}
						else if (sides == 0)
						{
							await ctx.RespondAsync("🎲 As you grasp the die " + ctx.Member.Mention + ", you notice a tingling sensation in your hand. You look down, into your palm and see nothing. Not even your palm. The wind starts rushing past you, getting faster and faster, sinking into where your palm use to be. You have opened a black hole, and your answer is within it");
							return;
						}
						else if (sides < 0)
						{
							isNegative = true;
							sides *= -1;
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

			if (isNegative)
			{
				tally *= -1;
				await ctx.RespondAsync(embed: new ResponseBuilder(ctx)
				{
					Description = $"🎲 Through the inverted dice, and the 4th dimensional plane, you rolled the dice. Lighting struck and the wind wirled. Chaos ensured and the daemons of Mentioth screamed. The dice, somehow still within this mortal plane, revealed `" + tally.ToString() + "`",
					Color = DiscordColor.Black,
				});
			}
			else
			{
				await ctx.RespondAsync(embed: new ResponseBuilder(ctx)
				{
					Description = $"🎲 Your random number is: `" + tally.ToString() + "`",
					Color = new DiscordColor(255, 96, 96),
				});
			}
		}

	}
}
