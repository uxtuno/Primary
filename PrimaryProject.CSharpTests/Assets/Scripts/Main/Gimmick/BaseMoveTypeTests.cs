using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
	[TestClass()]
	public class BaseMoveTypeTests
	{
		[TestMethod()]
		public void CreateTest()
		{
			BaseMoveType baseMoveType = BaseMoveType.Create(null, BaseMoveType.MoveType.Circulating, null);


			Assert.Fail();
		}

		[TestMethod()]
		public void SetReverseTest()
		{
			Assert.Fail();
		}

		[TestMethod()]
		public void ChangeStateTest()
		{
			Assert.Fail();
		}

		[TestMethod()]
		public void UpdateTest()
		{
			Assert.Fail();
		}

		[TestMethod()]
		public void CollisionDisappearanceTest()
		{
			Assert.Fail();
		}

		[TestMethod()]
		public void GetCurrentControlPointTest()
		{
			Assert.Fail();
		}

		[TestMethod()]
		public void GetNextControlPointTest()
		{
			Assert.Fail();
		}

		[TestMethod()]
		public void SwitchTest()
		{
			Assert.Fail();
		}
	}
}