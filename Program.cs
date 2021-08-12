// Resources.Use<IConfiguration, VerboseConfiguration>();
// Resources.Use<IDocument, VerboseDocument>();
// Resources.Use<IFileSystem, VerboseFileSystem>();
// Resources.Use<IJson, VerboseJson>();
// Resources.Use<ISections, VerboseSections>();
// Resources.Use<IWatcher, VerboseWatcher>();

Resources.Use<IConfiguration, Configuration.Base>();
Resources.Use<IDocument, Document.Base>();
Resources.Use<IFileSystem, FileSystem.Base>();
Resources.Use<IJson, Json.Base>();
Resources.Use<ISections, Sections.Base>();
Resources.Use<IWatcher, Watcher.Base>();

// await args[0].ToLiteDoc().Run();
await args[0].ToLiteDoc().StartWatching();