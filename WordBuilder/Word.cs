using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordBuilder
{
	/// <summary>
	/// A word as it exists on a Board
	/// </summary>
	class Word
	{
		#region Nested Types

		/// <summary>
		/// The direction that the word goes in
		/// </summary>
		public enum Direction
		{
			RIGHT,
			DOWN
		}

		#endregion

		#region Declarations

		/// <summary>
		/// The location of the first letter in the word
		/// </summary>
		Board.Coord _coords;

		/// <summary>
		/// The direction that the word goes in
		/// </summary>
		Direction _dir;

		/// <summary>
		/// The word itself
		/// </summary>
		string _word;

		#endregion

		#region Initialization

		public Word(Board.Coord coords, Direction dir, string word)
		{
			_coords = coords;
			_dir = dir;
			_word = word;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Return all letters that make up this word with thier coordinates
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Tuple<Board.Coord, char>> Letters()
		{
			switch(_dir)
			{
				case Word.Direction.DOWN:
					for(var letterIndex = 0; letterIndex < _word.Length; letterIndex++)
					{
						yield return Tuple.Create(new Board.Coord(_coords.X, _coords.Y + letterIndex), _word[letterIndex]);
					}
					break;
				case Word.Direction.RIGHT:
					for(var letterIndex = 0; letterIndex < _word.Length; letterIndex++)
					{
						yield return Tuple.Create(new Board.Coord(_coords.X + letterIndex, _coords.Y), _word[letterIndex]);
					}
					break;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} @ {1} {2}", _word, _coords, _dir);
		}

		#endregion

		#region Properties

		public Board.Coord Coords
		{
			get { return _coords; }
		}

		public Direction Dir
		{
			get { return _dir; }
		}

		public string Value
		{
			get { return _word; }
		}

		#endregion
	}
}
