using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace WordBuilder
{
	class Program
	{
		static void Main(string[] args)
		{
			string bananagramsLetters = "aaaaaaaaaaaaabbbcccddddddeeeeeeeeeeeeeeeeeefffgggghhhiiiiiiiiiiiijjkklllllmmmnnnnnnnnooooooooooopppqqrrrrrrrrrsssssstttttttttuuuuuuvvvwwwxxyyyzz";

			var sw = Stopwatch.StartNew();
			var words = File.ReadAllLines("2of4brif.txt");
			Console.WriteLine("Load words: {0}", sw.Elapsed);
			sw = Stopwatch.StartNew();
			var trie = new Trie(words);
			Console.WriteLine("Gen Trie: {0}", sw.Elapsed);


			while (true)
			{
				Console.Write("Letters: ");

				//var rand = new Random();
				//var letters = bananagramsLetters.OrderBy(_ => rand.Next()).Take(30).ToArray();
				//Console.WriteLine(string.Join("", letters));

				var letters = Console.ReadLine();
				//var letters = bananagramsLetters;
				var solver = new Solver(letters, trie);

				sw = Stopwatch.StartNew();
				solver.Solve();
				Console.WriteLine("Find solution: {0}", sw.Elapsed);

				Console.WriteLine();
				foreach (var solution in solver.Solutions)
				{
					if (solution.Letters.Count() != letters.Length)
					{
						var usedLetters = new List<char>(letters);
						foreach (var letter in solution.Letters.Select(x => x.Value))
							usedLetters.Remove(letter);

						Console.WriteLine("MISSING LETTERS! {0}", string.Join("", usedLetters.ToArray()));
					}
					Console.WriteLine(solution);
				}
				Console.WriteLine();
			}




			//var gen = new WordGenerator(trie);

			//while (true)
			//{
			//    //var rand = new Random();
			//    //var letters = bananagramsLetters.OrderBy(_ => rand.Next()).Take(21).ToArray();
			//    //Console.WriteLine(string.Join("", letters));

			//    Console.Write("Letters: ");
			//    var letters = Console.ReadLine();

			//    sw = Stopwatch.StartNew();
			//    var results = gen.AllPossibleWords(letters);
			//    Console.WriteLine("Find {0} words: {1}", results.Count, sw.Elapsed);

			//    Console.WriteLine();
			//    foreach (var word in results.OrderByDescending(w => w.Length).Take(25))
			//    {
			//        Console.WriteLine(word);
			//    }
			//    Console.WriteLine();
			//}


			//var trie = new Tries(word);
			//foreach (var word in new[] { "apple", "sous", "mouse", "cleats", "key", "waft", "learn", "theft", "swing", "weep" })
			//{
			//    sw = Stopwatch.StartNew();
			//    trie.Contains(word);
			//    Console.WriteLine("Seek {0}: {1}", word, sw.Elapsed);
			//}

			

			//var board = new Board(words);

			//board.Add(new Word(new Board.Coord(0, 0), Word.Direction.RIGHT, "pope"));
			//board.Add(new Word(new Board.Coord(3, 0), Word.Direction.DOWN, "egg"));
			//board.Add(new Word(new Board.Coord(2, 1), Word.Direction.RIGHT, "ignore"));

			//Console.WriteLine();
			//Console.WriteLine(board);

			//foreach (var word in board.Words)
			//    Console.WriteLine(word);


			//each choice is a mutation of an existing state



			// to start, generate longest word possible, place it at 0,0

			// foreach letter
			//  scan above / below letter to see limits, could be multiple constraints / joins
			//  generate all words with remaining letters + intersecting letter
			//  of those words, find the ones that match the constraint(letter exists in a valid location)
			//  order by largest word


			Console.WriteLine();
			Console.WriteLine("Done");
			//Console.ReadLine();
		}
	}
}
