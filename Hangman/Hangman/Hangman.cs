﻿using System;
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
            Random random = new Random();
            int index = random.Next(Words.Length);
            SecretWord = Words[index].ToLower();

            Correct = new char[SecretWord.Length];
            Incorrect = new StringBuilder();

            Guess = string.Empty;
            Tries = 10;
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
        }


        private string SecretWord
        {
            get; set;
        }


        private char[] Correct
        {
            get; set;
        }


        public StringBuilder Incorrect
        {
            get; private set;
        }


        private string Guess
        {
            get; set;
        }


        public int Tries
        {
            get; private set;
        }


        public void Reset()
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

            result = (Tries <= 0 && !HasWinner()) ? $"{result} You have no guesses left!\n\nThe secret word was '{SecretWord.ToUpper()}'.\n\nGAME OVER." : result;

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
                result = $"'{Guess.ToLower()}' is wrong.";
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
            Game = new Hangman();
            Play = true;
            ShowRules = true;
            Result = string.Empty;
            PlayHangman();
        }


        private Hangman Game
        {
            get; set;
        }


        private bool Play
        {
            get; set;
        }


        private bool ShowRules
        {
            get; set;
        }


        private string Result
        {
            get; set;
        }


        private void PlayHangman()
        {
            while (Play)
            {
                Console.Clear(); //Clear previous round

                Console.WriteLine("\n*-*-*-* Hangman *-*-*-*\n");

                PrintRules();

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


        private void PrintRules()
        {
            if (ShowRules)
            {
                Console.WriteLine($"Welcome to Hangman! Try to guess the letters in the secret word. You have {Game.Tries} guesses.");
                Console.WriteLine("Hint: The theme of the secret word is 'summer'.\n");
                ShowRules = false;
            }
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
                ShowRules = true;
                Game.Reset();
            }
            else
            {
                Console.WriteLine("\n\nBye!");
            }
        }


    }
}
