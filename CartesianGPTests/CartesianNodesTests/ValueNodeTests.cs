namespace CartesianGPTests;
using CartesianGP.Nodes.ValueNode;

public class ValueNodeTests
{
    [Fact]
    public void NoParentsTest()
    {
        var valueNode = new ValueNode();
    }
}