﻿//
// MSBuildGlobTests.cs
//
// Author:
//       Lluis Sanchez Gual <lluis@xamarin.com>
//
// Copyright (c) 2016 Xamarin, Inc (http://www.xamarin.com)
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
using System.IO;
using System.Xml;
using NUnit.Framework;
using UnitTests;
using MonoDevelop.CSharp.Project;
using MonoDevelop.Core;
using MonoDevelop.Ide.Projects;
using System.Linq;
using MonoDevelop.Projects.MSBuild;
using System.Threading.Tasks;
using MonoDevelop.Core.Serialization;
using MonoDevelop.Projects.Extensions;

namespace MonoDevelop.Projects
{
	[TestFixture]
	public class MSBuildGlobTests: TestBase
	{
		[Test]
		public async Task RemoveFile ()
		{
			string projFile = Util.GetSampleProject ("msbuild-glob-tests", "glob-test.csproj");
			var p = (DotNetProject)await Services.ProjectService.ReadSolutionItem (Util.GetMonitor (), projFile);
			p.UseAdvancedGlobSupport = true;

			Assert.AreEqual (3, p.Files.Count);

			var f = p.Files.First (fi => fi.FilePath.FileName == "c1.cs");
			p.Files.Remove (f);

			await p.SaveAsync (Util.GetMonitor ());

			string projectXml = File.ReadAllText (p.FileName);
			Assert.AreEqual (projectXml, File.ReadAllText (p.FileName.ChangeName ("glob-test-saved1")));

			p.AddFile (f.FilePath);
			await p.SaveAsync (Util.GetMonitor ());
			Assert.AreEqual (File.ReadAllText (p.FileName), File.ReadAllText (p.FileName.ChangeName ("glob-test-saved2")));
		}

		[Test]
		public async Task AddFile ()
		{
			// When adding a file, if the new file is included in a glob, there is no need to add a new project item.

			string projFile = Util.GetSampleProject ("msbuild-glob-tests", "glob-test.csproj");
			var p = (DotNetProject)await Services.ProjectService.ReadSolutionItem (Util.GetMonitor (), projFile);
			p.UseAdvancedGlobSupport = true;

			Assert.AreEqual (3, p.Files.Count);

			var f = p.Files.First (fi => fi.FilePath.FileName == "c1.cs");

			var newFile = f.FilePath.ChangeName ("c1bis");
			File.Copy (f.FilePath, newFile);

			p.AddFile (newFile);

			string projectXml = File.ReadAllText (p.FileName);
			await p.SaveAsync (Util.GetMonitor ());

			Assert.AreEqual (projectXml, File.ReadAllText (p.FileName.ChangeName ("glob-test-saved2")));
		}

		[Test]
		public async Task AddFileExcludedFromGlob ()
		{
			// Adding a new file is included in a glob, but the glob item has an exclude attribute that excludes the new file.
			// In this case, a new project item should be added

			string projFile = Util.GetSampleProject ("msbuild-glob-tests", "glob-test.csproj");
			var p = (DotNetProject)await Services.ProjectService.ReadSolutionItem (Util.GetMonitor (), projFile);
			p.UseAdvancedGlobSupport = true;

			Assert.AreEqual (3, p.Files.Count);

			var f = p.Files.First (fi => fi.FilePath.FileName == "c1.cs");

			var newFile = f.FilePath.ChangeName ("c9");
			File.Copy (f.FilePath, newFile);

			p.AddFile (newFile);

			await p.SaveAsync (Util.GetMonitor ());

			string projectXml = File.ReadAllText (p.FileName);
			Assert.AreEqual (projectXml, File.ReadAllText (p.FileName.ChangeName ("glob-test-saved4")));
		}

