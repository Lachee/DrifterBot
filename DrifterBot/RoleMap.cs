
using DrifterBot.Redis;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrifterBot
{
	public class RoleMap
	{
		public const string NAMESPACE = "DrifterBot:RoleMap:";

		private IRedisClient redis;
		public RoleMap(IRedisClient redis)
		{
			this.redis = redis;
		}

		public async Task RemoveMapping(DiscordGuild guild, DiscordEmoji emoji)
		{
			string keyEmoji = NAMESPACE + guild.Id + ":" + emoji.Id.ToString();
			string keyList = NAMESPACE + guild.Id + ":maps";

			await redis.ValueRemoveAsync(keyEmoji);
			await redis.SetRemoveAsync(keyList, emoji.Id.ToString());

		}
		public async Task UpdateMapping(DiscordGuild guild, DiscordEmoji emoji, DiscordRole role)
		{
			string keyEmoji = NAMESPACE + guild.Id + ":" + emoji.Id.ToString();
			string keyList = NAMESPACE + guild.Id + ":maps";

			await redis.SetAddAsync(keyList, emoji.Id.ToString());
			await redis.ValueSetAsync(keyEmoji, role.Id.ToString());
		}
		public async Task<DiscordRole> GetMapping(DiscordGuild guild, DiscordEmoji emoji)
		{
			string key = NAMESPACE + guild.Id + ":" + emoji.Id.ToString();

			string rolestr = await redis.ValueGetAsync(key);
			if (string.IsNullOrEmpty(rolestr))
				return null;

			ulong roleid;
			if (!ulong.TryParse(rolestr, out roleid))
				return null;

			return guild.GetRole(roleid);
		}
		public async Task<Dictionary<DiscordEmoji, DiscordRole>> GetAllMappings(DiscordGuild guild)
		{
			//Get all items in the mapping
			string keyList = NAMESPACE + guild.Id + ":maps";
			string[] ids = await redis.SetGetAllAsync(keyList);

			//Prepare the dictionary
			Dictionary<DiscordEmoji, DiscordRole> mapping = new Dictionary<DiscordEmoji, DiscordRole>();

			//Iterate over every id, getting the emoji and hte role it maps too
			for(int i = 0; i < ids.Length; i++)
			{
				ulong ID;
				
				//Get the ID, making sure its not bad
				if (!ulong.TryParse(ids[i], out ID))
					continue;

				//Get the emoji it belongs too and then the mapping
				DiscordEmoji emoji = await guild.GetEmojiAsync(ID);
				DiscordRole role = await GetMapping(guild, emoji);

				//Add to the dictionary
				mapping.Add(emoji, role);
			}

			return mapping;
		}

		public async Task UpdateMessage(DiscordGuild guild, DiscordMessage message)
		{
			string key = NAMESPACE + guild.Id + ":msgs";
			await redis.SetAddAsync(key, message.Id.ToString());
		}
		public async Task RemoveMessage(DiscordGuild guild, DiscordMessage message)
		{
			string key = NAMESPACE + guild.Id + ":msgs";
			await redis.SetRemoveAsync(key, message.Id.ToString());
		}
		public async Task<bool> IsMessageMapped(DiscordGuild guild, DiscordMessage message)
		{
			string key = NAMESPACE + guild.Id + ":msgs";
			return await redis.SetContainsAsync(key, message.Id.ToString());
		}
		public async Task<ulong[]> GetMappedMessages(DiscordGuild guild)
		{
			string key = NAMESPACE + guild.Id + ":msgs";

			string[] msgs = await redis.SetGetAllAsync(key);
			ulong[] ids = new ulong[msgs.Length];

			for (int i = 0; i < msgs.Length; i++)
				ids[i] = ulong.Parse(msgs[i]);

			return ids;
		}
	}
}
