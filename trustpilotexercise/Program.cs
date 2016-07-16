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
            // Погнали.......
            string questionString = "poultry outwits ants";
            string sortedAnagram = SortString(questionString);

            //Get All Words from .txt File
            List<string> words = GetWords();
            Console.WriteLine("Starting........");

            DateTime start = DateTime.UtcNow;
            Console.WriteLine("Start :" + start);

            Console.WriteLine("Searching..........");
            bool stop = false;
            for (int i = 0; i < words.Count && !stop; i++)//Работаем со 1-ым словом - погнали!
            {
                string fullAnagramSorted = sortedAnagram;
                string word1 = words[i];// 1ое слово
                string anagramMinusWord1 = RemoveCharacters(fullAnagramSorted, word1); // удаляем буквы первого слова из анаграммы 

                for (int j = 0; j < words.Count && !stop; j++)//Работаем со 2-ым словом - погнали!
                {
                    string word2 = words[j];

                    if (ContainsAll(anagramMinusWord1, word2) && !word1.Equals(word2)) // Если буквы слова2(word2) cодержатся в буквах Анграммы-буквы первого слова1 --> тогда слово2 нам интересно и мы можем с ним работать
                    {
                        // У нас есть 2 слова, которые мы можем составить из букв Анаграммы ==> "pastils turnout towy"
                        string anagramMinusWord1AndWord2 = RemoveCharacters(anagramMinusWord1, word2); // удаляем буквы слова2 из анаграммы

                        for (int k = 0; k < words.Count; k++)// Пробуем найти 3-ье слово -> Погнали !
                        {
                            string word3 = words[k]; // слово3

                            if (!word3.Equals(word1) && !word3.Equals(word2)) // если слово3 != слову1 и слову2, тогда оно нам интересно - погнали!
                            {
                                string sortedPossiblePhrase = SortString(word1 + " " + word2 + " " + word3); // метогд возвращает отсортированный string по алфавиту
                                if (sortedPossiblePhrase.Equals(sortedAnagram)) //если у нас есть совпадение всех букв фразы и анаграммы, то уже очень ГОРЯЧО, почти что сосиска в тесте!
                                {
                                    string answer = RunHashTest(word1, word2, word3); // генерируем md5Hash для каждой фразы и сравниваем с hashom который нам дали в задании
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

        //Удалить из словаА(fullAnagram) все буквы содержащиеся в словеБ(word1)
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

        //Проверить содержит ли СловаА(testString) все буквы из Слова Б(word)
        public static bool ContainsAll(string testString, string word)
        {
            foreach (var bukva in word)
            {
                if (testString.IndexOf(bukva) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        // Проверить есть ли среди возможных фраз(List<string> list) секретная фраза, которая даёт искомый hashCode
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

        //Экспорт и Фильтрация слова
        public static List<string> GetWords()
        {
            string sortedAnagram = SortString("poultry outwits ants");

            List<string> slova = new List<string>();

            string[] words = System.IO.File.ReadAllLines(@"wordlist.txt");
            // string[] words = System.IO.File.ReadAllLines(@"test.txt");

            foreach (string word in words)
            {
                string slovo = word.Trim();
                if (slovo.Length == 4 || slovo.Length == 7)  //remove words with ' character, example --> abusivenes's
                {
                    if (!slovo.Trim().Contains("'"))//remove words with wrong Length
                    {
                        if (ContainsAll(sortedAnagram, slovo)) // remove words which cannot be created from characters in "poultry outwits ants" test string
                        {
                            slova.Add(slovo);
                        }
                    }
                }
            }

            return slova;
        }

        //Принимаем слово и перестанавливаем его буквы в алфавитном порядке: coconut ==> ccnoot
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

    }
}

