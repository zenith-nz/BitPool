using System;
using BitPool.Cryptography.BouncyCastle;
using BitPool.Cryptography.BouncyCastle.Parameters;
using BitPool.Cryptography.Digests;
using BitPool.Cryptography.MACs.BLAKE2B;

namespace BitPool.Cryptography.MACs
{
	public class Blake2BMac : BLAKE2BDigest, IMac, IMacWithSalt
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BitPool.Cryptography.MACs.Blake2BMac"/> class.
		/// </summary>
		/// <param name="size">Size of the MAC to produce.</param>
		/// <param name="bits">Whether <paramref name="size"/> is interpreted as bits or bytes. If true, bits.</param>
		public Blake2BMac (int size, bool bits) : this(size, bits, true)
		{
		}

		/// <summary>
		/// If using this constructor, be SURE to use Init() before doing anything 
		/// else with the object to avoid null reference exceptions!
		/// </summary>
		/// <param name="size">Size of the MAC to produce.</param>
		/// <param name="bits">Whether <paramref name="size"/> is interpreted as bits or bytes. If true, bits.</param>
		/// <param name="init"><param>
		internal Blake2BMac(int size, bool bits, bool init) : base(size, bits, init) {
		}

		#region IMac implementation

		public void Init (ICipherParameters parameters)
		{
			this.Init (parameters, null);
		}

		/// <summary>
		/// Initialise the MAC primitive with a key and/or salt and/or a tag.
		/// </summary>
		/// <param name="parameters">Parameter object that may comprise a key and/or salt. Key maximum 128 bytes, salt 16.</param>
		/// <param name="tag">Tag/personalisation to include in the IV for the MAC. 16 bytes maximum.</param>
		public void Init(ICipherParameters parameters, byte[] tag) {
			byte[] key = null, salt = null;
		    var keyParameter = parameters as KeyParameter;
		    if (keyParameter != null) key = keyParameter.GetKey();
		    var parametersWithSalt = parameters as ParametersWithSalt;
		    if (parametersWithSalt != null) salt = parametersWithSalt.GetSalt();
			this.Init (key, salt, tag);
		}

		public void Init(byte[] key, byte[] salt, byte[] tag) {
			byte[] keyBytes = null, saltBytes = null, tagBytes = null;

			if(key != null) {
				if(key.Length > 128) throw new ArgumentOutOfRangeException("key", "Key is longer than 128 bytes.");
				keyBytes = new byte[128];
				Array.Copy(key, keyBytes, key.Length);
			}

			if(salt != null) {
				if(salt.Length > 16) throw new ArgumentOutOfRangeException("salt", "Salt is longer than 16 bytes.");
				saltBytes = new byte[16];
				Array.Copy(salt, saltBytes, salt.Length);
			}

			if(tag != null) {
				if(tag.Length > 16) throw new ArgumentOutOfRangeException("tag", "Tag is longer than 16 bytes.");
				tagBytes = new byte[16];
                Array.Copy(tag, tagBytes, tag.Length);
			}

			var config = new Blake2BConfig () {
				Key = keyBytes,
				Salt = saltBytes,
				Personalization = tagBytes,
				OutputSizeInBytes = outputSize,
			};

			hasher = new Blake2BHasher (config);
		}

		public int GetMacSize ()
		{
			return this.GetDigestSize();
		}

		#endregion

        public void Init (byte[] key, byte[] salt) {
            this.Init(key, salt, null);
        }
    }
}

