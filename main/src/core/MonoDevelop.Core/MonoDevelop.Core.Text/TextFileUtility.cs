﻿//
// TextFileUtility.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MonoDevelop.Core.Text
{
	/// <summary>
	/// This class handles text input from files, streams and byte arrays with auto-detect encoding.
	/// </summary>
	public static class TextFileUtility
	{
		readonly static int maxBomLength = 0;
		readonly static Encoding[] encodingsWithBom;

		static TextFileUtility ()
		{
			var encodings = new List<Encoding> ();

			foreach (var info in Encoding.GetEncodings ()) {
				Encoding encoding;
				try {
					encoding = info.GetEncoding ();
				} catch (NotSupportedException) {
					continue;
				}
				var bom = encoding.GetPreamble ();
				if (bom == null || bom.Length == 0)
					continue;
				maxBomLength = Math.Max (maxBomLength, bom.Length);
				encodings.Add (encoding);
			}
			encodingsWithBom = encodings.ToArray ();

			// Encoding verifiers
			var verifierList = new List<Verifier> {
				new Utf8Verifier (),
				new GB18030CodePageVerifier (),
				new WindowsCodePageVerifier (),
				new UnicodeVerifier (),
				new BigEndianUnicodeVerifier (),
				new CodePage858Verifier ()
			};

			verifiers = verifierList.Where (v => v.IsSupported).ToArray ();

			// cache the verifier machine state tables, to do the virtual StateTable only once.
			stateTables = new byte[verifiers.Length][][];
			for (int i = 0; i < verifiers.Length; i++) {
				verifiers [i].Initialize ();
				stateTables [i] = verifiers [i].StateTable;
			}
		}

		#region stream reader methods
		public static StreamReader OpenStream (string fileName)
		{
			bool hadBom;
			return OpenStream (File.ReadAllBytes (fileName), out hadBom);
		}

		public static StreamReader OpenStream (string fileName, out bool hadBom)
		{
			return OpenStream (File.ReadAllBytes (fileName), out hadBom);
		}

		public static StreamReader OpenStream (byte[] bytes)
		{
			bool hadBom;
			return OpenStream (bytes, out hadBom);
		}

		public static StreamReader OpenStream (byte[] bytes, out bool hadBom)
		{
			if (bytes == null)
				throw new ArgumentNullException ("bytes");
			return OpenStream (new MemoryStream (bytes, false), out hadBom);
		}

		public static StreamReader OpenStream (Stream stream)
		{
			bool hadBom;
			return OpenStream (stream, out hadBom);
		}

		public static StreamReader OpenStream (Stream stream, out bool hadBom)
		{
			if (stream == null)
				throw new ArgumentNullException ("stream");
			byte[] possibleBom = new byte[maxBomLength];
			stream.Read (possibleBom, 0, Math.Min ((int)stream.Length, maxBomLength));

			foreach (var encoding in encodingsWithBom) {
				var bom = encoding.GetPreamble ();
				bool invalid = false;
				for (int i = 0; i < bom.Length; i++) {
					if (bom [i] != possibleBom [i]) {
						invalid = true;
						break;
					}
				}

				if (!invalid) {
					hadBom = true;
					stream.Position = bom.Length;
					return new StreamReader (stream, encoding);
				}
			}
			stream.Position = 0;
			hadBom = false;
			return new StreamReader (stream, AutoDetectEncoding (stream));
		}
		#endregion

		#region string methods
		public static string GetText (byte[] bytes)
		{
			using (var stream = OpenStream (bytes)) {
				return stream.ReadToEnd ();
			}
		}

		public static string GetText (byte[] bytes, out Encoding encoding, out bool hadBom)
		{
			if (bytes == null)
				throw new ArgumentNullException ("bytes");
			using (var stream = OpenStream (bytes, out hadBom)) {
				encoding = stream.CurrentEncoding;
				return stream.ReadToEnd ();
			}
		}

		public static string GetText (byte[] bytes, Encoding encoding, out bool hadBom)
		{
			byte[] bom = encoding.GetPreamble ();
			if (bom != null && bom.Length > 0 && bom.Length <= bytes.Length) {
				hadBom = true;
				for (int i = 0; i < bom.Length; i++) {
					if (bytes [i] != bom [i]) {
						hadBom = false;
						break;
					}
				}
			} else {
				hadBom = false;
			}
			if (hadBom) 
				return encoding.GetString (bytes, bom.Length, bytes.Length - bom.Length);
			return encoding.GetString (bytes);
		}

		public static string GetText (Stream inputStream)
		{
			using (var stream = OpenStream (inputStream)) {
				return stream.ReadToEnd ();
			}
		}

		public static string GetText (Stream inputStream, out Encoding encoding, out bool hadBom)
		{
			if (inputStream == null)
				throw new ArgumentNullException ("inputStream");
			using (var stream = OpenStream (inputStream, out hadBom)) {
				encoding = stream.CurrentEncoding;
				return stream.ReadToEnd ();
			}
		}

		public static string GetText (string fileName)
		{
			using (var stream = OpenStream (fileName)) {
				return stream.ReadToEnd ();
			}
		}

		public static string GetText (string fileName, out Encoding encoding, out bool hadBom)
		{
			if (fileName == null)
				throw new ArgumentNullException ("fileName");
			using (var stream = OpenStream (fileName, out hadBom)) {
				encoding = stream.CurrentEncoding;
				return stream.ReadToEnd ();
			}
		}

		#endregion

		#region file methods
		public static void WriteText (string fileName, string text, Encoding encoding, bool hadBom)
		{
			if (fileName == null)
				throw new ArgumentNullException ("fileName");
			if (text == null)
				throw new ArgumentNullException ("text");
			if (encoding == null)
				throw new ArgumentNullException ("encoding");
			// atomic rename only works in the same directory on linux. The tmp files may be on another partition -> breaks save.
			string tmpPath = Path.Combine (Path.GetDirectoryName (fileName), ".#" + Path.GetFileName (fileName));
			using (var stream = new FileStream (tmpPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write)) {
				if (hadBom) {
					var bom = encoding.GetPreamble ();
					if (bom != null && bom.Length > 0)
						stream.Write (bom, 0, bom.Length);
				}
				byte[] bytes = encoding.GetBytes (text);
				stream.Write (bytes, 0, bytes.Length);
			}
			try {
				SystemRename (tmpPath, fileName);
			} catch (Exception) {
				try {
					File.Delete (tmpPath);
				} catch {
					// nothing
				}
				throw;
			}
		}

		/// <summary>
		/// Returns a byte array containing the text encoded by a specified encoding &amp; bom.
		/// </summary>
		/// <param name="text">The text to encode.</param>
		/// <param name="encoding">The encoding.</param>
		/// <param name="hadBom">If set to <c>true</c> a bom will be prepended.</param>
		public static byte[] GetBuffer (string text, Encoding encoding, bool hadBom)
		{
			using (var stream = new MemoryStream ()) {
				if (hadBom) {
					var bom = encoding.GetPreamble ();
					if (bom != null && bom.Length > 0)
						stream.Write (bom, 0, bom.Length);
				}
				byte[] bytes = encoding.GetBytes (text);
				stream.Write (bytes, 0, bytes.Length);
				return stream.GetBuffer ();
			}
		}

		// Code taken from FileService.cs
		static void SystemRename (string sourceFile, string destFile)
		{
			//FIXME: use the atomic System.IO.File.Replace on NTFS
			if (Platform.IsWindows) {
				string wtmp = null;
				if (File.Exists (destFile)) {
					do {
						wtmp = Path.Combine (Path.GetTempPath (), Guid.NewGuid ().ToString ());
					} while (File.Exists (wtmp));
					File.Move (destFile, wtmp);
				}
				try {
					File.Move (sourceFile, destFile);
				} catch {
					try {
						if (wtmp != null)
							File.Move (wtmp, destFile);
					} catch {
						wtmp = null;
					}
					throw;
				} finally {
					if (wtmp != null) {
						try {
							File.Delete (wtmp);
						} catch {
						}
					}
				}
			} else {
				Mono.Unix.Native.Syscall.rename (sourceFile, destFile);
			}
		}

		public static string ReadAllText (string fileName)
		{
			bool hadBom;
			Encoding encoding;
			return ReadAllText (fileName, out hadBom, out encoding);
		}

		public static string ReadAllText (string fileName, out bool hadBom, out Encoding encoding)
		{
			if (fileName == null)
				throw new ArgumentNullException ("fileName");
			using (var stream = new FileStream (fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				return GetText (stream, out encoding, out hadBom);
			}
		}

		public static string ReadAllText (string fileName, Encoding encoding, out bool hadBom)
		{
			if (fileName == null)
				throw new ArgumentNullException ("fileName");
			if (encoding == null)
				throw new ArgumentNullException ("encoding");

			byte[] content = File.ReadAllBytes (fileName);
			return GetText (content, encoding, out hadBom); 
		}
		#endregion

		#region ASCII encoding check
		public static bool IsASCII (string text)
		{
			if (text == null)
				throw new ArgumentNullException ("text");
			for (int i = 0; i < text.Length; i++) {
				var ch = text [i];
				if (ch > 0x7F)
					return false;
			}
			return true;
		}
		#endregion

		#region Binary check
		public static bool IsBinary (byte[] bytes)
		{
			if (bytes == null)
				throw new ArgumentNullException ("bytes");
			return IsBinary (new MemoryStream (bytes, false));
		}

		public static bool IsBinary (string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException ("fileName");
			using (var stream = new FileStream (fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				return IsBinary (stream);
			}
		}

		public static bool IsBinary (Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException ("stream");

			var enc = AutoDetectEncoding (stream);
			return enc == Encoding.ASCII;
		}
		#endregion

		#region Encoding autodetection
		static readonly Verifier[] verifiers;
		static readonly byte[][][] stateTables;

		const int maxBufferLength = 50 * 1024;
		static Queue<EncodingCache> caches = new Queue<EncodingCache>();
		class EncodingCache
		{
			internal byte[] readBuf;
			internal byte[] states;

			internal static EncodingCache Create()
			{
				return new EncodingCache () {
					readBuf =  new byte[maxBufferLength],
					states = new byte[verifiers.Length]
				};
			}
		}

		static EncodingCache RequestCache()
		{
			lock (caches) {
				if (caches.Count > 0)
					return caches.Dequeue ();
				return EncodingCache.Create ();
			}
		}

		static void PutbackCache (EncodingCache cache)
		{
			lock (caches) {
				caches.Enqueue (cache);
			}
		}

		static unsafe Encoding AutoDetectEncoding (Stream stream)
		{
			var cache = RequestCache ();
			try {
				var readBuf = cache.readBuf;
				var states = cache.states;
				int max = (int)System.Math.Min (stream.Length, maxBufferLength);
				int readLength = stream.Read (readBuf, 0, max);
				stream.Position = 0;

				// Store the dfa data from the verifiers in local variables.
				int verifiersRunning = verifiers.Length;

				for (int i = 0; i < verifiers.Length; i++)
					states [i] = verifiers [i].InitalState;

				// run the verifiers
				fixed (byte* bBeginPtr = readBuf, stateBeginPtr = states) {
					byte* bPtr = bBeginPtr;
					byte* bEndPtr = bBeginPtr + readLength;
					byte* sEndPtr = stateBeginPtr + states.Length;

					while (bPtr != bEndPtr) {
						byte* sPtr = stateBeginPtr;
						int i = 0;
						while (sPtr != sEndPtr) {
							byte curState = *sPtr;
							if (curState != 0) {
								curState = stateTables [i] [curState] [*bPtr];
								if (curState == 0) {
									verifiersRunning--;
									if (verifiersRunning == 0) 
										goto finishVerify;
								}
								*sPtr = curState;
							}
							sPtr++;
							i++;
						}
						bPtr++;
					}
					finishVerify:
					if (verifiersRunning > 0) {
						//						Console.WriteLine ("valid encodings:");
						//						for (int i = 0; i < verifiers.Length; i++) {
						//							if (verifiers [i].IsEncodingValid (states [i]))
						//								Console.WriteLine (verifiers [i].Encoding.EncodingName);
						//						}
						//						Console.WriteLine ("---------------");
						for (int i = 0; i < verifiers.Length; i++) {
							if (verifiers [i].IsEncodingValid (states [i])) {
								PutbackCache (cache);
								return verifiers [i].Encoding;
							}
						}
					}
				}

			} catch (Exception e) {
				Console.WriteLine (e);
			}
			PutbackCache (cache);
			return Encoding.ASCII;
		}

		abstract class Verifier
		{
			internal const byte Error = 0;
			protected static readonly byte[] errorTable = new byte[(int)byte.MaxValue + 1];

			public abstract byte InitalState { get; }

			public abstract Encoding Encoding { get; }

			public abstract byte[][] StateTable { get; }

			protected abstract void Init ();

			bool isInitialized = false;

			public void Initialize ()
			{
				if (isInitialized)
					throw new InvalidOperationException ("Already initialized");
				isInitialized = true;
				Init ();
			}

			public abstract bool IsSupported { get; }

			public virtual bool IsEncodingValid (byte state)
			{
				return state != Error; 
			}
		}

		class Utf8Verifier : Verifier
		{
			const byte UTF1 = 1;
			const byte UTFTail1 = 2;
			const byte UTFTail2 = 3;
			const byte UTFTail3 = 4;
			const byte UTF8_3_TailPre1 = 5;
			const byte UTF8_3_TailPre2 = 6;
			const byte UTF8_4_TailPre1 = 7;
			const byte UTF8_4_TailPre2 = 8;
			const byte LAST = 9;
			static byte[][] table;

			public override bool IsSupported {
				get {
					try {
						return Encoding.UTF8 != null;
					} catch (Exception) {
						return false;
					}
				}
			}

			protected override void Init ()
			{
				table = new byte[LAST][];
				table [0] = errorTable;
				for (int i = 1; i < LAST; i++)
					table [i] = new byte[(int)byte.MaxValue + 1];

				// UTF8-1      = %x00-7F
				// Take out the 0 case, that indicates a UTF16/32 file.
				for (int i = 0x00; i <= 0x7F; i++) {
					table [UTF1] [i] = UTF1;
				}

				// UTF8-tail   = %x80-BF
				for (int i = 0x80; i <= 0xBF; i++) {
					table [UTFTail1] [i] = UTF1;
					table [UTFTail2] [i] = UTFTail1;
					table [UTFTail3] [i] = UTFTail2;
				}

				// UTF8-2 = %xC2-DF UTF8-tail
				for (int i = 0xC2; i <= 0xDF; i++)
					table [UTF1] [i] = UTFTail1;

				// UTF8-3      = %xE0 %xA0-BF UTF8-tail / %xE1-EC 2( UTF8-tail ) /
				//               %xED %x80-9F UTF8-tail / %xEE-EF 2( UTF8-tail )
				for (int i = 0xA0; i <= 0xBF; i++) {
					table [UTF8_3_TailPre1] [i] = UTFTail1;
				}
				for (int i = 0x80; i <= 0x9F; i++) {
					table [UTF8_3_TailPre2] [i] = UTFTail1;
				}

				table [UTF1] [0xE0] = UTF8_3_TailPre1;
				for (int i = 0xE1; i <= 0xEC; i++)
					table [UTF1] [i] = UTFTail2;
				table [UTF1] [0xED] = UTF8_3_TailPre2;
				for (int i = 0xEE; i <= 0xEF; i++)
					table [UTF1] [i] = UTFTail2;

				// UTF8-4      = %xF0 %x90-BF 2( UTF8-tail ) / %xF1-F3 3( UTF8-tail ) /
				//               %xF4 %x80-8F 2( UTF8-tail )

				for (int i = 0x90; i <= 0xBF; i++) {
					table [UTF8_4_TailPre1] [i] = UTFTail2;
				}
				for (int i = 0x80; i <= 0xBF; i++) {
					table [UTF8_4_TailPre2] [i] = UTFTail2;
				}
				table [UTF1] [0xF0] = UTF8_4_TailPre1;
				for (int i = 0xF1; i <= 0xF3; i++)
					table [UTF1] [i] = UTFTail3;
				table [UTF1] [0xF4] = UTF8_4_TailPre2;

				// always invalid.
				for (int i = 0; i < table.Length; i++) {
					table [i] [0xC0] = Error;
					table [i] [0xC1] = Error;
					table [i] [0xF5] = Error;
					table [i] [0xFF] = Error;
				}
			}

			public override byte InitalState { get { return UTF1; } }

			public override Encoding Encoding { get { return Encoding.UTF8; } }

			public override byte[][] StateTable { get { return table; } }
		}

		/// <summary>
		/// Unicode verifier
		/// </summary> 
		class UnicodeVerifier : Verifier
		{
			const byte Even = 1;
			const byte Odd = 2;
			const byte EvenPossible = 3;
			const byte OddPossible = 4;
			const byte LAST = 5;
			static byte[][] table;

			protected override void Init ()
			{
				// Simple approach - detect 0 at odd posititons, then it's likely a utf16
				// if 0 at an even position it's regarded as no utf-16.
				table = new byte[LAST][];
				table [0] = errorTable;
				for (int i = 1; i < LAST; i++)
					table [i] = new byte[(int)byte.MaxValue + 1];

				for (int i = 0x00; i <= 0xFF; i++) {
					table [Even] [i] = Odd;
					table [Odd] [i] = Even;
					table [EvenPossible] [i] = OddPossible;
					table [OddPossible] [i] = EvenPossible;
				}
				table [Odd] [0] = EvenPossible;
				table [Even] [0] = Error;
				table [EvenPossible] [0] = Error;
			}

			public override byte InitalState { get { return Even; } }

			public override Encoding Encoding { get { return Encoding.Unicode; } }

			public override byte[][] StateTable { get { return table; } }

			public override bool IsSupported {
				get {
					try {
						return Encoding.Unicode != null;
					} catch (Exception) {
						return false;
					}
				}
			}

			public override bool IsEncodingValid (byte state)
			{
				return state == EvenPossible || state == OddPossible;
			}
		}

		class BigEndianUnicodeVerifier : Verifier
		{
			const byte Even = 1;
			const byte Odd = 2;
			const byte EvenPossible = 3;
			const byte OddPossible = 4;
			const byte LAST = 5;

			public override byte InitalState { get { return Even; } }

			public override Encoding Encoding { get { return Encoding.BigEndianUnicode; } }

			public override byte[][] StateTable { get { return table; } }

			public override bool IsSupported {
				get {
					try {
						return Encoding.BigEndianUnicode != null;
					} catch (Exception) {
						return false;
					}
				}
			}

			public override bool IsEncodingValid (byte state)
			{
				return state == EvenPossible || state == OddPossible;
			}

			static byte[][] table;

			protected override void Init ()
			{
				// Simple approach - detect 0 at even posititons, then it's likely a utf16be
				// if 0 at an odd position it's regarded as no utf-16be.
				table = new byte[LAST][];
				table [0] = errorTable;
				for (int i = 1; i < LAST; i++)
					table [i] = new byte[(int)byte.MaxValue + 1];

				for (int i = 0x00; i <= 0xFF; i++) {
					table [Even] [i] = Odd;
					table [Odd] [i] = Even;
					table [EvenPossible] [i] = OddPossible;
					table [OddPossible] [i] = EvenPossible;
				}
				table [Odd] [0] = Error;
				table [OddPossible] [0] = Error;
				table [Even] [0] = OddPossible;
			}
		}

		/// <summary>
		/// Code page 1252 was the long time default on windows. This encoding is a superset of ISO 8859-1.
		/// </summary>
		class WindowsCodePageVerifier : Verifier
		{
			const byte Valid = 1;
			const byte LAST = 2;
			static byte[][] table;
			static Encoding EncodingWindows;

			public override byte InitalState { get { return Valid; } }

			public override Encoding Encoding { get { return EncodingWindows; } }

			public override byte[][] StateTable { get { return table; } }


			const int westernEncodingCodePage = 1252;
			/// <summary>
			/// Try to guess the windows code page using the default encoding, on non windows system default
			/// to 1252 (western encoding).
			/// </summary>
			int WindowsCodePage {
				get {
					if (Platform.IsWindows) {
						int cp = Encoding.Default.CodePage;
						if (cp >= 1250 && cp < 1260)
							return cp;
					}
					return westernEncodingCodePage;
				}
			}

			public override bool IsSupported {
				get {
					try {
						return Encoding.GetEncoding (WindowsCodePage) != null;
					} catch (Exception) {
						return false;
					}
				}
			}

			protected override void Init ()
			{
				EncodingWindows = Encoding.GetEncoding (WindowsCodePage);
				table = new byte[LAST][];
				table [0] = errorTable;
				for (int i = 1; i < LAST; i++)
					table [i] = new byte[(int)byte.MaxValue + 1];

				for (int i = 0x00; i <= 0xFF; i++) {
					table [Valid] [i] = Valid;
				}
				table [Valid] [0x81] = Error;
				table [Valid] [0x8D] = Error;
				table [Valid] [0x8F] = Error;
				table [Valid] [0x90] = Error;
				table [Valid] [0x9D] = Error;
			}
		}

		/// <summary>
		/// Code page 858 supports old DOS style files extended with the euro sign.
		/// </summary>
		class CodePage858Verifier : Verifier
		{
			const byte Valid = 1;
			const byte LAST = 2;
			static byte[][] table;
			static Encoding EncodingCp858;

			public override byte InitalState { get { return Valid; } }

			public override Encoding Encoding { get { return EncodingCp858; } }

			public override byte[][] StateTable { get { return table; } }

			public override bool IsSupported {
				get {
					try {
						return Encoding.GetEncoding (858) != null;
					} catch (Exception) {
						return false;
					}
				}
			}

			protected override void Init ()
			{
				EncodingCp858 = Encoding.GetEncoding (858);
				table = new byte[LAST][];
				table [0] = errorTable;
				for (int i = 1; i < LAST; i++)
					table [i] = new byte[(int)byte.MaxValue + 1];

				for (int i = 0x20; i <= 0xFF; i++) {
					table [Valid] [i] = Valid;
				}
			}
		}

		/// <summary>
		/// Try to detect chinese encoding.
		/// </summary>
		class GB18030CodePageVerifier : Verifier
		{
			const byte Valid  = 1;
			const byte Second = 2;
			const byte Third  = 3;
			const byte Fourth  = 4;
			const byte NotValid  = 5;

			const byte LAST = 6;
			static byte[][] table;
			static Encoding EncodingWindows;

			public override byte InitalState { get { return NotValid; } }

			public override Encoding Encoding { get { return EncodingWindows; } }

			public override byte[][] StateTable { get { return table; } }

			public override bool IsEncodingValid (byte state)
			{
				return state == Valid; 
			}

			int WindowsCodePage {
				get {
					return 54936;
				}
			}

			public override bool IsSupported {
				get {
					try {
						return Encoding.GetEncoding (WindowsCodePage) != null;
					} catch (Exception) {
						return false;
					}
				}
			}

			protected override void Init ()
			{
				EncodingWindows = Encoding.GetEncoding (WindowsCodePage);
				table = new byte[LAST][];
				table [0] = errorTable;
				for (int i = 1; i < LAST; i++)
					table [i] = new byte[(int)byte.MaxValue + 1];

				for (int i = 0x00; i <= 0x80; i++)
					table [Valid] [i] = Valid;
				for (int i = 0x81; i <= 0xFE; i++)
					table [Valid] [i] = Second;
				table [Valid] [0xFF] = Error;

				// need to encounter a multi byte sequence first.
				for (int i = 0x00; i <= 0x80; i++)
					table [NotValid] [i] = NotValid;
				for (int i = 0x81; i <= 0xFE; i++)
					table [NotValid] [i] = Second;
				table [NotValid] [0xFF] = Error;

				for (int i = 0x00; i <= 0xFF; i++)
					table [Second] [i] = Error;
				for (int i = 0x40; i <= 0xFE; i++)
					table [Second] [i] = Valid;
				for (int i = 0x30; i <= 0x39; i++)
					table [Second] [i] = Third;

				for (int i = 0x00; i <= 0xFF; i++)
					table [Third] [i] = Error;
				for (int i = 0x81; i <= 0xFE; i++)
					table [Third] [i] = Fourth;

				for (int i = 0x00; i <= 0xFF; i++)
					table [Fourth] [i] = Error;
				for (int i = 0x30; i <= 0x39; i++)
					table [Fourth] [i] = Valid;
			}
		}
		#endregion
	}
}