
var Pacer = new PacerAPiIntegration();

var PacerAuth = Pacer.PacerAuthentication();

//Console.WriteLine(PacerAuth);

Pacer.PacerErrorCheck(PacerAuth);

//var CaseResults = Pacer.PacerCaseSearch("6:21-bk-00018", PacerAuth);

//var CaseResults = Pacer.PacerAdvCaseSearch(PacerAuth, "azttbk", "6:21-bk-00018");

//var CaseResults = Pacer.PacerAdvCaseSearch(PacerAuth, "tcebk", "6:21-bk-00018");

//var CaseResults = Pacer.PacerCaseSearch("1:2008-bk-00189", PacerAuth);

var CaseResults = Pacer.PacerAdvCaseSearch(PacerAuth, "azttbk", "1:2008-bk-00189");

//var CaseResults = Pacer.PacerAdvCaseSearch(PacerAuth, "tcebk", "1:2008-bk-00189");

var logout = Pacer.PacerLogOut(PacerAuth);

Pacer.PacerErrorCheck(logout);

//Console.WriteLine(logout);


string[] caseLinks = Array.Empty<string>();

if (CaseResults.content == null)
    {
        Console.WriteLine("Pacer Results variable is empty");
        System.Environment.Exit(0);
    }

foreach (var item in CaseResults.content)
    if (item.caseLink != null)
        caseLinks = caseLinks.Append(item.caseLink).ToArray();


var pdfRequest = new PDFRetrival();

//var pdfDownloadSucess = pdfRequest.SavePDF(caseLinkInfo);

var pdfDownloadSucess = pdfRequest.SaveMultiplePDFs(caseLinks[0], 2);


Console.WriteLine(pdfDownloadSucess);















