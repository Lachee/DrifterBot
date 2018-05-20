using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrifterBot.Helper
{
	class ResponseBuilder : DiscordEmbedBuilder
	{
		public string AvatarAPI { get; set; } = "https://d.lu.je/avatar/";

		public ResponseBuilder (CommandContext ctx, Exception e) : this(ctx)
		{
			this.Description = string.Format("An exception has occured during the {0} command: ```{1}``` **Stacktrace** ```{2}```", ctx.Command.Name, e.Message, e.StackTrace);
			this.Color = DiscordColor.Red;

		}
		public ResponseBuilder(CommandContext ctx) : this(ctx, ctx.Command.Name + " Response") { }
		public ResponseBuilder(CommandContext ctx, string responseName) : base()
		{
			if (responseName.Length > 1)
			{
				var c = responseName[0];
				responseName = c.ToString().ToUpperInvariant() + responseName.Substring(1);
			}

			Footer = new DiscordEmbedBuilder.EmbedFooter()
			{
				Text = "Response to " + ctx.User.Username,
				IconUrl = AvatarAPI + ctx.User.Id
			};
			Author = new DiscordEmbedBuilder.EmbedAuthor()
			{
				Name = responseName,
				Url = "http://hyperlight.chickatrice.net/",
				IconUrl = "https://i.imgur.com/KLNIH31.png"
			};
			Color = new DiscordColor(12794879);
			Timestamp = DateTimeOffset.Now;
		}		
	}
}
