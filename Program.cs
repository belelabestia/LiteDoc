using System.Linq;
using System.Text.Json;

var workingPath = args.First();
var confFileName = "litedoc.conf.json";
var workplaceService = new WorkplaceService(workingPath);
var jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
var JSONService = new JSONService(workplaceService, jsonSerializerOptions);
var configurationService = new ConfigurationService(confFileName, JSONService);
var sectionService = new SectionService(workingPath, configurationService);
var sectionsParser = new SectionsParser(configurationService, sectionService);
var sectionsWriter = new SectionsWriter(workingPath, sectionsParser);
var PDFWriter = new PDFWriter(workingPath, sectionsParser, sectionsWriter);
var liteDoc = new LiteDoc(workingPath, sectionsParser, PDFWriter);
await liteDoc.WriteDocument();