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
		public Task<byte[]> GetAsync(string key, CancellationToken token) {
			if (key == null) throw new ArgumentNullException(nameof(key));

			var ret = CSRedis.QuickHelperBase.HashGet(key, "data");
			return Task.FromResult<byte[]>(string.IsNullOrEmpty(ret) ? null : Convert.FromBase64String(ret));
		}

		public void Set(string key, byte[] value, DistributedCacheEntryOptions options) {
			this.SetAsync(key, value, options, CancellationToken.None).Wait();
		}
		public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token) {
			if (key == null) throw new ArgumentNullException(nameof(key));
			if (value == null) throw new ArgumentNullException(nameof(value));
			if (options == null) throw new ArgumentNullException(nameof(options));

			var expire = options.AbsoluteExpiration.HasValue ? options.AbsoluteExpirationRelativeToNow.Value : options.SlidingExpiration ?? TimeSpan.FromMinutes(20);
			CSRedis.QuickHelperBase.HashSet(key, expire, "expire", expire.Ticks, "data", Convert.ToBase64String(value));
			return Task.Run(() => { });
		}

		public void Refresh(string key) {
			this.RefreshAsync(key, CancellationToken.None).Wait();
		}

		public Task RefreshAsync(string key, CancellationToken token) {
			if (key == null) throw new ArgumentNullException(nameof(key));

			if (long.TryParse(CSRedis.QuickHelperBase.HashGet(key, "expire"), out long expire) && expire > 0) CSRedis.QuickHelperBase.Expire(key, TimeSpan.FromTicks(expire));
			return Task.Run(() => { });
		}
		public void Remove(string key) {
			this.RemoveAsync(key, CancellationToken.None).Wait();
		}

		public Task RemoveAsync(string key, CancellationToken token) {
			if (key == null) throw new ArgumentNullException(nameof(key));

			CSRedis.QuickHelperBase.Remove(key);
			return Task.Run(() => { });
		}
	}
}
