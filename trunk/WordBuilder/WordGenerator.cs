using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordBuilder
{
	/// <summary>
	/// From a set of letters, generate all possible words
	/// </summary>
	class WordGenerator
	{
		#region Declarations

		Trie _validWords;

		List<string> _words = new List<string>();

		#endregion

		#region Initialization

		public WordGenerator(Trie validWords)
		{
			_validWords = validWords;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Find all possible words from this set of letters
		/// </summary>
		/// <param name="letters">must be all lower case</param>
		/// <returns></returns>
		public ICollection<string> AllPossibleWordsContaining(IEnumerable<char> letters, char hasLetter)
		{
			var node = _validWords.Root;
			int[] availableLetters = new int[26];
			foreach (var letter in letters)
			{
				int location = letter - TrieNode.ASCIIA;
				availableLetters[location]++;
			}

			_words = new List<string>();

			VisitChoicesContaining(_validWords.Root, availableLetters, string.Empty, hasLetter, false);

			return _words;
		}

		private void VisitChoicesContaining(TrieNode node, int[] lettersAvailable, string wordSoFar, char containingLetter, bool hasLetter)
		{
			if (node.isEnd && hasLetter)
				_words.Add(wordSoFar);

			for (int i = 0; i < 26; i++)
			{
				var child = node.nodes[i];

				if (child != null && lettersAvailable[i] > 0)
				{
					lettersAvailable[i]--;

					char newLetter = (char)(i + TrieNode.ASCIIA);
					var word = wordSoFar + newLetter;

					VisitChoicesContaining(child, lettersAvailable, word, containingLetter, hasLetter || newLetter == containingLetter);

					lettersAvailable[i]++;
				}
			}
		}


		/// <summary>
		/// Find all possible words from this set of letters
		/// </summary>
		/// <param name="letters">must be all lower case</param>
		/// <returns></returns>
		public ICollection<string> AllPossibleWords(IEnumerable<char> letters)
		{
			var node = _validWords.Root;
			int[] availableLetters = new int[26];
			foreach (var letter in letters)
			{
				int location = letter - TrieNode.ASCIIA;
				availableLetters[location]++;
			}

			_words = new List<string>();

			VisitChoices(_validWords.Root, availableLetters, string.Empty);

			return _words;
		}

		private void VisitChoices(TrieNode node, int[] lettersAvailable, string wordSoFar)
		{
			if (node.isEnd)
				_words.Add(wordSoFar);

			for (int i = 0; i < 26; i++)
			{
				var child = node.nodes[i];

				if (child != null && lettersAvailable[i] > 0)
				{
					lettersAvailable[i]--;

					char newLetter = (char)(i + TrieNode.ASCIIA);
					var word = wordSoFar + newLetter;

					VisitChoices(child, lettersAvailable, word);

					lettersAvailable[i]++;
				}
			}
		}

		#endregion
	}
}
