﻿using System;
using System.Linq;
using System.Text;
using PlatoCore.Text.Abstractions;

namespace PlatoCore.Text
{

    /// <summary>
    /// Generates a simple random alpha numeric string configurable via the Configure method.
    /// This should NOT be used for any type of encryption or hashing operations.
    /// </summary>
    public class KeyGenerator : IKeyGenerator
    {

        static readonly object SyncLock = new object();
        private readonly Random _rnd;
        private readonly KeyGeneratorOptions _options;

        public KeyGenerator()
        {
            _options = new KeyGeneratorOptions();
            lock (SyncLock)
            {
                _rnd = new Random();
            }
        }

        public string GenerateKey()
        {
            // Use defaults
            return GenerateKey(opts => {});
        }

        public string GenerateKey(Action<KeyGeneratorOptions> configure)
        {
            configure(_options);
            return GenerateKeyInternal();
        }

        string GenerateKeyInternal()
        {

            var sb = new StringBuilder(_options.MaxLength);
            for (var i = 0; i <= _options.Iterations; i++)
            {
                sb.Append(RandomAlphaNumeric(_rnd.Next(_options.MinLengthPerIteration, _options.MaxLengthPerIteration)));
            }

            var output = sb.ToString();

            // Restrict to a specific length
            if (_options.MaxLength > 0)
            {
                if (output.Length > _options.MaxLength)
                {
                    output = output.Substring(0, _options.MaxLength);
                }
            }

            // Add our unique identifier somewhere within the response
            if (!String.IsNullOrEmpty(_options.UniqueIdentifier))
            {
                var index = _rnd.Next(0, output.Length);
                return
                    output.Substring(0, index) +
                    _options.UniqueIdentifier +
                    output.Substring(index);
            }

            return output;

        }

        string RandomAlphaNumeric(int length)
        {

            var sb = new System.Text.StringBuilder(length);
            for (var i = 1; i <= length; i++)
            {
                var charIndex = 0;
                do
                {
                    charIndex = _rnd.Next(48, 123);
                } while (!((charIndex >= 48 && charIndex <= 57) ||
                           (charIndex >= 65 && charIndex <= 90) ||
                           (charIndex >= 97 && charIndex <= 122)));

                var character = System.Convert.ToChar(charIndex);
                if (_options.SuppressCharacters != null)
                {
                    sb.Append(_options.SuppressCharacters.Contains(character)
                        ? System.Convert.ToChar(charIndex + 3)
                        : character);
                }
                else
                {
                    sb.Append(character);
                }
            }

            return sb.ToString();

        }

    }

}
