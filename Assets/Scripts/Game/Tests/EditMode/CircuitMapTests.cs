#if UNITY_INCLUDE_TESTS
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

        bool added = mapManager.AddStructure(coil, position);

        Circuit circuit = mapManager.GetUtilityMap().GetCircuitAt(position);

        Assert.IsTrue(added);
        Assert.IsNotNull(circuit);
        Assert.IsTrue(circuit._coils.ContainsKey(position));
        Assert.AreEqual(1, mapManager.GetUtilityMap().CircuitCount);
    }

    [Test]
    public void RemoveSingleCoil_Should_RemoveCircuit()
    {
        Vector3Int position = new Vector3Int(2, 2, 0);

        mapManager.AddStructure(new Coil(), position);

        bool removed = mapManager.GetUtilityMap().RemoveCoil(position);

        Circuit circuit = mapManager.GetUtilityMap().GetCircuitAt(position);

        Assert.IsTrue(removed);
        Assert.IsNull(circuit);
        Assert.AreEqual(0, mapManager.GetUtilityMap().CircuitCount);
    }

    [Test]
    public void AddAdjacentCoils_Should_BeInSameCircuit()
    {
        Vector3Int posA = new Vector3Int(2, 2, 0);
        Vector3Int posB = new Vector3Int(3, 2, 0);

        mapManager.AddStructure(new Coil(), posA);
        mapManager.AddStructure(new Coil(), posB);

        Circuit circuitA = mapManager.GetUtilityMap().GetCircuitAt(posA);
        Circuit circuitB = mapManager.GetUtilityMap().GetCircuitAt(posB);

        Assert.IsNotNull(circuitA);
        Assert.IsNotNull(circuitB);
        Assert.AreSame(circuitA, circuitB);
        Assert.AreEqual(1, mapManager.GetUtilityMap().CircuitCount);
    }

    [Test]
    public void AddingTwoAdjacentCoils_ShouldCreateOneCircuit()
    {
        var pos1 = new Vector3Int(0, 0, 0);
        var pos2 = new Vector3Int(1, 0, 0);

        UtilityMap utilityMap = mapManager.GetUtilityMap();
        utilityMap.AddCoil(pos1);
        utilityMap.AddCoil(pos2);

        var c1 = utilityMap.GetCircuitAt(pos1);
        var c2 = utilityMap.GetCircuitAt(pos2);

        Assert.AreSame(c1, c2);
    }

    [Test]
    public void RemoveMiddleCoil_Should_SplitCircuit()
    {
        Vector3Int left = new Vector3Int(1, 2, 0);
        Vector3Int middle = new Vector3Int(2, 2, 0);
        Vector3Int right = new Vector3Int(3, 2, 0);

        mapManager.AddStructure(new Coil(), left);
        mapManager.AddStructure(new Coil(), middle);
        mapManager.AddStructure(new Coil(), right);

        mapManager.GetUtilityMap().RemoveCoil(middle);

        Circuit leftCircuit = mapManager.GetUtilityMap().GetCircuitAt(left);
        Circuit rightCircuit = mapManager.GetUtilityMap().GetCircuitAt(right);
        Circuit middleCircuit = mapManager.GetUtilityMap().GetCircuitAt(middle);

        Assert.IsNull(middleCircuit);
        Assert.IsNotNull(leftCircuit);
        Assert.IsNotNull(rightCircuit);
        Assert.AreNotSame(leftCircuit, rightCircuit);
        Assert.AreEqual(2, mapManager.GetUtilityMap().CircuitCount);
    }
}
#endif