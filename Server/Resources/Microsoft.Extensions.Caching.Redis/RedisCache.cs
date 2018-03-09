using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text;
using System.Threading;

namespace Microsoft.Extensions.Caching.Redis {
	public class RedisSuperCache : IDistributedCache {

		public byte[] Get(string key) {
			return this.GetAsync(key, CancellationToken.None).Result;
		}
		async public Task<byte[]> GetAsync(string key, CancellationToken token) {
			if (key == null) throw new ArgumentNullException(nameof(key));

			var ret = await CSRedis.QuickHelperBase.HashGetAsync(key, "data");
			return string.IsNullOrEmpty(ret) ? null : Convert.FromBase64String(ret);
		}

		public void Set(string key, byte[] value, DistributedCacheEntryOptions options) {
			this.SetAsync(key, value, options, CancellationToken.None).Wait();
		}
		async public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token) {
			if (key == null) throw new ArgumentNullException(nameof(key));
			if (value == null) throw new ArgumentNullException(nameof(value));
			if (options == null) throw new ArgumentNullException(nameof(options));

			var expire = options.AbsoluteExpiration.HasValue ? options.AbsoluteExpirationRelativeToNow.Value : options.SlidingExpiration ?? TimeSpan.FromMinutes(20);
			await CSRedis.QuickHelperBase.HashSetAsync(key, expire, "expire", expire.Ticks, "data", Convert.ToBase64String(value));
		}

		public void Refresh(string key) {
			this.RefreshAsync(key, CancellationToken.None).Wait();
		}

		async public Task RefreshAsync(string key, CancellationToken token) {
			if (key == null) throw new ArgumentNullException(nameof(key));

			if (long.TryParse(await CSRedis.QuickHelperBase.HashGetAsync(key, "expire"), out long expire) && expire > 0) await CSRedis.QuickHelperBase.ExpireAsync(key, TimeSpan.FromTicks(expire));
		}
		public void Remove(string key) {
			this.RemoveAsync(key, CancellationToken.None).Wait();
		}

		async public Task RemoveAsync(string key, CancellationToken token) {
			if (key == null) throw new ArgumentNullException(nameof(key));

			await CSRedis.QuickHelperBase.RemoveAsync(key);
		}
	}
}
