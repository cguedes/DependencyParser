﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using NUnit.Framework;

namespace DependencyParser.Test {

	[NUnit.Framework.TestFixture]
	public class DethOfInheritanceTreeAnalyzerTest {

		[Test]
		public void Should_Compute_DIT()
		{
			var t = GetType("DependencyParser.Test.DethOfInheritanceTreeAnalyzerTest");
			var result = new DethOfInheritanceTreeAnalyzer().ComputeDIT(t);
			Assert.AreEqual(1, result);
		}


		private TypeDefinition GetType(string name)
		{
			string unit = Assembly.GetExecutingAssembly().Location;
			var assembly = AssemblyDefinition.ReadAssembly(unit);
			return assembly.MainModule.GetType(name);
		}
	}
}
