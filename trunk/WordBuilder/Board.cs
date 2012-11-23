using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordBuilder
{
	/// <summary>
	/// A board of interconnecting words
	/// </summary>
	class Board
	{
		#region Nested Types

		/// <summary>
		/// A location on the board
		/// </summary>
		public struct Coord : IEquatable<Coord>
		{
			int _x;
			int _y;

			public Coord(int x, int y)
			{
				_x = x;
				_y = y;
			}

			public int X
			{
				get { return _x; }
			}

			public int Y
			{
				get { return _y; }
			}

			#region IEquatable<Coord> Members

			public bool Equals(Coord other)
			{
				return _x == other._x && _y == other._y;
			}

			public override int GetHashCode()
			{
				return _x + _y * 100;
			}

			public override bool Equals(object obj)
			{
				if (obj is Coord)
					return Equals((Coord)obj);
				return false;
			}

			public override string ToString()
			{
				return string.Format("[ {0}, {1} ]", _x, _y);
			}

			#endregion
		}

		#endregion

		#region Declarations

		/// <summary>
		/// 
		/// </summary>
		Dictionary<Coord, char> _board = new Dictionary<Coord,char>();

		/// <summary>
		/// 
		/// </summary>
		Trie _validWords;

		#endregion

		#region Initialization

		/// <summary>
		/// Create a new Board that uses the list of words as validWords
		/// </summary>
		/// <param name="validWords"></param>
		public Board(Trie validWords)
		{
			_validWords = validWords;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other"></param>
		public Board(Board other)
		{
			_board = new Dictionary<Coord, char>(other._board);
			_validWords = other._validWords;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a word to the board, returns null if the move was invalid, a new board if the Add was valid
		/// </summary>
		/// <param name="word"></param>
		/// <param name="usedLetters">The letters used when adding the word, only valid</param>
		/// <returns>The new Board after adding the word</returns>
		public Board TryAdd(Word word, List<char> usedLetters)
		{
			var child = new Board(this);

			bool isFirstWord = _board.Count == 0;
			int overlappedLetterCount = 0;

			for (int letterIndex = 0; letterIndex < word.Value.Length; letterIndex++)
			{
				Coord letterCoord;
				if (word.Dir == Word.Direction.DOWN)
				{
					letterCoord = new Coord(word.Coords.X, word.Coords.Y + letterIndex);
				}
				else
				{
					letterCoord = new Coord(word.Coords.X + letterIndex, word.Coords.Y);
				}

				char letter = word.Value[letterIndex];

				char existingLetter;
				if (child._board.TryGetValue(letterCoord, out existingLetter))
				{
					if (existingLetter != letter)
					{
						return null;

						//throw new InvalidOperationException(string.Format("Attempted to place a '{0}' on top of '{1}' at {2}", letter, existingLetter, letterCoord));
					}
					overlappedLetterCount++;
				}
				else
				{
					child._board.Add(letterCoord, letter);
					usedLetters.Add(letter);
				}
			}

			// don't want to do an add that doesn't actually add any letters to the board
			if (overlappedLetterCount == word.Value.Length)
				return null;

			if (!child.IsValid)
				return null;

			return child;
		}

		/// <summary>
		/// Attempt to remove this word, does not remove any letters used by other words
		/// </summary>
		/// <param name="word"></param>
		/// <param name="newLetters">Letters are added to this list if they are removed</param>
		/// <returns>non null on success</returns>
		public Board TryRemove(Word word, List<char> newLetters)
		{
			var doubleUsedCoords = CalcDoubleUsage();
			var child = new Board(this);

			foreach (var deleteLetter in word.Letters())
			{
				// can't remove letter used by another word
				if (doubleUsedCoords.Contains(deleteLetter.Item1))
					continue;

				child._board.Remove(deleteLetter.Item1);
				newLetters.Add(deleteLetter.Item2);
			}

			if (newLetters.Count == 0)
				return null;

			if (!child.IsValid)
				return null;

			return child;
		}

		/// <summary>
		/// Compute how many different letters are being used by both a right and down word
		/// </summary>
		/// <returns></returns>
		private HashSet<Coord> CalcDoubleUsage()
		{
			var downWordCoords = new HashSet<Coord>();
			var rightWordCoords = new HashSet<Coord>();

			foreach (var word in Words)
			{
				switch(word.Dir)
				{
					case Word.Direction.DOWN:
						downWordCoords.UnionWith(word.Letters().Select(l => l.Item1));
						break;
					case Word.Direction.RIGHT:
						rightWordCoords.UnionWith(word.Letters().Select(l => l.Item1));
						break;
				}
			}

			downWordCoords.IntersectWith(rightWordCoords);
			return downWordCoords;
		}

		#endregion

		#region Overrides

		public override string ToString()
		{
			// need to find min X and Y
			int minX = _board.Keys.Min(coord => coord.X);
			int minY = _board.Keys.Min(coord => coord.Y);
			int maxX = _board.Keys.Max(coord => coord.X);
			int maxY = _board.Keys.Max(coord => coord.Y);

			var result = new StringBuilder();

			for (int y = minY; y <= maxY; y++)
			{
				for (int x = minX; x <= maxX; x++)
				{
					var letter = this[new Coord(x, y)];
					if (letter == char.MinValue)
						result.Append('.');
					else
						result.Append(letter);
				}
				result.AppendLine();
			}

			return result.ToString();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Return true of false if this board is valid
		/// </summary>
		public bool IsValid
		{
			get
			{
				HashSet<Coord> seenCoords = null;

				// check that all of the words are valid
				foreach (var boardWord in Words)
				{
					// each word must be in dictionary
					if (!_validWords.Contains(boardWord.Value))
						return false;

					if (seenCoords == null)
					{
						seenCoords = new HashSet<Coord>(boardWord.Letters().Select(x => x.Item1));
					}
					else
					{
						// check that at least one letter intersects with an existing word
						var coords = boardWord.Letters().Select(x => x.Item1).ToArray();
						bool overlapsExisting = false;
						foreach(var letterCoord in coords)
						{
							if(seenCoords.Contains(letterCoord))
							{
								overlapsExisting = true;
								break;
							}
						}

						if (!overlapsExisting)
							return false;

						seenCoords.UnionWith(coords);
					}
				}

				return true;
			}
		}

		/// <summary>
		/// Access the character at this location, char.MinValue if no actual letter exists
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public char this[Coord pos]
		{
			get
			{
				char value = char.MinValue;
				_board.TryGetValue(pos, out value);
				return value;
			}
		}

		/// <summary>
		/// Iterate over all words contained in the board
		/// </summary>
		public IEnumerable<Word> Words
		{
			get
			{
				foreach (var key in _board.Keys)
				{
					// detect horizontal words starting at this letter
					if (!_board.ContainsKey(new Coord(key.X - 1, key.Y)) && _board.ContainsKey(new Coord(key.X + 1, key.Y)))
					{
						var wordBuilder = new StringBuilder();

						var pos = key;
						do
						{
							wordBuilder.Append(_board[pos]);
							pos = new Coord(pos.X + 1, pos.Y);
						}
						while (_board.ContainsKey(pos));

						yield return new Word(key, Word.Direction.RIGHT, wordBuilder.ToString());
					}

					// detect vertical words starting at this letter
					if (!_board.ContainsKey(new Coord(key.X, key.Y - 1)) && _board.ContainsKey(new Coord(key.X, key.Y + 1)))
					{
						var wordBuilder = new StringBuilder();

						var pos = key;
						do
						{
							wordBuilder.Append(_board[pos]);
							pos = new Coord(pos.X, pos.Y + 1);
						}
						while (_board.ContainsKey(pos));

						yield return new Word(key, Word.Direction.DOWN, wordBuilder.ToString());
					}
				}
			}
		}

		/// <summary>
		/// Return all of the letters in the board
		/// </summary>
		public IEnumerable<KeyValuePair<Coord, char>> Letters
		{
			get
			{
				return _board;
			}
		}

		#endregion
	}
}