		[Test]
		public async Task AddFileWithMetadata ()
		{
			string projFile = Util.GetSampleProject ("msbuild-glob-tests", "glob-test.csproj");
			var p = (DotNetProject)await Services.ProjectService.ReadSolutionItem (Util.GetMonitor (), projFile);
			p.UseAdvancedGlobSupport = true;

			Assert.AreEqual (3, p.Files.Count);

			var f = p.Files.First (fi => fi.FilePath.FileName == "c1.cs");

			var newFile = f.FilePath.ChangeName ("c1bis");
			File.Copy (f.FilePath, newFile);

			var pf = p.AddFile (newFile);
			pf.Metadata.SetValue ("foo", "bar");

			await p.SaveAsync (Util.GetMonitor ());

			string projectXml = File.ReadAllText (p.FileName);
			Assert.AreEqual (projectXml, File.ReadAllText (p.FileName.ChangeName ("glob-test-saved3")));
		}

		[Test]
		public async Task ImplicitAddFile ()
		{
			string projFile = Util.GetSampleProject ("msbuild-glob-tests", "glob-auto-include.csproj");
			var p = (DotNetProject)await Services.ProjectService.ReadSolutionItem (Util.GetMonitor (), projFile);
			p.UseAdvancedGlobSupport = true;

			Assert.AreEqual (0, p.Files.Count);

			// Add file matching first glob

			var f = p.ItemDirectory.Combine ("a1.cs");
			File.WriteAllText (f, "");

			var res = p.AddItemsForFileIncludedInGlob (f).ToArray ();

			Assert.AreEqual (1, res.Length);
			Assert.IsInstanceOf<ProjectFile> (res[0]);
			Assert.AreEqual ("Compile", res[0].ItemName);
			Assert.AreEqual (1, p.Files.Count);
			Assert.IsTrue (p.Files.Contains (res[0]));

			string projectXml = File.ReadAllText (p.FileName);
		
			await p.SaveAsync (Util.GetMonitor ());
			Assert.AreEqual (projectXml, File.ReadAllText (p.FileName));

			// Add file matching second glob

			f = p.ItemDirectory.Combine ("1b.cs");
			File.WriteAllText (f, "");

			res = p.AddItemsForFileIncludedInGlob (f).ToArray ();

			Assert.AreEqual (1, res.Length);
			Assert.IsInstanceOf<ProjectFile> (res [0]);
			Assert.AreEqual ("Extra", res [0].ItemName);
			Assert.AreEqual (2, p.Files.Count);
			Assert.IsTrue (p.Files.Contains (res [0]));

			await p.SaveAsync (Util.GetMonitor ());
			Assert.AreEqual (projectXml, File.ReadAllText (p.FileName));

			// Add file matching both globs

			f = p.ItemDirectory.Combine ("a1b.cs");
			File.WriteAllText (f, "");

			res = p.AddItemsForFileIncludedInGlob (f).ToArray ();

			Assert.AreEqual (2, res.Length);
			Assert.IsInstanceOf<ProjectFile> (res [0]);
			Assert.IsInstanceOf<ProjectFile> (res [1]);
			Assert.AreEqual ("Compile", res [0].ItemName);
			Assert.AreEqual ("Extra", res [1].ItemName);
			Assert.AreEqual (4, p.Files.Count);
			Assert.IsTrue (p.Files.Contains (res [0]));
			Assert.IsTrue (p.Files.Contains (res [1]));

			await p.SaveAsync (Util.GetMonitor ());
			Assert.AreEqual (projectXml, File.ReadAllText (p.FileName));
		}

		[Test]
		public async Task ModifyFileWithMetadata ()
		{
			// When the metadata of a file included by a glob changes (so it doesn't match the glob anymore)
			// then the file has to be excluded from the glob

			string projFile = Util.GetSampleProject ("msbuild-glob-tests", "glob-test.csproj");
			var p = (DotNetProject)await Services.ProjectService.ReadSolutionItem (Util.GetMonitor (), projFile);
			p.UseAdvancedGlobSupport = true;

			Assert.AreEqual (3, p.Files.Count);

			var f = p.Files.First (fi => fi.FilePath.FileName == "c2.cs");
			f.Metadata.SetValue ("foo", "bar");

			await p.SaveAsync (Util.GetMonitor ());

			string projectXml = File.ReadAllText (p.FileName);
			Assert.AreEqual (projectXml, File.ReadAllText (p.FileName.ChangeName ("glob-test-saved5")));
		}
	}
}