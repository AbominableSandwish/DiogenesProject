#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
public class ConstructionSiteTests
{
    [Test]
    public void Progress_StartsAtZero()
    {
        ConstructionSite site = new ConstructionSite(new FakeStructure());
        Assert.AreEqual(0f, site.Progress);
    }

    [Test]
    public void Progress_Increases_WhenWorkIsAdded()
    {
        ConstructionSite site = new ConstructionSite(new FakeStructure());
        site.AddWork(50f);
        Assert.AreEqual(0.5f, site.Progress);
    }

    [Test]
    public void Site_IsComplete_WhenEnoughWorkIsAdded()
    {
        ConstructionSite site = new ConstructionSite(new FakeStructure());
        site.AddWork(100f);
        Assert.IsTrue(site.IsCompleted);
    }

    [Test]
    public void Site_Uses_TargetStructureLayer()
    {
        FakeStructure target = new FakeStructure();
        ConstructionSite site = new ConstructionSite(target);
        Assert.AreEqual(target.Layer, site.Layer);
    }
}
#endif