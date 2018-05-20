using DrifterBot.Helper;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;

namespace DrifterBot.Commands
{
	public class Voice
	{
		[Command("ping")]
		[Description("Fetches the current IP address of Hyperlight. Only to be used if DNS failed to update.")]
		public async Task IP(CommandContext ctx)
		{
			await ctx.TriggerTypingAsync();

			var embed = new ResponseBuilder(ctx).WithDescription("```PONG```");
			await ctx.RespondAsync(embed: embed);
		}

		[Command("say")]
		public async Task Say(CommandContext ctx, [RemainingText] string say)
		{
			var vnext = ctx.Client.GetVoiceNextClient();

			var vnc = vnext.GetConnection(ctx.Guild);
			if (vnc == null)
			{				
				var chn = ctx.Member?.VoiceState?.Channel;
				if (chn == null)
					throw new InvalidOperationException("You need to be in a voice channel.");

				vnc = await vnext.ConnectAsync(chn);
			}

			await ctx.RespondAsync(embed: new ResponseBuilder(ctx).WithDescription("👌 Saying `" + say + "`"));
			await Say(vnc, say);
			
		}
		
		public static async Task Say(VoiceNextConnection vnc, string say)
		{
			await vnc.SendSpeakingAsync(true); // send a speaking indicator

			using (MemoryStream stream = new MemoryStream())
			{
				var info = new SpeechAudioFormatInfo(48000, AudioBitsPerSample.Sixteen, AudioChannel.Stereo);
				using (SpeechSynthesizer synth = new SpeechSynthesizer())
				{

					//synth.SetOutputToAudioStream(stream, info);
					synth.SetOutputToAudioStream(stream, info);

					//var t = synth.GetInstalledVoices();
					//synth.SelectVoice(t.First().VoiceInfo.Name);
					synth.Speak(say);
					synth.SetOutputToNull();
				}

				//await vnc.SendAsync(stream.ToArray(), 20, info.BitsPerSample);

				stream.Seek(0, SeekOrigin.Begin);

				Console.WriteLine("Format: {0}", info.EncodingFormat);
				Console.WriteLine("BitRate: {0}", info.BitsPerSample);
				Console.WriteLine("Block Alignment: {0}", info.BlockAlign);
				Console.WriteLine("Samples per second: {0}", info.SamplesPerSecond);

				var buff = new byte[3840];
				var br = 0;
				while ((br = stream.Read(buff, 0, buff.Length)) > 0)
				{
					if (br < buff.Length) // not a full sample, mute the rest
						for (var i = br; i < buff.Length; i++)
							buff[i] = 0;


					await vnc.SendAsync(buff, 20, info.BitsPerSample);
				}

			}

			await vnc.SendSpeakingAsync(false); // we're not speaking anymore
		}

		[Command("play")]
		public async Task Play(CommandContext ctx, [RemainingText] string file)
		{
			var vnext = ctx.Client.GetVoiceNextClient();

			var vnc = vnext.GetConnection(ctx.Guild);
			if (vnc == null)
			{
				//await ctx.RespondAsync(embed: new ResponseBuilder(ctx).WithDescription("Could not connect in this guild"));
				await Join(ctx);
				await Task.Delay(1000);

				vnc = vnext.GetConnection(ctx.Guild);
				if (vnc == null)
				{
					await ctx.RespondAsync(embed: new ResponseBuilder(ctx).WithDescription("Failed to join the connection succesfull. Please make sure you use the `join` command."));
					return;
				}
			}

			if (!File.Exists(file))
			{
				await ctx.RespondAsync(embed: new ResponseBuilder(ctx).WithDescription("Could not find the file `" + file + "`"));
				return;
			}

			await ctx.RespondAsync(embed: new ResponseBuilder(ctx).WithDescription("👌 Playing the file `" + file + "`"));			
			await vnc.SendSpeakingAsync(true); // send a speaking indicator

			try
			{
				var psi = new ProcessStartInfo
				{
					FileName = "c:/fmpeg/bin/ffmpeg.exe",
					Arguments = $@"-i ""{file}"" -ac 2 -f s16le -ar 48000 pipe:1",
					RedirectStandardOutput = true,
					UseShellExecute = false
				};
				var ffmpeg = Process.Start(psi);
				var ffout = ffmpeg.StandardOutput.BaseStream;

				var buff = new byte[3840];
				var br = 0;
				while ((br = ffout.Read(buff, 0, buff.Length)) > 0)
				{
					if (br < buff.Length) // not a full sample, mute the rest
						for (var i = br; i < buff.Length; i++)
							buff[i] = 0;

					await vnc.SendAsync(buff, 20);
				}

			}
			catch (Exception e)
			{
				await ctx.RespondAsync(embed: new ResponseBuilder(ctx, e));
				Console.WriteLine("E");
			}

			await vnc.SendSpeakingAsync(false); // we're not speaking anymore
		}


		[Command("join")]
		public async Task Join(CommandContext ctx)
		{
			var vnext = ctx.Client.GetVoiceNextClient();

			var vnc = vnext.GetConnection(ctx.Guild);
			if (vnc != null)
				throw new InvalidOperationException("Already connected in this guild.");

			var chn = ctx.Member?.VoiceState?.Channel;
			if (chn == null)
				throw new InvalidOperationException("You need to be in a voice channel.");

			vnc = await vnext.ConnectAsync(chn);
			//await ctx.RespondAsync("👌");
		}

		[Command("leave")]
		public async Task Leave(CommandContext ctx)
		{
			var vnext = ctx.Client.GetVoiceNextClient();

			var vnc = vnext.GetConnection(ctx.Guild);
			if (vnc == null)
				throw new InvalidOperationException("Not connected in this guild.");

			vnc.Disconnect();
			await ctx.RespondAsync("👌");
		}
	}
}
