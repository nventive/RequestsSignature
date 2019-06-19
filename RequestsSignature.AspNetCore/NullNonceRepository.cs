using System.Threading.Tasks;

namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// <see cref="INonceRepository"/> implementation that stores nothing
    /// and always returns false for existence check.
    /// </summary>
    public class NullNonceRepository : INonceRepository
    {
        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static readonly NullNonceRepository Instance = new NullNonceRepository();

        private NullNonceRepository()
        {
        }

        /// <inheritdoc />
        public Task Add(string nonce) => Task.CompletedTask;

        /// <inheritdoc />
        public Task<bool> Exists(string nonce) => Task.FromResult(false);
    }
}
