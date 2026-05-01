using NUnit.Framework;
using UnityEngine;

public class MapManagerTests
{
    private MapManager mapManager;

    [SetUp]
    public void SetUp()
    {
        GameObject go = new GameObject("MapManager");
        mapManager = go.AddComponent<MapManager>();

        // À adapter selon ton init réelle
        mapManager.InitForTests();
    }

    [Test]
    public void AddStructure_ShouldBeRetrievable()
    {
        Vector3Int position = new Vector3Int(2, 3, 0);
        Structure structure = new FakeStructure();

        mapManager.AddStructure(structure, position);

        Structure result = mapManager.GetStructure(position, structure.Layer);

        Assert.IsNotNull(result);
        Assert.AreSame(structure, result);
    }
}