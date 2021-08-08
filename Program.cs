using System.Linq;

var workingPath = args.First();
var liteDoc = new LiteDoc(workingPath);
await liteDoc.WriteDocument();