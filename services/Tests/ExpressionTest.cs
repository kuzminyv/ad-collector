using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Utils;
using Core.Expressions;

namespace Tests
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class ExpressionTest
	{
		[TestMethod]
		public void ExpressionTest_Match()
		{
			BoundExpressionGroup vendorGroup1 = new BoundExpressionGroup("Vendor", new BoundSelector("<div>", "автомобиль1</div> <div>", "</div>"));
            BoundExpressionGroup vendorGroup2 = new BoundExpressionGroup("Vendor", new BoundSelector("<div>", "автомобиль2</div> <div>", "</div>"));
            BoundExpressionGroup yearGroup = new BoundExpressionGroup("Year", new BoundSelector("выпуска <br>", "</br>"));

			string content = @"<div>Продается автомобиль1</div> <div>Opel</div>Год выпуска <br> 2000 </br>
						   Delimitters <div>Продается автомобиль2</div> <div>BMW</div> Год выпуска <br>1111 </br> Delimitters";

			var tokens = new List<BoundExpressionToken>();
			tokens.Add(new BoundExpressionCondition(
                vendorGroup1, new BoundExpressionGroup[]{vendorGroup1, yearGroup}, 
                vendorGroup2, new BoundExpressionGroup[]{vendorGroup2, yearGroup}));

			BoundExpression expr = new BoundExpression(tokens);

			var matches = expr.Matches(content);

			Assert.AreEqual(matches.Count, 2);

			Assert.AreEqual(matches[0]["Vendor"], "Opel");
			Assert.AreEqual(matches[0][yearGroup.Name], "2000");

			Assert.AreEqual(matches[1]["Vendor"], "BMW");
			Assert.AreEqual(matches[1][yearGroup.Name], "1111");

		}
	}
}
