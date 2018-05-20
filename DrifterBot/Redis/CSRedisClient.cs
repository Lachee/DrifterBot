using CSRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrifterBot.Redis
{
	public class CSRedisClient : IRedisClient
	{
		private RedisClient client;
		public CSRedisClient(string host)
		{
			this.client = new CSRedis.RedisClient(host);
		}

		public async Task<string> SetDatabase(int index) => await client.SelectAsync(index);

		public async Task<T> HasGetAsync<T>(string key) where T : class => await client.HGetAllAsync<T>(key);
		public async Task<string> HashSetAsync<T>(string key, T obj) where T : class => await client.HMSetAsync<T>(key, obj);

		public async Task Initialize() => await Task.Delay(0);

		public async Task<long> SetAddAsync(string key, string value) => await client.SAddAsync(key, value);
		public async Task<long> SetAddAsync(string key, params string[] values) => await client.SAddAsync(key, values);
		public async Task<string[]> SetGetAllAsync(string key) => await client.SMembersAsync(key);
		public async Task<string> SetGetRandomAsync(string key) => await client.SRandMemberAsync(key);
		public async Task<long> SetRemoveAsync(string key, string value) => await client.SRemAsync(key, value);
		public async Task<long> SetRemoveAsync(string key, params string[] values) => await client.SRemAsync(key, values);
		public async Task<bool> SetContainsAsync(string key, string value) => await client.SIsMemberAsync(key, value);

		public void Dispose()
		{
			this.client.Dispose();
		}

		public async Task<string> ValueGetAsync(string key) => await client.GetAsync(key);
		public async Task ValueSetAsync(string key, string value) => await client.SetAsync(key, value);
		public async Task ValueRemoveAsync(string key) => await client.DelAsync(key);
	}
}
