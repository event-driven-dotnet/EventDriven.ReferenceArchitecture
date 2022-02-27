namespace EventDriven.ReferenceArchitecture.Specs.Repositories;

public class JsonFilesRepository
{
    private const string Root = "../../../json/";
    public Dictionary<string, string> Files { get; } = new();

    public JsonFilesRepository(params string[] files)
    {
        var filesList = files.ToList();
        if (!filesList.Any())
            foreach (var file in Directory.GetFiles(Root))
                filesList.Add(Path.GetFileName(file));

        foreach (var file in filesList)
        {
            var path = Path.Combine(Root, file);
            var contents = File.ReadAllText(path);
            Files.Add(file, contents);
        }
    }
}