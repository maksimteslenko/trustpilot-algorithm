using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trustpilotexercise
{

    class Program
    {
        static string answerHash = "4624d200580677270a54ccff86b9610e";

        static void Main(string[] args)
        {
            string questionString = "poultry outwits ants";
            string sortedAnagram = SortString(questionString);

            //Get All Words from .txt File
            List<string> words = GetWords();
            Console.WriteLine("Starting........");

            DateTime start = DateTime.UtcNow;
            Console.WriteLine("Start :" + start);

            Console.WriteLine("Searching..........");
            bool stop = false;
            for (int i = 0; i < words.Count && !stop; i++)//Working with the first word!
            {
                string fullAnagramSorted = sortedAnagram;
                string word1 = words[i];// 1st word
                string anagramMinusWord1 = RemoveCharacters(fullAnagramSorted, word1); // delete letters of the first word from the anagram 

                List<string> filteredWords = GetFilteredWords(words, anagramMinusWord1);

                for (int j = 0; j < filteredWords.Count && !stop; j++)//working with the second word
                {
                    string word2 = filteredWords[j];

                    if (ContainsAll(anagramMinusWord1, word2) && !word1.Equals(word2))
                    {
                        // We have two words, which can make out of anagram ==> "pastils turnout towy"
                        string anagramMinusWord1AndWord2 = RemoveCharacters(anagramMinusWord1, word2); // deleting the letters of the second word from the anagram

                        List<string> filteredWords2 = GetFilteredWords(filteredWords, anagramMinusWord1AndWord2);

                        for (int k = 0; k < filteredWords2.Count; k++)// looking for 3rd word
                        {
                            string word3 = filteredWords2[k]; // word3

                            int phraseLength = word1.Length + word2.Length + word3.Length;

                            if (phraseLength ==  sortedAnagram.Length && !word3.Equals(word1) && !word3.Equals(word2)) // if word3 != word1 and word2, the its good!
                            {
                                string sortedPossiblePhrase = SortString(word1 + " " + word2 + " " + word3); // method returns sorted by alphabet string
                                if (sortedPossiblePhrase.Equals(sortedAnagram)) //!
                                {
                                    string answer = RunHashTest(word1, word2, word3); // generating md5Hash for each phrase and then comparing with hash in the assignment
                                    if (answer != null)
                                    {
                                        Console.WriteLine("Eureka");
                                        Console.WriteLine("Secret phrase = " + answer);
                                        stop = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Finished. Time spent = " + (DateTime.UtcNow - start));
            Console.ReadLine();
        }

        //delete from fullAnagram the letters which consist in word1
        private static string RemoveCharacters(string fullAnagram, string word1)
        {
            var charsToBeRemoved = word1.ToCharArray();
            string wordToReturn = fullAnagram;

            for (int i = 0; i < charsToBeRemoved.Length; i++)
            {
                int index = wordToReturn.IndexOf(charsToBeRemoved[i]);
                if (index > 0)
                {
                    wordToReturn = wordToReturn.Remove(index, 1);
                }
            }

            return wordToReturn;
        }

        //Check if testString has letters from word
        public static bool ContainsAll(string testString, string word)
        {
            foreach (var letter in word)
            {
                if (testString.IndexOf(letter) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        // Check if in the list of possible phrases (List<string> list) there is a secret phrase which gives the hashCode we are looking for
        private static string RunHashTest(string word1, string word2, string word3)
        {
            Dictionary<string, string> hashesMap = new Dictionary<string, string>() { };
            hashesMap.Add(ConvertStringToMd5Hash(word1 + " " + word2 + " " + word3), word1 + " " + word2 + " " + word3);
            hashesMap.Add(ConvertStringToMd5Hash(word1 + " " + word3 + " " + word2), word1 + " " + word3 + " " + word2);
            hashesMap.Add(ConvertStringToMd5Hash(word2 + " " + word1 + " " + word3), word2 + " " + word1 + " " + word3);
            hashesMap.Add(ConvertStringToMd5Hash(word2 + " " + word3 + " " + word1), word2 + " " + word3 + " " + word1);
            hashesMap.Add(ConvertStringToMd5Hash(word3 + " " + word1 + " " + word2), word3 + " " + word1 + " " + word2);
            hashesMap.Add(ConvertStringToMd5Hash(word3 + " " + word2 + " " + word1), word3 + " " + word2 + " " + word1);

            if (hashesMap.ContainsKey(answerHash))
            {
                return hashesMap[answerHash];
            }

            return null;
        }

        //Import from file and filter the words
        public static List<string> GetWords()
        {
            string sortedAnagram = SortString("poultry outwits ants");

            List<string> wordList = new List<string>();

            string[] words = System.IO.File.ReadAllLines(@"wordlist.txt");
            // string[] words = System.IO.File.ReadAllLines(@"test.txt");

            foreach (string word in words)
            {
                string cleanWord = word.Trim();
                if (cleanWord.Length == 4 || cleanWord.Length == 7)  //remove words with ' character, example --> abusivenes's
                {
                    if (!cleanWord.Trim().Contains("'"))//remove words with wrong Length
                    {
                        if (ContainsAll(sortedAnagram, cleanWord)) // remove words which cannot be created from characters in "poultry outwits ants" test string
                        {
                            wordList.Add(cleanWord);
                        }
                    }
                }
            }

            return wordList;
        }

        //Take a word and sort it in alphabetical order: coconut ==> ccnoot
        public static string SortString(string word)
        {
            char[] foo = word.Replace(" ", "").ToArray();
            Array.Sort(foo);
            return new string(foo);
        }

        // Convert String to MD5 Hashcode
        public static string ConvertStringToMd5Hash(string word)
        {
            string hashedString = word;

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(hashedString);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                hashedString = sb.ToString();
            }

            return hashedString;
        }

        //Remove words that cannot be created from characters in updated anagram
        public static List<string> GetFilteredWords(List<string> words, string anagram) 
        {
            List<string> suitableWords = new List<string>();

            foreach (var word in words)
            {
                if (ContainsAll(anagram, word))
                {
                    suitableWords.Add(word);
                }
            }
            return suitableWords;
        }
    }
}

