namespace TomsToolbox.Essentials.Tests;

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class SemanticVersionTest
{
    [TestMethod]
    public void ParsesVersionsCorrectly()
    {
        var target12 = SemanticVersion.Parse("1.2");
        var target123 = SemanticVersion.Parse("1.2.3");
        var target1234 = SemanticVersion.Parse("Version 1.2.3.4");
        var target12beta = SemanticVersion.Parse("1.2-beta");
        var target12beta2 = SemanticVersion.Parse("1.2-beta2");
        var target123alpha = SemanticVersion.Parse("V 1.2.3-alpha etc.");
        var target1234alpha = SemanticVersion.Parse("1.2.3.4-alpha");
        var target1234_2 = SemanticVersion.Parse("V 1.2.3.4 -alpha");

        Assert.AreEqual(string.Empty, target1234_2.Suffix);
        Assert.AreEqual("-alpha", target1234alpha.Suffix);
        Assert.AreEqual(new Version(1, 2, 3, 4), target1234alpha.Version);
        Assert.AreEqual("-beta", target12beta.Suffix);
        Assert.AreEqual("1.2.3-alpha", target123alpha.ToString());
        Assert.AreEqual("1.2", target12.ToString());

        Assert.AreEqual(target1234, target1234_2);
        Assert.IsTrue(target123 > target12);
        Assert.IsTrue(target123alpha > target12);
        Assert.IsTrue(target12beta < target12);
        Assert.IsTrue(target12beta < target12beta2);
        Assert.IsTrue(target1234alpha > target123);
        Assert.IsTrue(target1234alpha < target1234);
    }
}