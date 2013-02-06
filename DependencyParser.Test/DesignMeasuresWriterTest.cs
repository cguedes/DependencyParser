﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Mono.Cecil;
using NUnit.Framework;

namespace DependencyParser.Test {

	[NUnit.Framework.TestFixture]
	public class DesignMeasuresWriterTest {
		[Test]
		public void Should_Generate()
		{
			File.Delete("lcom4-generated.xml");
			using (var stream = new FileStream("lcom4-generated.xml", FileMode.Create))
			{
				using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
				{
					writer.Formatting = Formatting.Indented;
					var designWriter = new DesignMeasuresWriter();
					var typeDefinition = getType("DependencyParser.Test.SimpleClassWithTwoFields");
					var blocks = new HashSet<HashSet<MemberReference>>();
				
					foreach (var mth in typeDefinition.Methods)
					{
						var block = new HashSet<MemberReference> { mth, typeDefinition.Fields.First() };
						blocks.Add(block);
					}
					designWriter.Xml = writer;
					designWriter.Type = typeDefinition;
					designWriter.Lcom4Blocks = blocks;
					designWriter.ResponseForClass = 42;
					designWriter.DethOfInheritance = 17;

					designWriter.Write();
				}
			}
			string expected = File.ReadAllText("lcom4-expected.xml");
			string result = File.ReadAllText("lcom4-generated.xml");

			Assert.AreEqual(expected, result);
		}

		private TypeDefinition getType(string name)
		{
			string unit = Assembly.GetExecutingAssembly().Location;
			var assembly = AssemblyDefinition.ReadAssembly(unit);
			return assembly.MainModule.GetType(name);
		}
	}

	public class SimpleClassWithTwoFields {
		private int fieldA;

		private int fieldB;

		public void doA()
		{
			fieldA++;
		}

		public void doB()
		{
			fieldB++;
		}
	}
}
