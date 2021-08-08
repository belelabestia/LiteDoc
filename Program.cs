using System.Linq;

var workingPath = args.First();
var configurationService = new ConfigurationService(workingPath);
var sectionService = new SectionService(workingPath, configurationService);
var sectionsParser = new SectionsParser(configurationService, sectionService);
var sectionsWriter = new SectionsWriter(workingPath, sectionsParser);
var PDFWriter = new PDFWriter(workingPath, sectionsParser, sectionsWriter);
var liteDoc = new LiteDoc(workingPath, sectionsParser, PDFWriter);
await liteDoc.WriteDocument();