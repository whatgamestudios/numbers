// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;

using System;
using System.Security.Cryptography;

namespace FourteenNumbers {

    /**
    * Deterministic "random" sequence of numbers generator, based on the game day,
    * the game being played, the iteration of the game, and the count offset within 
    * the game. Using this class should ensure people all around the world have to 
    * tackle the same problem each day.
    */
    public class SeedGen{

        /**
        * Generate a seed value to generate values from, based on the number of days since 
        * the start of the game epoch, the game being played, and the iteration of the game. 
        *
        * @param game The game being played.
        * @param iteration The iteration of the game being played.
        * @returns A seed value to be presented to calls to GetNextValue.
        */
        public static byte[] GenerateSeed(uint game, uint iteration) {
            uint daysSinceStart = Timeline.GameDay();
            byte[] d = getBytesBigEndian(daysSinceStart);
            byte[] g = getBytesBigEndian(game);
            byte[] i = getBytesBigEndian(iteration);

            byte[] derivationPath = new byte[d.Length + g.Length + i.Length];
            System.Buffer.BlockCopy(d, 0, derivationPath, 0, d.Length);
            System.Buffer.BlockCopy(g, 0, derivationPath, d.Length, g.Length);
            System.Buffer.BlockCopy(i, 0, derivationPath, d.Length + g.Length, i.Length);
            // Debug.Log("Derivation path[]: " + HexDump.Dump(derivationPath));

            HashAlgorithm sha = SHA256.Create();
            byte[] seed = sha.ComputeHash(derivationPath);
            // Debug.Log("Seed: " + HexDump.Dump(seed));
            return seed;
        }


        /**
        * Calculate the next value in the sequence.
        *
        * @param seed Value generated by GenerateSeed
        * @param count The value from the sequence to return. This should increase from 0 as a game is played.
        * @param mod One more than the maximum value that should be returned.
        */
        public static uint GetNextValue(byte[] seed, uint count, uint mod) {
            byte[] c = getBytesBigEndian(count);

            byte[] combined = new byte[seed.Length + c.Length];
            System.Buffer.BlockCopy(seed, 0, combined, 0, seed.Length);
            System.Buffer.BlockCopy(c, 0, combined, seed.Length, c.Length);
            // Debug.Log("Combined: " + HexDump.Dump(combined));

            HashAlgorithm sha = SHA256.Create();
            byte[] raw = sha.ComputeHash(combined);
            // Debug.Log("Raw: " + HexDump.Dump(raw));
            int intLen = 4;
            byte[] raw2 = new byte[intLen];
            System.Buffer.BlockCopy(raw, raw.Length - intLen, raw2, 0, intLen);
            uint raw1 = bytesToUIntBigEndian(raw);
            return raw1 % mod;
        }

        private static byte[] getBytesBigEndian(uint a) {
            byte[] b = BitConverter.GetBytes(a);
            if (BitConverter.IsLittleEndian) {
                Array.Reverse(b);
            }
            return b;
        }

        private static uint bytesToUIntBigEndian(byte[] b) {
            if (BitConverter.IsLittleEndian) {
                Array.Reverse(b);
            }
            return BitConverter.ToUInt32(b, 0);
        }
        
    }
}