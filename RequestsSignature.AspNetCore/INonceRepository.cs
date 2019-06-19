using System.Threading.Tasks;

namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// Manages the storage for requests signature nonce.
    /// </summary>
    public interface INonceRepository
    {
        /// <summary>
        /// Adds a new incoming nonce.
        /// </summary>
        /// <param name="nonce">The nonce to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task Add(string nonce);

        /// <summary>
        /// Indicates whether the nonce has been stored previously.
        /// </summary>
        /// <param name="nonce">The nonce to check.</param>
        /// <returns>true if the nonce was stored previously, false otherwise.</returns>
        Task<bool> Exists(string nonce);
    }
}
