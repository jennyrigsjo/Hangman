using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame
{
    internal class Hangman
    {
        public Hangman()
        {
            Init();
        }

        private string[] _words = {
            "summer",
            "flower",
            "icecream",
            "beach",
            "sand",
            "sea",
            "sunglasses",
            "lake",
            "cloud",
            "rain",
            "vacation",
            "swimsuit",
        };
        private string[] Words
        {
            get => _words;
            set => _words = value;
        }


        private string _secretWord = string.Empty;
        private string SecretWord
        {
            get => _secretWord;
            set => _secretWord = value;
        }


        private char[] _correct = Array.Empty<char>();
        private char[] Correct
        {
            get => _correct;
            set => _correct = value;
        }


        private StringBuilder _incorrect = new();
        public StringBuilder Incorrect
        {
            get => _incorrect;
            private set => _incorrect = value;
        }


        private string _guess = string.Empty;
        private string Guess
        {
            get => _guess;
            set => _guess = value;
        }


        private int _tries = 10;
        public int Tries
        {
            get => _tries;
            private set => _tries = value;
        }


        public void Init()
        {
            Random random = new Random();
            int index = random.Next(Words.Length);
            SecretWord = Words[index].ToLower();

            Correct = new char[SecretWord.Length];
            Incorrect.Clear();

            Guess = string.Empty;
            Tries = 10;
        }


        public string SecretWordStatus()
        {
            string word = "";

            foreach (char letter in SecretWord)
            {
                if (Correct.Contains(letter))
                {
                    word += $"{char.ToUpper(letter)} ";
                }
                else
                {
                    word += "_ ";
                }
            }

            return word;
        }


        public string MakeGuess(string guess)
        {
            Guess = guess;

            if ( ( GuessIsLetter() && !AlreadyGuessedLetter() ) || GuessIsWord() )
            {
                Tries--;
            }

            string result = Guess switch
            {
                string when !AllCharsAreLetters() => throw new ArgumentException($"Invalid input (\"{Guess}\"): alphabetic characters only"),
                string when GuessIsLetter() => GuessLetter(),
                string when GuessIsWord() => GuessWord(),
            };

            result = (Tries <= 0 && !HasWinner()) ? $"{result} You have no guesses left!\n\nGAME OVER." : result;

            return result;
        }


        private bool AllCharsAreLetters()
        {
            return (!string.IsNullOrEmpty(Guess) && !string.IsNullOrWhiteSpace(Guess) && Guess.All(char.IsLetter));
        }


        private bool GuessIsLetter()
        {
            return (AllCharsAreLetters() && Guess.Length == 1);
        }


        private bool GuessIsWord()
        {
            return (AllCharsAreLetters() && Guess.Length > 1);
        }


        private bool AlreadyGuessedLetter()
        {
            char letter = char.ToLower(char.Parse(Guess));
            return (Correct.Contains(letter) || Incorrect.ToString().Contains(letter));
        }


        private void AddCorrectLetter(char letter)
        {
            //string correct = Correct.ToString(); //Does not work for some reason
            string correct = new(Correct);
            correct += letter;
            Correct = correct.ToCharArray();
        }


        private string GuessLetter()
        {
            char letter = char.ToLower(char.Parse(Guess));
            string result;

            if (AlreadyGuessedLetter())
            {
                result = $"You already guessed '{letter}'.";
            }
            else if (SecretWord.Contains(letter))
            {
                AddCorrectLetter(letter);
                result = HasWinner() ? $"Correct! The secret word was '{SecretWord.ToUpper()}'." : $"The word contains '{letter}'.";
            }
            else
            {
                Incorrect.Append($"{letter} ");
                result = $"There is no '{letter}' in the word.";
            }

            return result;
        }


        private string GuessWord()
        {
            string result;

            if (Guess.ToLower() == SecretWord)
            {
                result = $"Correct! The secret word was '{Guess.ToUpper()}'.";
            } 
            else
            {
                result = $"'{Guess}' is wrong.";
            }

            return result;
        }


        public bool HasWinner()
        {
            bool hasWinner = false;

            if (Guess.ToLower() == SecretWord)
            {
                hasWinner = true;
            }
            else
            {
                foreach (char letter in SecretWord)
                {
                    if (Correct.Contains(letter))
                    {
                        hasWinner = true;
                    }
                    else
                    {
                        hasWinner = false;
                        break;
                    }
                }
            }

            return hasWinner;
        }

    }

    internal class HangmanController
    {
        public HangmanController()
        {
            while (Play)
            {
                Console.Clear(); //Clear previous round

                Console.WriteLine("\n*-*-*-* Hangman *-*-*-*\n");

                PrintStatus();

                if (Game.HasWinner() || Game.Tries == 0)
                {
                    AskPlayAgain();
                    continue;
                }

                Console.Write($"\nMake a guess (letter or word): ");
                var guess = Console.ReadLine();

                if (string.IsNullOrEmpty(guess) || string.IsNullOrWhiteSpace(guess))
                {
                    continue;
                }

                MakeGuess(guess);
            }
        }


        private Hangman _game = new();
        private Hangman Game 
        {
            get => _game;
        }


        private bool _play = true;
        private bool Play
        {
            get => _play;
            set => _play = value;
        }


        private string _result = string.Empty;
        private string Result
        {
            get => _result;
            set => _result = value;
        }


        private void PrintStatus()
        {
            Console.WriteLine($"Secret word: {Game.SecretWordStatus()} ({Game.Tries} guesses left)");

            if (Game.Incorrect.ToString().Length > 0)
            {
                Console.WriteLine($"Incorrect letters: {Game.Incorrect}");
            }

            if (!string.IsNullOrEmpty(Result))
            {
                Console.WriteLine($"\n{Result}");
            }
        }


        private void MakeGuess(string guess)
        {
            try
            {
                Result = Game.MakeGuess(guess);
            }
            catch (ArgumentException e)
            {
                Result = e.Message;
            }
        }


        private void AskPlayAgain()
        {
            Console.Write("\nPlay again ('y' for yes, any other key to quit): ");
            char choice = char.ToLower(Console.ReadKey().KeyChar);

            Play = choice switch
            {
                'y' => true,
                _ => false,
            };

            if (Play)
            {
                Result = string.Empty;
                Game.Init();
            }
            else
            {
                Console.WriteLine("\n\nBye!");
            }
        }
    }
}
