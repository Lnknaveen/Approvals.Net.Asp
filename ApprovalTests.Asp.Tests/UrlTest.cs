﻿using ApprovalTests.Asp;

using ApprovalUtilities.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApprovalTests.Asp.Tests
{
	[TestClass]
	public class UrlTest
	{
		[TestMethod]
		public void TestUrlResolving()
		{
			PortFactory.AspPort = 1000;
			var urls = new[]{"~/home","/home","http://www.google.com"};
			Approvals.VerifyAll("Resolving:", urls,
				u => "{0} => {1}".FormatWith(u,AspApprovals.ResolveUrl(u)));
		}
	}
}