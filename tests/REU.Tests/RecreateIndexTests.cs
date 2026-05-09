using Writer.Indexers;
namespace Tests.Writer.Indexers;

public class RecreateIndexTests
{
    [Test]
    public void RecreateIndex_ShouldRecreateIndexFile()
    {
        var outputRoot = "../../../../../output";
        var indexer = new Indexer(outputRoot, null);

        //dotnet test --filter "FullyQualifiedName~Tests.Writer.Indexers.RecreateIndexTests.RecreateIndex_ShouldRecreateIndexFile"
    }
}