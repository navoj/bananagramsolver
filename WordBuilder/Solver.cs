using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordBuilder
{
	/// <summary>
	/// Given a set of letters and a Trie of valid words, generate all possible boards
	/// </summary>
	class Solver
	{
		#region Declarations

		Trie _validWords;

		WordGenerator _wordGen;

		int maxSolutions = 25;

		#endregion

		#region Initialization

		/// <summary>
		/// Construct a new solver
		/// </summary>
		/// <param name="letters"></param>
		/// <param name="validWords"></param>
		public Solver(Trie validWords)
		{
			_validWords = validWords;
			_wordGen = new WordGenerator(_validWords);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Find a board that utilizes all of the letters
		/// </summary>
		/// <returns></returns>
		public Board Solve(IEnumerable<char> lettersInHand)
		{
			var blankBoard = new Board(_validWords);
			var solutions = new List<Board>();

			foreach (var initialWord in _wordGen.AllPossibleWords(lettersInHand))
			{
				var usedLetters = new List<char>();
				var board = blankBoard.TryAdd(
					new Word(new Board.Coord(0, 0), Word.Direction.RIGHT, initialWord),
					usedLetters);

				if (board == null)
					continue;

				var letters = new List<char>(lettersInHand);
				foreach (var letter in usedLetters)
					letters.Remove(letter);

				if (letters.Count == 0)
				{
					solutions.Add(board);
					continue;
				}

				//Console.WriteLine("Starting with word {0}, remaining {1}", initialWord, string.Join("", letters.ToArray()));

				var result = MakeMove(letters, board);
				if (result != null)
					solutions.Add(result);

				if (solutions.Count > maxSolutions)
					break;
			}

			if (solutions.Count == 0)
				return null;

			return solutions.MinBy(board => board.Words.Count());
		}

		/// <summary>
		/// Given a starting board, add a single letter making minimal changes to the board
		/// </summary>
		/// <param name="board"></param>
		/// <param name="newLetter"></param>
		public Board SolveIncremental(Board board, char newLetter)
		{
			// see if board can be satisfied by adding the letter as prefix of suffix to an existing word
			foreach (var existingWord in board.Words)
			{
				var possibleNewWords = new List<Word>();
				switch (existingWord.Dir)
				{
					case Word.Direction.DOWN:
						// prefix
						var newWord = newLetter + existingWord.Value;
						if (_validWords.Contains(newWord))
						{
							possibleNewWords.Add(new Word(new Board.Coord(existingWord.Coords.X, existingWord.Coords.Y - 1),
								Word.Direction.DOWN,
								newWord));
						}
						// suffix
						newWord = existingWord.Value + newLetter;
						if (_validWords.Contains(newWord))
						{
							possibleNewWords.Add(new Word(new Board.Coord(existingWord.Coords.X, existingWord.Coords.Y),
								Word.Direction.DOWN,
								newWord));
						}
						break;
					case Word.Direction.RIGHT:
						// prefix
						newWord = newLetter + existingWord.Value;
						if (_validWords.Contains(newWord))
						{
							possibleNewWords.Add(new Word(new Board.Coord(existingWord.Coords.X-1, existingWord.Coords.Y),
								Word.Direction.RIGHT,
								newWord));
						}
						// suffix
						newWord = existingWord.Value + newLetter;
						if (_validWords.Contains(newWord))
						{
							possibleNewWords.Add(new Word(new Board.Coord(existingWord.Coords.X, existingWord.Coords.Y),
								Word.Direction.RIGHT,
								newWord));
						}
						break;
				}
				foreach(var possibleNewWord in possibleNewWords)
				{
					var newBoard = board.TryAdd(possibleNewWord, new List<char>());
					if (newBoard != null)
					{
						Console.WriteLine("-> Modify existing {0}", possibleNewWord);
						return newBoard;
					}
				}
			}

			// see if board can be satisfied by adding the letter somewhere
			var result = MakeMove(new List<char> { newLetter }, board);
			if (result != null)
			{
				Console.WriteLine("-> Added single letter");
				return result;
			}

			var allWords = board.Words.ToArray().OrderBy(w => w.Value.Length).ToArray();
			// see if the board can be satifisfied by removing a word
			foreach (var word in allWords)
			{
				var lettersInHand = new List<char> { newLetter };
				var newBoard = board.TryRemove(word, lettersInHand);

				if (newBoard == null) // unable to remove word
				{
					continue;
				}

				result = MakeMove(lettersInHand, newBoard);
				if (result != null)
				{
					Console.WriteLine("-> Removed {0}", word);
					return result;
				}
			}

			// see if the baord can be satisfied by removing two words
			for (int firstWordIndex = 0; firstWordIndex < allWords.Length; firstWordIndex++)
			{
				var firstWord = allWords[firstWordIndex];

				var lettersInHand = new List<char> { newLetter };
				var newBoard = board.TryRemove(firstWord, lettersInHand);
				if (newBoard == null)
					continue;

				for (int secondWordIndex = firstWordIndex + 1; secondWordIndex < allWords.Length; secondWordIndex++)
				{
					var secondWord = allWords[secondWordIndex];

					var secondLettersInHand = lettersInHand.ToList();
					var secondNewBoard = newBoard.TryRemove(secondWord, secondLettersInHand);
					if (secondNewBoard == null)
					{
						continue;
					}

					result = MakeMove(secondLettersInHand, secondNewBoard);
					if (result != null)
					{
						Console.WriteLine("-> Removed {0} and {1}", firstWord, secondWord);
						return result;
					}
				}
			}

			// just attempt to solve from scratch
			Console.WriteLine("-> Recreate entire board");
			return Solve(board.Letters.Select(x => x.Value).Concat(new[]{ newLetter }));
		}

		/// <summary>
		/// Make a move on the board, returning non null Board if a solution is found
		/// </summary>
		/// <param name="lettersInHand"></param>
		/// <param name="board"></param>
		/// <returns></returns>
		public Board MakeMove(List<char> lettersInHand, Board board)
		{
			List<char> usedLetters = new List<char>();

			foreach (var boardLetter in board.Letters)
			{
				lettersInHand.Add(boardLetter.Value);
				var wordChoices = _wordGen.AllPossibleWordsContaining(lettersInHand, boardLetter.Value);
				lettersInHand.Remove(boardLetter.Value);

				foreach (var wordChoice in wordChoices)//.OrderByDescending(x => x.Length))
				{
					// need to handle multiple letters in same word
					int letterPosition = wordChoice.IndexOf(boardLetter.Value);
					if (letterPosition == -1)
						continue;

					foreach (var possibleWord in new[] { 
						new Word(new Board.Coord(boardLetter.Key.X - letterPosition, boardLetter.Key.Y), Word.Direction.RIGHT, wordChoice),
						new Word(new Board.Coord(boardLetter.Key.X, boardLetter.Key.Y - letterPosition), Word.Direction.DOWN, wordChoice),})
					{
						usedLetters.Clear();
						var result = board.TryAdd(possibleWord, usedLetters);

						if (result == null)
							continue;

						var lettersLeft = new List<char>(lettersInHand);
						foreach (var letter in usedLetters)
							lettersLeft.Remove(letter);

						//Console.WriteLine("Adding {0}, remaining {1}", wordChoice, string.Join("", lettersLeft.ToArray()));

						if (lettersLeft.Count == 0)
						{
							// display results for long running queries
							//Console.Clear();
							//Console.WriteLine("Found:");
							//Console.WriteLine(result);

							return result;
						}
						else
						{
							//don't make more than one solution with this root word
							return MakeMove(lettersLeft, result);
						}
					}
				}
			}
			return null;
		}

		#endregion

		#region Properties

		#endregion
	}

	/// <summary>
	/// Implement select min using a given Func in linq
	/// From http://stackoverflow.com/questions/914109/how-to-use-linq-to-select-object-with-minimum-or-maximum-property-value
	/// </summary>
	public static class MinByExtension
	{
		public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
		{
			return source.MinBy(selector, Comparer<TKey>.Default);
		}

		public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
		{
			using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
			{
				if (!sourceIterator.MoveNext())
				{
					throw new InvalidOperationException("Sequence was empty");
				}
				TSource min = sourceIterator.Current;
				TKey minKey = selector(min);
				while (sourceIterator.MoveNext())
				{
					TSource candidate = sourceIterator.Current;
					TKey candidateProjected = selector(candidate);
					if (comparer.Compare(candidateProjected, minKey) < 0)
					{
						min = candidate;
						minKey = candidateProjected;
					}
				}
				return min;
			}
		}
	}
}
