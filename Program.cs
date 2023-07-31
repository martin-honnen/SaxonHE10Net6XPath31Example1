using Saxon.Api;
using System.Xml;

string xmlFilePath = "input2.xml";
XmlReaderSettings settings = new XmlReaderSettings();
settings.Schemas.Add("http://sg.iaea.org/ssac-qs/qsSchema.xsd", "input.xsd");
settings.ValidationType = ValidationType.Schema;
settings.ValidationEventHandler += (sender, e) =>
{
    Console.WriteLine($"{e.Severity} {e.Message}");
};

using (XmlReader reader = XmlReader.Create(xmlFilePath, settings))
{
    XmlDocument document = new XmlDocument();
    document.Load(reader);

    Processor processor = new Processor(false);

    DocumentBuilder docBuilder = processor.NewDocumentBuilder();

    XdmNode wrappedDoc = docBuilder.Wrap(document);

    XPathCompiler xpathCompiler = processor.NewXPathCompiler();

    xpathCompiler.DeclareNamespace("", "http://sg.iaea.org/ssac-qs/qsSchema.xsd");
    xpathCompiler.DeclareNamespace("sg", "http://sg.iaea.org/ssac-qs/qsSchema.xsd");

    foreach (XdmNode car in xpathCompiler.Evaluate("//Car[@condition]", wrappedDoc))
    {
        var conditionEvaluationResult = xpathCompiler.EvaluateSingle(car.GetAttributeValue("condition"), car) as XdmAtomicValue;
        if (!(bool)conditionEvaluationResult.Value)
        {
            Console.WriteLine($"Validation failed for Car {car.GetAttributeValue("id")} with year {car.GetAttributeValue("year")}.");
        }
    }

}