﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using NUnit.Framework;

namespace DependencyParser.Test {


	[NUnit.Framework.TestFixture]
	public class Lcom4AnalyzerTest {
		[Test]
		public void Should_Find_One_Block_On_A_Simple_Class()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.SimpleClass"));
			Assert.AreEqual(1, blocks.Count);
		}

		[Test]
		public void Should_Find_One_Block_On_A_Class_With_Strong_Cohesion()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.ClassWithManyMethodsAndStrongCohesion"));
			Assert.AreEqual(1, blocks.Count);
			Assert.AreEqual(5, blocks.ElementAt(0).Count);
		}

		[Test]
		public void Should_Ignore_Simple_Accessors()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.ClassWithProperties"));
			Assert.AreEqual(0, blocks.Count);
		}

		[Test]
		public void Should_Take_In_Account_Calls_To_Simple_Accessors()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.ClassWithCallsToAccessors"));
			Assert.AreEqual(1, blocks.Count);
		}

		[Test]
		public void Should_Take_In_Account_Complex_Accessors()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.ClassWithComplexProperties"));
			Assert.AreEqual(2, blocks.Count);
		}

		[Test]
		public void Should_Ignore_Abstract_Methods()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.AbstractClass"));
			Assert.AreEqual(1, blocks.Count);
		}

		[Test]
		public void Should_Take_In_Account_Calls_To_Abstract_Methods()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.AbstractTemplateClass"));
			Assert.AreEqual(1, blocks.Count);
		}

		[Test]
		public void Should_Ignore_Constructors()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.SimpleClassWithCtr"));
			Assert.AreEqual(2, blocks.Count);
		}

		[Test]
		public void Should_Not_Fail_On_Empty_Interface()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.IEmpty"));
			Assert.AreEqual(0, blocks.Count);
		}

		[Test]
		public void Should_Not_Fail_On_Empty_Class()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.EmptyClass"));
			Assert.AreEqual(0, blocks.Count);
		}

		[Test]
		public void Should_Not_Take_In_Account_Dispose_Methods()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.SimpleDisposableClass"));
			Assert.AreEqual(1, blocks.Count);
		}
		
		[Test]
		public void Should_Not_Take_In_Account_Equals_Methods()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.SimpleClassWithEquals"));
			Assert.AreEqual(2, blocks.Count);
		}

		[Test]
		public void Should_Not_Take_In_Account_ToString_Methods()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.SimpleClassWithToString"));
			Assert.AreEqual(2, blocks.Count);
		}

		[Test]
		public void Should_Not_Take_In_Account_Methods_From_Wrapped_Objects()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.SimpleClassWithDelegation"));
			Assert.AreEqual(1, blocks.Count);
			Assert.AreEqual(3, blocks.ElementAt(0).Count);
		}

		[Test]
		public void Should_Not_Take_In_Account_Inherited_Methods()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.DerivedClass"));
			Assert.AreEqual(1, blocks.Count);
			Assert.AreEqual(3, blocks.ElementAt(0).Count);
		}

		[Test]
		public void Should_Not_Take_In_Account_Static_Methods()
		{
			var analyzer = new Lcom4Analyzer();
			var blocks = analyzer.FindLcomBlocks(GetType("DependencyParser.Test.SimpleClassWithStaticMethod"));
			Assert.AreEqual(1, blocks.Count);
		}

		private TypeDefinition GetType(string name)
		{
			string unit = Assembly.GetExecutingAssembly().Location;
			var assembly = AssemblyDefinition.ReadAssembly(unit);
			return assembly.MainModule.GetType(name);
		}

	}

	public class EmptyClass
	{
		
	}

	public interface IEmpty {

	}

	public class SimpleClass
	{
		private int counter;

		public void Increment()
		{
			counter++;
		}

		public void Decrement()
		{
			counter--;
		}
	}

	public class SimpleClassWithCtr {
		private int fieldA;

		private int fieldB;

		public SimpleClassWithCtr()
		{
			fieldA = 42;
			fieldB = 36;
		}

		public void doA()
		{
			fieldA++;
		}

		public void doB()
		{
			fieldB++;
		}
	}

	public class ClassWithManyMethodsAndStrongCohesion {
		
		public void doA()
		{
			Foo();
		}

		public void doB()
		{
			Foo();
			Bar();
		}


		public void doC()
		{
			Bar();
		}

		private void Foo()
		{
			Console.WriteLine("Whatever".ToLower());
		}

		private void Bar()
		{
			Console.WriteLine("Whatever");
		}
	}

	public class ClassWithProperties {
		public int Counter { get; set; }

		public string Name { get; set; }
	}

	public class ClassWithCallsToAccessors {
		public int Counter { get; set; }

		public string Name { get; set; }

		public void Foo()
		{
			Console.WriteLine("Whatever " + Counter + " " + Name);
		}

		public void Bar()
		{
			Console.WriteLine("Whatever " + Counter + " " + Name);
		}
	}

	public class ClassWithComplexProperties {
		private int counter;
		
		public int Counter
		{
			get
			{
				if (counter>10)
				{
					return 10;
				}
				return counter;	
			}
			
			set
			{
				counter = value;
			}
		}

		public void Foo()
		{
			Console.WriteLine("Whatever".ToLower());
		}
	}

	public abstract class AbstractClass {

		public void DoA()
		{
			Foo();
		}

		public void DoB()
		{
			Foo();
		}


		public abstract void DoC();

		private void Foo()
		{
			Console.WriteLine("Whatever".ToLower());
		}
	}

	public abstract class AbstractTemplateClass {

		public void DoA()
		{
			DoC();
		}

		public void DoB()
		{
			DoC();
		}

		public abstract void DoC();

	}

	public class SimpleDisposableClass : IDisposable
	{
		public void DoSomething()
		{
			Console.WriteLine("Whatever");
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}

	public class SimpleClassWithEquals {
		private int fieldA;

		private int fieldB;

		public void DoA()
		{
			fieldA++;
		}

		public void DoB()
		{
			fieldB++;
		}

		public bool Equals(SimpleClassWithEquals other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.fieldA == fieldA && other.fieldB == fieldB;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (SimpleClassWithEquals)) return false;
			return Equals((SimpleClassWithEquals) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (fieldA*397) ^ fieldB;
			}
		}

		public static bool operator ==(SimpleClassWithEquals left, SimpleClassWithEquals right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SimpleClassWithEquals left, SimpleClassWithEquals right)
		{
			return !Equals(left, right);
		}
	}

	public class SimpleClassWithToString {
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

		public override string ToString()
		{
			return fieldA + " " + fieldB;
		}
	}

	public class SimpleClassWithDelegation {
		private SimpleClassWithToString wrapped = new SimpleClassWithToString();

		public void doA()
		{
			wrapped.doA();
		}

		public void doB()
		{
			wrapped.doB();
		}
	}

	public class DerivedClass : SimpleClassWithTwoFields {

		public void doC()
		{
			doA();
		}
		public void doD()
		{
			doA();
			doA();
		}
	}

	public class SimpleClassWithStaticMethod {
		private int counter;

		public void Increment()
		{
			counter++;
		}

		public void Decrement()
		{
			counter--;
		}

		public static SimpleClassWithStaticMethod create()
		{
			Console.WriteLine("factory method");
			return new SimpleClassWithStaticMethod();
		}
	}
}
