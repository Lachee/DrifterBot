using DrifterBot.Commands;
using DrifterBot.Redis;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrifterBot
{
	class Drifter
	{
		public static Drifter Instance => _instance;
		private static Drifter _instance;

		#region Discord
		private VoiceNextClient Voice => Program.voice;
		public DiscordClient Discord => _discord;
		public CommandsNextModule Commands => _commands;
		public InteractivityModule Interactivity => _interactivity;

		private DiscordClient _discord;
		private CommandsNextModule _commands;
		private InteractivityModule _interactivity;
		#endregion

		public RoleMap RoleMap => _rolemap;
		private RoleMap _rolemap;
		private IRedisClient redis;

		public Drifter(DiscordClient client)
		{
			_instance = this;
			this._discord = client;
			this.redis = new CSRedisClient("localhost");
			this.redis.SetDatabase(1).Wait();

			this._rolemap = new RoleMap(this.redis);

			_interactivity = Discord.UseInteractivity(new InteractivityConfiguration() { });
			_commands = Discord.UseCommandsNext(new CommandsNextConfiguration() { StringPrefix = ";" });
			Commands.RegisterCommands<Commands.AdminCommands>();
			Commands.RegisterCommands<Commands.MemeCommands>();
			Commands.RegisterCommands<Commands.UtilityCommands>();
			//Commands.RegisterCommands<Commands.VoiceCommand>();
			Commands.RegisterCommands<Commands.ReactionRoleCommands>();

			client.MessageReactionAdded += OnReactionAdded;
			client.MessageReactionRemoved += OnReactionRemoved;
			//client.VoiceStateUpdated += OnVoiceChange;
		}

		private async Task OnReactionRemoved(DSharpPlus.EventArgs.MessageReactionRemoveEventArgs e)
		{
			var guild = e.Channel.Guild;
			if (await RoleMap.IsMessageMapped(guild, e.Message))
			{
				//Get the mapping
				DiscordRole role = await RoleMap.GetMapping(guild, e.Emoji);
				if (role != null)
				{
					var member = await guild.GetMemberAsync(e.User.Id);
					await guild.RevokeRoleAsync(member, role, "Response to reaction");					
				}
			}
		}
		private async Task OnReactionAdded(DSharpPlus.EventArgs.MessageReactionAddEventArgs e)
		{
			try
			{
				var guild = e.Channel.Guild;
				if (await RoleMap.IsMessageMapped(guild, e.Message))
				{
					//Get the mapping
					DiscordRole role = await RoleMap.GetMapping(guild, e.Emoji);
					if (role != null)
					{
						var member = await guild.GetMemberAsync(e.User.Id);
						await guild.GrantRoleAsync(member, role, "Response to reaction");
					}
				}
			}catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
		}

		public async Task Initialize()
		{
			await redis.Initialize();
			await Task.Delay(0);
		}
		

		/*
		private Dictionary<ulong, VoiceState> _states = new Dictionary<ulong, VoiceState>();
		private async Task OnVoiceChange(DSharpPlus.EventArgs.VoiceStateUpdateEventArgs e)
		{
			try
			{
				//Get the voice connection for this server
				var vnc = voice.GetConnection(e.Guild);
				if (vnc == null) return;

				//Add this user to the tracking
				VoiceState state;
				if (!_states.TryGetValue(e.User.Id, out state))
				{
					//We havn't got a state yet, better create one
					state = new VoiceState()
					{
						User = e.User,
						Guild = e.Guild,
						Channel = null
					};

					Console.WriteLine("Adding new state to the dictionary for {0}", state.User);
					_states.Add(state.User.Id, state);
				}

				//Set the message
				string joinMessage = "{0} joined the channel";
				string leaveMessage = "{0} left the channel";
				string message = null;

				state.Member = await e.Guild.GetMemberAsync(e.User.Id);

				//Check for a join
				if (state.Channel != e.Channel)
				{
					Console.WriteLine("{0} -> {1}", state.Channel, e.Channel);
					if (state.Channel == null || e.Channel == vnc.Channel)
					{
						message = joinMessage;
						Console.WriteLine("{0} has joined voice {1}", state.User, e.Channel);
					}
					else
					{
						message = leaveMessage;
						Console.WriteLine("{0} has left voice {1}", state.User, state.Channel);
					}
				}
				else
				{
					Console.WriteLine("{0} has a generic state change", state.User);
				}

				//Update the state
				state.Channel = e.Channel;

				//IF we have a valid state change (message not null), say the message
				if (!string.IsNullOrEmpty(message))
				{
					if (vnc != null)
					{
						//Say the message
						string name = state.Member != null && string.IsNullOrEmpty(state.Member.Nickname) ? state.User.Username : state.Member.Nickname;
						await Voice.Say(vnc, string.Format(message, name));
					}
				}
			}catch(Exception exeception)
			{
				Console.WriteLine(exeception);
			}
		}
	}

	class VoiceState
	{
		public DiscordUser User { get; set; }
		public DiscordGuild Guild { get; set; }
		public DiscordChannel Channel { get; set; }
		public DiscordMember Member { get; set; }

		public bool IsConnected { get { return Channel != null; } }

		public VoiceState() { }
		public VoiceState(VoiceState state)
		{
			this.User = state.User;
			this.Channel = state.Channel;
			this.Guild = state.Guild;
		}

	}
	*/
	}
}
