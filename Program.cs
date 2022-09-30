/*****************************************************************\
	Markov Chain Based Text Generator
		Copyright (c) Kira "Siri" K. 2022
		Source code available under the Mozilla Public License.
		See the LICENSE file for more information.
		If a LICENSE file was not provided with this source code,
			visit github.com/siri-chan/markov and review the
			LICENSE there.
\*****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Siri.markov {
	class Program {
		static void Main(string[] args) {
			Config cfg = new ConfigBuilder().setWorkingDirectory("E:/markov_test").build();
			//todo try/catch this and make sure file esxists
			var checksum = GetChecksum(cfg.workingDir + "/list.txt");
			Cache cache;
			if (cfg.useCache){
				try {
					cache = Cache.load();
					if (checksum != cache.list_checksum) {
						cache = new Cache(checksum);
					} else {
						cache = Cache.load();
					}
				} catch (Exception e) {
					cache = new Cache(checksum);
				}
			} else {
				cache = new Cache(checksum);
			}
			string[] lines = System.IO.File.ReadAllLines(cfg.workingDir + "/list.txt");
			if (/*cache.firstWords == new List<string>()*/ true) { //todo check why this doesnt work
				foreach (string line in lines) {
					//Regex for find dash-seperated word (with or without apostrophies). will add more acceptable characters if need be
					//Console.WriteLine(line);
					cache.firstWords.Add(Regex.Match(line, @"^([\w\-\\']+)").ToString());
				}
			}

			var r = new Random();
			int i = r.Next(cache.firstWords.Count);

			string curWord = cache.firstWords[i];
			string nextWord;

			Console.WriteLine("First Word: " + curWord);

			string buf = curWord;
			while(true){
				if(cfg.useCache)
					cache.save();
				for (int k = 0; k < cfg.length; k++){
					FreqTable freq;
					if (cache.frequencies.ContainsKey(curWord) && cache.frequencies[curWord].curWord == curWord){
						freq = cache.frequencies[curWord];
						int j = r.Next(freq.words.Count);
						nextWord = freq.words[j];
						Console.WriteLine("Next Word: " + nextWord);
					} else {
						freq = makeTable(curWord, lines);
						cache.frequencies.Add(curWord, freq);
						int j = r.Next(freq.words.Count);
						nextWord = freq.words[j];
						Console.WriteLine("Next Word: " + nextWord);
					}
					buf += " " + nextWord;
					curWord = nextWord;
				}
				Console.WriteLine(buf);
				Console.WriteLine("Press Enter to Quit, or Space to Generate More");
				var key = Console.ReadKey().Key;
				while (key != ConsoleKey.Enter && key != ConsoleKey.Spacebar) { key = Console.ReadKey().Key; }
				if(key == ConsoleKey.Enter) System.Environment.Exit(0);
			}
		}
		static string GetChecksum(string filename) {
			using (var md5 = System.Security.Cryptography.MD5.Create()) {
				using (var stream = System.IO.File.OpenRead(filename)) {
					var hash = md5.ComputeHash(stream);
					return BitConverter.ToString(hash).Replace("-", "");
				}
			}
		}
		static FreqTable makeTable(string curWord, string[] lines) {
			var table = new FreqTable();
			table.curWord = curWord;
			table.words = new List<string>();
			List<string> _words = new List<string>();
			foreach (string line in lines){
				_words.AddRange(line.Split(" "));
			}
			for (int i = 0; i < _words.Count - 1; i++){ // this crashes sometimes if the last word of the file is chosen, thats why there is a -1 here
				if (_words[i] != curWord || _words[i+1] == null) continue;
				table.words.Add(_words[i+1]);
			}
			return table;
		}
	}

	[System.Serializable]
	public struct FreqTable {
		public string curWord;
		public List</*FreqPair*/ string> words; //pairs;
	}

	/*
	public struct FreqPair {
		public string nextWord;
		public int freq;
	}
	*/

	[System.Serializable]
	public class Cache {
		public string list_checksum;
		public List<string> firstWords = new List<string>();
		public Dictionary<string, FreqTable> frequencies = new Dictionary<string, FreqTable>();
		public static Cache load(){
			FileStream file = new FileStream(".cache", FileMode.OpenOrCreate);
			Cache cache = null;
			try {
				BinaryFormatter formatter = new BinaryFormatter();

				// Deserialize the hashtable from the file and
				// assign the reference to the local variable.
				cache = (Cache) formatter.Deserialize(file);
			} catch (SerializationException e) {
				Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
				throw;
			} finally {
				file.Close();
			}
			return cache;
		}
		public Cache(string checksum){
			this.list_checksum = checksum;
			// todo stub
		}
		public void save(){
			FileStream file = new FileStream(".cache", FileMode.Truncate);
			BinaryFormatter formatter = new BinaryFormatter();
			try {
				formatter.Serialize(file, this);
			} catch (SerializationException e) {
				Console.WriteLine("Failed to serialize. Reason: " + e.Message);
				throw;
			} finally {
				file.Close();
			}
		}
	}

	public struct Config {
		public bool useCache;
		public string workingDir;
		public int length;
	}

	public class ConfigBuilder {
		bool useCache;
		string workingDir;
		int length;
		public ConfigBuilder() {
			useCache = true;
			if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
				workingDir = Environment.GetEnvironmentVariable("LOCALAPPDATA");
			} else {
				workingDir = "~";
			}
			workingDir += "/.markov";
			setWorkingDirectory(workingDir);
			length = 10;
		}

		public ConfigBuilder setWorkingDirectory(string path, bool verbose = true) {
			try {
			    if (Directory.Exists(path)) {
					if (verbose)
						Console.WriteLine("Path exists already.");
				} else {
					Directory.CreateDirectory(path);
				}
				this.workingDir = path;
			}
			catch (Exception e) {
				if (verbose)
					Console.Error.WriteLine("Failed to Create Working Directory: {0}", e.ToString());
			}

			return this;
		}

		public ConfigBuilder setLength(int len) {
			this.length = len;
			return this;
		}

		public Config build() {
			Environment.CurrentDirectory = workingDir;
			var config = new Config();
			config.useCache = this.useCache;
			config.workingDir = this.workingDir;
			config.length = this.length;
			return config;
		}
	}
}
