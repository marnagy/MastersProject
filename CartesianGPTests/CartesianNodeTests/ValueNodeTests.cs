namespace CartesianGPTests;

public class ValueNodeTests
{
    [Theory]
    [InlineData(-0.7d)]
    [InlineData(0.5d)]
    [InlineData(3d)]
    [InlineData(5d)]
    [InlineData(6d)]
    public void ValueTest(double value)
    {
        var valueNode = new ValueNode(value, 
            Enumerable.Range(0, CartesianNode.ParentsAmount)
            .Select(_ => ParentIndices.GetInvalid())
            .ToArray()
        );
        Assert.Equal(valueNode.Result, value);
    }
    [Fact]
    public void NoParentsTest()
    {
        var valueNode = new ValueNode(1d, 
            Enumerable.Range(0, CartesianNode.ParentsAmount)
            .Select(_ => ParentIndices.GetInvalid())
            .ToArray()
        );

        foreach (var parent in valueNode.Parents)
        {
            Assert.Equal(-1, parent.LayerIndex);
            Assert.Equal(-1, parent.Index);
        }
    }
    [Fact]
    public void ParentsTest1()
    {
        const int layerRange = 20;
        const int nodeRange = 10;
        var rng = new Random(42);
        var parentIndices = Enumerable.Range(0, CartesianNode.ParentsAmount)
            .Select(_ => new ParentIndices{
                LayerIndex = rng.Next(layerRange),
                Index = rng.Next(nodeRange)
            })
            .ToArray();
        var valueNode = new ValueNode(1d, parentIndices);

        for (int i = 0; i < CartesianNode.ParentsAmount; i++)
        {
            Assert.
            Assert.Equal(parentIndices[i].LayerIndex, valueNode.LayerIndex);
            Assert.Equal(parentIndices[i].Index, valueNode.Index);
        }
    }
}