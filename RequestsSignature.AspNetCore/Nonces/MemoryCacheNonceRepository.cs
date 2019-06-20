using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace RequestsSignature.AspNetCore.Nonces
{
    /// <summary>
    /// <see cref="INonceRepository"/> implementation that uses <see cref="IMemoryCache"/>.
    /// </summary>
    public class MemoryCacheNonceRepository : INonceRepository
    {
        /// <summary>
        /// Gets the default key prefix.
        /// </summary>
        public const string DefaultKeyPrefix = "nonce-";

        private readonly IOptionsMonitor<RequestsSignatureOptions> _optionsMonitor;
        private readonly IMemoryCache _cache;
        private readonly string _keyPrefix;
        private RequestsSignatureOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheNonceRepository"/> class.
        /// </summary>
        /// <param name="cache">The <see cref="IMemoryCache"/>.</param>
        /// <param name="options">The <see cref="RequestsSignatureOptions"/>.</param>
        /// <param name="keyPrefix">The prefix pre-pended to nonce when stored.</param>
        public MemoryCacheNonceRepository(
            IOptionsMonitor<RequestsSignatureOptions> options,
            IMemoryCache cache,
            string keyPrefix = DefaultKeyPrefix)
        {
            _optionsMonitor = options ?? throw new ArgumentNullException(nameof(options));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _keyPrefix = keyPrefix ?? throw new ArgumentNullException(nameof(keyPrefix));
            _optionsMonitor.OnChange(OnOptionsChange);
            OnOptionsChange(_optionsMonitor.CurrentValue);
        }

        /// <inheritdoc />
        public async Task Add(string nonce)
            => _cache.Set(_keyPrefix + nonce, string.Empty, _options.ClockSkew);

        /// <inheritdoc />
        public async Task<bool> Exists(string nonce)
            => _cache.TryGetValue(_keyPrefix + nonce, out var _);

        private void OnOptionsChange(RequestsSignatureOptions options)
            => _options = options;
    }
}
