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

		List<char> _initialLetters;

		Trie _validWords;

		WordGenerator _wordGen;

		List<Board> _solutions;

		int maxSolutions = 4;

		#endregion

		#region Initialization

		/// <summary>
		/// Construct a new solver
		/// </summary>
		/// <param name="letters"></param>
		/// <param name="validWords"></param>
		public Solver(IEnumerable<char> letters, Trie validWords)
		{
			_initialLetters = letters.ToList();
			_validWords = validWords;
			_wordGen = new WordGenerator(_validWords);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Find a board that utilizes all of the letters
		/// </summary>
		/// <returns></returns>
		public void Solve()
		{
			var blankBoard = new Board(_validWords);
			_solutions = new List<Board>();

			foreach (var initialWord in _wordGen.AllPossibleWords(_initialLetters))//.OrderByDescending(l => l.Length))
			{
				var usedLetters = new List<char>();
				var board = blankBoard.Add(
					new Word(new Board.Coord(0, 0), Word.Direction.RIGHT, initialWord),
					usedLetters);

				if (board == null)
					continue;

				var letters = new List<char>(_initialLetters);
				foreach (var letter in usedLetters)
					letters.Remove(letter);

				//Console.WriteLine("Starting with word {0}, remaining {1}", initialWord, string.Join("", letters.ToArray()));

				MakeMove(letters, board);

				if (_solutions.Count > maxSolutions)
					return;
			}
		}

		/// <summary>
		/// Make a move on the board, returning non null Board if a solution is found
		/// </summary>
		/// <param name="lettersInHand"></param>
		/// <param name="board"></param>
		/// <returns></returns>
		public void MakeMove(List<char> lettersInHand, Board board)
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

					// try to add a horizontal word
					usedLetters.Clear();
					var result = board.Add(
						new Word(new Board.Coord(boardLetter.Key.X - letterPosition, boardLetter.Key.Y), Word.Direction.RIGHT, wordChoice),
						usedLetters);

					if (result != null)
					{
						var lettersLeft = new List<char>(lettersInHand);
						foreach (var letter in usedLetters)
							lettersLeft.Remove(letter);

						//Console.WriteLine("Adding {0}, remaining {1}", wordChoice, string.Join("", lettersLeft.ToArray()));

						if (lettersLeft.Count == 0)
						{
							Console.Clear();
							//Console.CursorLeft = 0;
							//Console.CursorTop = 0;

							Console.WriteLine("Found:");
							Console.WriteLine(result);
							//Console.ReadLine();

							_solutions.Add(result);
							return;
						}
						else
						{
							MakeMove(lettersLeft, result);
							return; //don't make more than one solution with this root word
						}
					}

					// try to add a vertical word
					usedLetters.Clear();
					result = board.Add(
						new Word(new Board.Coord(boardLetter.Key.X, boardLetter.Key.Y - letterPosition), Word.Direction.DOWN, wordChoice),
						usedLetters);

					if (result != null)
					{
						var lettersLeft = new List<char>(lettersInHand);
						foreach (var letter in usedLetters)
							lettersLeft.Remove(letter);

						//Console.WriteLine("Adding {0}, remaining {1}", wordChoice, string.Join("", lettersLeft.ToArray()));

						if (lettersLeft.Count == 0)
						{
							Console.Clear();
							//Console.CursorLeft = 0;
							//Console.CursorTop = 0;

							Console.WriteLine("Found:");
							Console.WriteLine(result);
							//Console.ReadLine();

							_solutions.Add(result);
							return;
						}
						else
						{
							MakeMove(lettersLeft, result);
							return; //don't make more than one solution with this root word
						}
					}
				}
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// All of the solutions that were found
		/// </summary>
		public ICollection<Board> Solutions
		{
			get { return _solutions; }
		}

		#endregion
	}


}
