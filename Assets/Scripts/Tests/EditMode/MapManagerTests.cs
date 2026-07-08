#if UNITY_INCLUDE_TESTS
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

    [Test]
    public void RemoveStructure_Should_Remove_Existing_Structure()
    {
        // Arrange
        Vector3Int position = new Vector3Int(1, 1, 0);
        Structure structure = new FakeStructure();

        mapManager.AddStructure(structure, position);

        // Act
        mapManager.RemoveStructure(structure, position);

        // Assert
        Structure result = mapManager.GetStructure(position, structure.Layer);

        Assert.IsNull(result);
    }

    [Test]
    public void RemoveStructure_OnEmptyCell_Should_NotCrash()
    {
        Vector3Int position = new Vector3Int(5, 5, 0);

        Assert.DoesNotThrow(() =>
        {
            mapManager.RemoveStructure(new FakeStructure(), position);
        });
    }
}
#endif