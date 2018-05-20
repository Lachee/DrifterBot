
using DrifterBot.Helper;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace DrifterBot.Commands
{
	[Group("admin"), RequireOwner, Hidden]
	public class AdminCommands
	{
		private readonly ulong LACHEE_UID = 130973321683533824L;

		[Command("del")]
		[Description("deletes own message, including the senders.")]
		public async Task DeleteSelfMessage(CommandContext ctx, DiscordMessage message)
		{
			if (!await AllowedExecution(ctx))
				return;

			//Delete the last message
			await ctx.Message.DeleteAsync();
			await message.DeleteAsync();
		}

		[Command("ping")]
		[Description("Makes the bot respond with PONG.")]
		public async Task Ping(CommandContext ctx)
		{
			var embed = new ResponseBuilder(ctx)
				.WithDescription("```PONG```")
				.AddField("Guild Name", ctx.Guild.Name)
				.AddField("Channel Name", ctx.Guild.Name)
				.AddField("Message ID", ctx.Message.Id.ToString())
				.AddField("User Name", ctx.Member.Username);
			await ctx.RespondAsync(embed: embed);
		}

		[Command("serbs")]
		[Description("Lists all the servers this bot is appart off.")]
		public async Task Serbs(CommandContext ctx)
		{
			if (!await AllowedExecution(ctx))
				return;

			int count = 0;

			//List off all the servers
			StringBuilder servers = new StringBuilder();
			foreach(var g in ctx.Client.Guilds)
			{
				servers
					.Append(++count).Append(": ")
					.Append("(").Append(g.Key).Append(") ")
					.Append(g.Value.Name)
					.Append("\n");

			}

			await ctx.RespondAsync(embed: new ResponseBuilder(ctx).WithDescription("```" + servers.ToString() + "``` **" + count + "** servers listed"));
		}


		public async Task<bool> AllowedExecution(CommandContext ctx)
		{
			await ctx.TriggerTypingAsync();
			if (ctx.User.Id != LACHEE_UID)
			{
				await ctx.RespondAsync(embed: new ResponseBuilder(ctx).WithDescription("You are forbidden from accessing this command!").WithColor(DiscordColor.Red));
				return false;
			}

			return true;
		}
	}
}
