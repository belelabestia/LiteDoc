Configurations.Instance = new VerboseConfiguration();
Document.Instance = new VerboseDocument();
FileSystem.Instance = new VerboseFileSystem();
Json.Instance = new VerboseJson();
Sections.Instance = new VerboseSections();
Watcher.Instance = new VerboseWatcher();

await args[0].ToLiteDoc().Run();
// await args[0].ToLiteDoc().StartWatching();