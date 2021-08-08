using System.Linq;
using System.Text.Json;

var workingPath = args.First();
var confFileName = "litedoc.conf.json";
var workplaceService = new WorkplaceService(workingPath);
var jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
var JSONService = new JSONService(workplaceService, jsonSerializerOptions);
var configurationService = new ConfigurationService(confFileName, JSONService);
var sectionService = new SectionService(configurationService, workplaceService);
var sectionsParser = new SectionsParser(configurationService, sectionService);
var sectionsWriter = new SectionsWriter(workingPath, sectionsParser);
var PDFWriter = new PDFWriter(configurationService, workplaceService, sectionsWriter);
var liteDoc = new LiteDoc(workplaceService, sectionsParser, PDFWriter);
await PDFWriter.WriteDocument();