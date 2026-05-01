using NUnit.Framework;
using UnityEngine;

public class CircuitMapTests
{
    private MapManager mapManager;

    [SetUp]
    public void SetUp()
    {
        GameObject go = new GameObject("MapManager");
        mapManager = go.AddComponent<MapManager>();

        mapManager.InitForTests();
    }

    [Test]
    public void AddCoil_Should_CreateCircuit()
    {
        Vector3Int position = new Vector3Int(2, 2, 0);

        Coil coil = new Coil();

        mapManager.AddStructure(coil, position);

        Circuit circuit = mapManager.GetUtilityMap().GetCircuitAt(position);

        Assert.IsNotNull(circuit);
        Assert.IsTrue(circuit._coils.ContainsKey(position));
    }

    [Test]
    public void RemoveCoil_Should_RemoveCircuit_WhenOnlyTile()
    {
        Vector3Int position = new Vector3Int(2, 2, 0);

        Coil coil = new Coil();

        mapManager.AddStructure(coil, position);
        mapManager.RemoveStructure(coil, position);

        Circuit circuit = mapManager.GetUtilityMap().GetCircuitAt(position);

        Assert.IsNull(circuit);
    }
}