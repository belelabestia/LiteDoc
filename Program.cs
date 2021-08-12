Resources.Use<IConfiguration, VerboseConfiguration>();
Resources.Use<IDocument, VerboseDocument>();
Resources.Use<IFileSystem, VerboseFileSystem>();
Resources.Use<IJson, VerboseJson>();
Resources.Use<ISections, VerboseSections>();
Resources.Use<IWatcher, VerboseWatcher>();

// await args[0].ToLiteDoc().Run();
await args[0].ToLiteDoc().StartWatching();