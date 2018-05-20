using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrifterBot.Redis
{
	public interface IRedisClient : IDisposable
	{
		Task Initialize();

		Task<string> HashSetAsync<T>(string key, T obj) where T : class;
		Task<T> HasGetAsync<T>(string key) where T : class;

		Task<long> SetAddAsync(string key, string value);
		Task<long> SetAddAsync(string key, params string[] values);
		Task<long> SetRemoveAsync(string key, string value);
		Task<long> SetRemoveAsync(string key, params string[] values);
		Task<bool> SetContainsAsync(string key, string value);

		Task<string> SetGetRandomAsync(string key);
		Task<string[]> SetGetAllAsync(string key);

		Task<string> ValueGetAsync(string key);
		Task ValueSetAsync(string key, string value);
		Task ValueRemoveAsync(string key);
	}
}
