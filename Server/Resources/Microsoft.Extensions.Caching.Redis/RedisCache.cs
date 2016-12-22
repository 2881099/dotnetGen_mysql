using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text;

namespace Microsoft.Extensions.Caching.Redis {
	public class RedisCache : IDistributedCache {

		public byte[] Get(string key) {
			return this.GetAsync(key).Result;
		}
		public Task<byte[]> GetAsync(string key) {
			if (key == null) throw new ArgumentNullException(nameof(key));

			var ret = CSRedis.QuickHelperBase.HashGet(key, "data");
			return Task.FromResult<byte[]>(string.IsNullOrEmpty(ret) ? null : Convert.FromBase64String(ret));
		}

		public void Set(string key, byte[] value, DistributedCacheEntryOptions options) {
			this.SetAsync(key, value, options).Wait();
		}
		public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options) {
			if (key == null) throw new ArgumentNullException(nameof(key));
			if (value == null) throw new ArgumentNullException(nameof(value));
			if (options == null) throw new ArgumentNullException(nameof(options));

			var expire = options.AbsoluteExpiration.HasValue ? options.AbsoluteExpirationRelativeToNow.Value : options.SlidingExpiration ?? TimeSpan.FromMinutes(20);
			CSRedis.QuickHelperBase.HashSet(key, expire, "expire", expire.Ticks, "data", Convert.ToBase64String(value));
			return Task.Run(() => { });
		}

		public void Refresh(string key) {
			this.RefreshAsync(key).Wait();
		}

		public Task RefreshAsync(string key) {
			if (key == null) throw new ArgumentNullException(nameof(key));

			long expire;
			if (long.TryParse(CSRedis.QuickHelperBase.HashGet(key, "expire"), out expire) && expire > 0) CSRedis.QuickHelperBase.Expire(key, TimeSpan.FromTicks(expire));
			return Task.Run(() => { });
		}
		public void Remove(string key) {
			this.RemoveAsync(key).Wait();
		}

		public Task RemoveAsync(string key) {
			if (key == null) throw new ArgumentNullException(nameof(key));

			CSRedis.QuickHelperBase.Remove(key);
			return Task.Run(() => { });
		}
	}
}
