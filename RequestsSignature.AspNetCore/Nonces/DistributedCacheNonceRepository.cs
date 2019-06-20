using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace RequestsSignature.AspNetCore.Nonces
{
    /// <summary>
    /// <see cref="INonceRepository"/> implementation that uses <see cref="IDistributedCache"/>.
    /// </summary>
    public class DistributedCacheNonceRepository : INonceRepository
    {
        /// <summary>
        /// Gets the default key prefix.
        /// </summary>
        public const string DefaultKeyPrefix = "nonce-";

        private static readonly byte[] CacheValue = Encoding.UTF8.GetBytes(string.Empty);

        private readonly IOptionsMonitor<RequestsSignatureOptions> _optionsMonitor;
        private readonly IDistributedCache _cache;
        private readonly string _keyPrefix;

        private RequestsSignatureOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedCacheNonceRepository"/> class.
        /// </summary>
        /// <param name="cache">The <see cref="IDistributedCache"/>.</param>
        /// <param name="options">The <see cref="RequestsSignatureOptions"/>.</param>
        /// <param name="keyPrefix">The prefix pre-pended to nonce when stored.</param>
        public DistributedCacheNonceRepository(
            IOptionsMonitor<RequestsSignatureOptions> options,
            IDistributedCache cache,
            string keyPrefix = DefaultKeyPrefix)
        {
            _optionsMonitor = options ?? throw new ArgumentNullException(nameof(options));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _keyPrefix = keyPrefix ?? throw new ArgumentNullException(nameof(keyPrefix));
            _optionsMonitor.OnChange(OnOptionsChange);
            OnOptionsChange(_optionsMonitor.CurrentValue);
        }

        /// <inheritdoc />
        public Task Add(string nonce)
            => _cache.SetAsync(
                nonce,
                CacheValue,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _options.ClockSkew });

        /// <inheritdoc />
        public async Task<bool> Exists(string nonce)
            => (await _cache.GetAsync(nonce)) != null;

        private void OnOptionsChange(RequestsSignatureOptions options)
            => _options = options;
    }
}
