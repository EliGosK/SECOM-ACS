using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Text
{
    public static class TextGenerator
    {
        private static string PASSWORD_CHARS_NUMERIC = "0123456789";
        private static string PASSWORD_CHARS_UCASE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string PASSWORD_CHARS_LCASE = "abcdefghijklmnopqrstuvwxyz";
        private static string PASSWORD_CHARS_SPECIAL = "*{}@#$%<>-+_[]^!|";

        public static string Generate(int length = 8)
        {
            return Generate(length, length);
        }

        public static string Generate(int min,int max)
        {
            var options = new GenerateOptions(min,max);
            return Generate(options);
        }

        public static string GenerateNumber(int length)
        {
            return GenerateNumber(length, length);
        }

        public static string GenerateNumber(int min, int max)
        {
            var options = new GenerateOptions(min, max) { CharacterModes = GenerateCharacterModes.Numbers };
            return Generate(options);
        }

        public static string GenerateUpperCharacter(int length)
        {
            return GenerateUpperCharacter(length, length);
        }

        public static string GenerateUpperCharacter(int min, int max)
        {
            var options = new GenerateOptions(min, max) { CharacterModes = GenerateCharacterModes.Uppercase };
            return Generate(options);
        }

        public static string GenerateLowerCharacter(int length)
        {
            return GenerateUpperCharacter(length, length);
        }

        public static string GenerateLowerCharacter(int min, int max)
        {
            var options = new GenerateOptions(min, max) { CharacterModes = GenerateCharacterModes.Lowercase };
            return Generate(options);
        }

        public static string Generate(GenerateOptions options)
        {
            
            string scope = String.Concat(options.IncludeNumbers ? PASSWORD_CHARS_NUMERIC : "", options.IncludeUppercase ? PASSWORD_CHARS_UCASE : "", options.IncludeLowercase ? PASSWORD_CHARS_LCASE : "", options.IncludeSymbols ? PASSWORD_CHARS_SPECIAL : "");
            string generateText = String.Empty;

            byte[] randomBytes = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);
            // Convert 4 bytes into a 32-bit integer value.
            int seed = BitConverter.ToInt32(randomBytes, 0);
            Random r = new Random(seed);
            int textLenth = r.Next(options.MinStringLength, options.MaxStringLength);
            while (generateText.Length < textLenth)
            {
                int index = r.Next(0, scope.Length);
                generateText += scope[index];
            }
            return generateText;
        }

        public static string Generate2(int length = 8)
        {
            var options = new GenerateOptions(length);
            return Generate2(options);
        }

        public static string Generate2(int min,int max)
        {
            var options = new GenerateOptions(min,max);
            return Generate2(options);
        }

        public static string Generate2(GenerateOptions options)
        {
            int minLength = options.MinStringLength;
            int maxLength = options.MaxStringLength;
            // Make sure that input parameters are valid.
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                return null;

            // Create a local array containing supported password characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the password strength.
            char[][] charGroups = new char[][]
            {
                PASSWORD_CHARS_LCASE.ToCharArray(),
                PASSWORD_CHARS_UCASE.ToCharArray(),
                PASSWORD_CHARS_NUMERIC.ToCharArray(),
                PASSWORD_CHARS_SPECIAL.ToCharArray()
            };

            // Use this array to track the number of unused characters in each
            // character group.
            int[] charsLeftInGroup = new int[charGroups.Length];

            // Initially, all characters in each group are not used.
            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            int[] leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            byte[] randomBytes = new byte[4];

            // Generate 4 random bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            int seed = BitConverter.ToInt32(randomBytes, 0);

            // Now, this is real randomization.
            Random random = new Random(seed);

            // This array will hold password characters.
            char[] password = null;

            // Allocate appropriate memory for the password.
            if (minLength < maxLength)
                password = new char[random.Next(minLength, maxLength + 1)];
            else
                password = new char[minLength];

            // Index of the next character to be added to password.
            int nextCharIdx;

            // Index of the next character group to be processed.
            int nextGroupIdx;

            // Index which will be used to track not processed character groups.
            int nextLeftGroupsOrderIdx;

            // Index of the last non-processed character in a group.
            int lastCharIdx;

            // Index of the last non-processed group.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Generate password characters one at a time.
            for (int i = 0; i < password.Length; i++)
            {
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                                                         lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Get the index of the last unprocessed characters in this group.
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                if (lastCharIdx == 0)
                    nextCharIdx = 0;
                else
                    nextCharIdx = random.Next(0, lastCharIdx + 1);

                // Add this character to the password.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] =
                                              charGroups[nextGroupIdx].Length;
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                    charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                    leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }

            // Convert password characters into a string and return the result.
            return new string(password);
        }

        public static Random GetRandom(int bytesCount = 4)
        {
            byte[] randomBytes = new byte[bytesCount];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);
            // Convert 4 bytes into a 32-bit integer value.
            int seed = BitConverter.ToInt32(randomBytes, 0);
            return new Random(seed);
        }
    }

    public class GenerateOptions
    {
        public GenerateOptions() : this(8,8)
        {
          
        }

        public GenerateOptions(int length) : this(length,length)
        {
        }


        public GenerateOptions(int min,int max)
        {
            this.MinStringLength = min;
            this.MaxStringLength = max;
        }

        public int MinStringLength { get; set; }
        public int MaxStringLength { get; set; }
        public GenerateCharacterModes CharacterModes { get; set; } = GenerateCharacterModes.Default;
        public bool IncludeNumbers { get { return (this.CharacterModes & GenerateCharacterModes.Numbers) > 0; } } 
        public bool IncludeLowercase { get { return (this.CharacterModes & GenerateCharacterModes.Lowercase) > 0; } }
        public bool IncludeUppercase { get { return (this.CharacterModes & GenerateCharacterModes.Uppercase) > 0; } }
        public bool IncludeSymbols { get { return (this.CharacterModes & GenerateCharacterModes.Symbols) > 0; } }
        public bool IncludeSpecialCharacter { get { return (this.CharacterModes & GenerateCharacterModes.SpecialCharacter) > 0; } }
    }

    public enum GenerateCharacterModes
    {
        Lowercase = 1,
        Uppercase = 2,
        Numbers = 4,
        Symbols = 8,
        SpecialCharacter = 16,
        Default = Lowercase | Uppercase | Numbers,
        All = Lowercase | Uppercase | Numbers | Symbols | SpecialCharacter
    }
}
