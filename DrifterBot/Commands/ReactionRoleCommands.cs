using DrifterBot.Helper;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrifterBot.Commands
{
	[Group("rolemap")]
	[Description("Handles mapping emoji reactions to roles")]
	public class ReactionRoleCommands
	{
		[Command("add")]
		[Description("maps a emoji reaction to a role")]
		[RequirePermissions(DSharpPlus.Permissions.ManageRoles | DSharpPlus.Permissions.ManageEmojis | DSharpPlus.Permissions.ManageGuild)]
		public async Task AddRole(CommandContext ctx,
			[Description("The emoji the user must react with")] DiscordEmoji emoji,
			[Description("The role to be given when the emoji is reacted")] DiscordRole role)
		{
			//Start typing and add the mapping
			await ctx.TriggerTypingAsync();
			await Drifter.Instance.RoleMap.UpdateMapping(ctx.Guild, emoji, role);

			await ctx.RespondAsync(embed: new ResponseBuilder(ctx).WithDescription("Succesfully **created** the mapping:\n " + Formatter.Emoji(emoji) + " :: " + Formatter.Mention(role)));
		}

		[Command("remove")]
		[Description("removes a mapping")]
		[RequirePermissions(DSharpPlus.Permissions.ManageRoles | DSharpPlus.Permissions.ManageEmojis | DSharpPlus.Permissions.ManageGuild)]
		public async Task RemoveRole(CommandContext ctx,
		   [Description("The emoji to remove")] DiscordEmoji emoji)
		{
			//Start typing and add the mapping
			await ctx.TriggerTypingAsync();
			await Drifter.Instance.RoleMap.RemoveMapping(ctx.Guild, emoji);

			await ctx.RespondAsync(embed: new ResponseBuilder(ctx).WithDescription("Succesfully **removed** the mapping for the following emoji:\n" + Formatter.Emoji(emoji)));
		}

		[Command("list")]
		[Description("lists all mappings")]
		[RequirePermissions(DSharpPlus.Permissions.ManageRoles | DSharpPlus.Permissions.ManageEmojis | DSharpPlus.Permissions.ManageGuild)]
		public async Task ListRoles(CommandContext ctx)
		{
			//Start typing and add the mapping
			await ctx.TriggerTypingAsync();

			//Get all mappings
			var mappings = await Drifter.Instance.RoleMap.GetAllMappings(ctx.Guild);
			StringBuilder text = new StringBuilder();

			foreach (var kp in mappings)
			{
				text.Append(Formatter.Emoji(kp.Key))
					.Append(" :: ")
					.Append(Formatter.Mention(kp.Value))
					.AppendLine();
			}

			//Print it out
			await ctx.RespondAsync(embed: new ResponseBuilder(ctx).WithDescription("Here is the current emoji mapping:\n" + text.ToString()));
		}


		[Command("addmsg")]
		[Description("adds a message")]
		[RequirePermissions(DSharpPlus.Permissions.ManageRoles | DSharpPlus.Permissions.ManageEmojis | DSharpPlus.Permissions.ManageGuild)]
		public async Task AddMessage(CommandContext ctx, DiscordMessage message)
		{
			//Start typing and add the mapping
			await ctx.TriggerTypingAsync();

			//Get all mappings
			await ctx.TriggerTypingAsync();
			await Drifter.Instance.RoleMap.UpdateMessage(ctx.Guild, message);

			await ctx.RespondAsync(embed: new ResponseBuilder(ctx).WithDescription("Succesfully **created** the mapping for message `" + message.Id + "`"));
		}

		[Command("remmsg")]
		[Description("adds a message")]
		[RequirePermissions(DSharpPlus.Permissions.ManageRoles | DSharpPlus.Permissions.ManageEmojis | DSharpPlus.Permissions.ManageGuild)]
		public async Task RemoveMessage(CommandContext ctx, DiscordMessage message)
		{
			//Start typing and add the mapping
			await ctx.TriggerTypingAsync();

			//Get all mappings
			await ctx.TriggerTypingAsync();
			await Drifter.Instance.RoleMap.RemoveMessage(ctx.Guild, message);

			await ctx.RespondAsync(embed: new ResponseBuilder(ctx).WithDescription("Succesfully **removed** the mapping for message `" + message.Id + "`"));
		}

		[Command("messages")]
		[Description("lists all messages")]
		[RequirePermissions(DSharpPlus.Permissions.ManageRoles | DSharpPlus.Permissions.ManageEmojis | DSharpPlus.Permissions.ManageGuild)]
		public async Task ListMessages(CommandContext ctx)
		{
			//Start typing and add the mapping
			await ctx.TriggerTypingAsync();

			//Get all mappings
			var mappings = await Drifter.Instance.RoleMap.GetMappedMessages(ctx.Guild);
			StringBuilder text = new StringBuilder();

			text.Append("```");
			foreach (var kp in mappings)
			{
				text.AppendLine(kp.ToString());
			}
			text.Append("```");

			//Print it out
			await ctx.RespondAsync(embed: new ResponseBuilder(ctx).WithDescription("Here is the current emoji mapping: " + text.ToString()));
		}


	}
}
