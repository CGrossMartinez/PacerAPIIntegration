/* 
 * Title: Pacer API Integration classes & methods
 * Authors: Carlos Gross-Martinez and Henry Gongora
 * Description: Condcuts basic and advance case searches in Pacer  
 */

//library import to be used in methods
using RestSharp;
using Newtonsoft.Json;
using static PacerAPiIntegration;

public class PacerAPiIntegration
{
    //class declaration of variables for pacer authentication
    public class PacerAuthenticationRequest 
        {
            public string? loginId { get; set; }
            public string? password { get; set; }
            public string? clientCode { get; set; }
            public string? redactFlag { get; set;}
        }

    //class declaration of variables for pacer authentication response
    public class PacerAuthenticationResponse
        {
            public string? loginResult { get; set; }
            public string? nextGenCSO { get; set; }
            public string? errorDescription { get; set;}
        }

    //class declaration of variable for pacer search based on Full case number
    public class PacerCaseSearchRequest
        {
            public string? caseNumberFull { get; set; }
        }

    //class declaration of variables for advance pacer search based on jurisdiction, court id, and full case number
    public class PacerAdvCaseSearchRequest
        {
            
            public string? jurisdictionType { get; set; }
            public List<string>? courtId { get; set; }
            public string? caseNumberFull { get; set; }

        }

    //class declaration of variable for pacer logout request
    public class PacerLogOutRequest
        {
            public string? nextGenCSO { get; set; }
        }

    //class declaration of variables for pacer logout response
    public class PacerLogOutResults
        { 
            public string? loginResult { get; set; }
            public string? errorDescription { get; set; }
        }

    //class declaration of variables for Pacer case search results in JSON format
    public class PacerCaseSearchResults
        { 
            public PacerReceipSearchtResults? receipt { get; set; }
            public PacerPageInfoSearchResults? pageInfo { get; set; }            
            public List<PacerContentSearchResults>? content { get; set; }
            public string? masterCase { get; set; }
        }

    //class declaration of variables for Pacer receipt search results in JSON format
    public class PacerReceipSearchtResults
        { 
            public string? transactionDate { get; set; }
            public int? billablePages { get; set; }
            public string? loginId { get; set;}
            public string? clientCode { get; set; }
            public string? firmId { get; set; }
            public string? search { get; set; }
            public string? description { get; set; }
            public int? csoId { get; set; }
            public string? reportId { get; set; }
            public string? searchFee { get; set; }
        }

    //class declaration of variables for Pacer page information search results in JSON format
    public class PacerPageInfoSearchResults
        { 
            public int? number { get; set; }
            public int? size { get; set; }
            public int? totalPages { get; set; }
            public int? totalElements { get; set; }  
            public int? numberOfElements { get; set; }
            public bool? first { get; set; }
            public bool? last { get; set; }   
        }

    //class declaration of variables for Pacer case content information search results in JSON format
    public class PacerContentSearchResults
        {
            public string? courtId { get; set; }
            public string? caseId { get; set; }
            public string? caseYear { get; set; }
            public string? caseNumber { get; set; }
            public string? caseOffice { get; set; }
            public string? caseType { get; set; }
            public string? caseTitle { get; set; }
            public string? dateFiled { get; set; }
            public string? bankruptcyChapter { get; set; }
            public string? jointBankruptcyFlag { get; set; }
            public string? jurisdictionType { get; set; }
            public string? caseLink { get; set; }
            public string? caseNumberFull { get;set; }
        }

    //Method that authenticates in Pacer and returns NextGenCSO
    public string PacerAuthentication()
        {
            //saving path where text file with login credentials for pacer are located
            string fileName = @"C:\Users\cagrossmartinez\source\repos\PacerAPIIntegration\PacerAPIIntegration\Credentials.txt";

            //reading information from credential text file
            var CredentialContent = File.ReadAllLines(fileName);

            //conditional check to ensure text file with credentials is not empty
            if (CredentialContent.Length == 0 || CredentialContent == null)
                return "1";              

            //assignment of credentials in text file to variables in PacerAuthenticationRequest class
            var contentBody = new PacerAuthenticationRequest
                {
                    loginId = CredentialContent[0],
                    password = CredentialContent[1],
                    clientCode = "testclientcode",
                    redactFlag = "1"
                };

            //instantiating a client variable for connection to specific pacer URL
            var client = new RestSharp.RestClient("https://qa-login.uscourts.gov");

            //instantiating a connection resquest updating the path of URL and method
            var request = new RestSharp.RestRequest("/services/cso-auth", Method.Post);

            //updating header content-type
            request.AddHeader("Content-Type", "application/json");

            //updating header accept
            request.AddHeader("Accept", "application/json");

            //serializing login information into json format
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(contentBody);

            //adding all parameters to connection request
            request.AddParameter("application/json", body, RestSharp.ParameterType.RequestBody);

            //setting the connection security protocol
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

            //excuting connection request and saving response into variable
            var response = client.Execute(request);

            //saving the contents of response into variable
            var responseContent = response.Content;

            //checking if response is null
            if (responseContent == null)
                return "2";
    
            //desiralizing response and saving it as class object in variable
            var oResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PacerAuthenticationResponse>(responseContent);

            //checking that variable is not null
            if (oResponse == null)
                return "3";

            //checking for errors while logging in
            if (oResponse.errorDescription != null)
                return oResponse.errorDescription;

            //returning next gen authentication key
            return (oResponse.nextGenCSO != null && oResponse.loginResult == "0" && oResponse.nextGenCSO.Length == 128) ? oResponse.nextGenCSO : "3";  
        }

    //Method that conducts case searches based on full case number
    public PacerCaseSearchResults PacerCaseSearch(string caseNumber, string nextGenCSO)
        {
            //assignment of case number to variable in PacerCaseSearchRequest class
            var caseNum = new PacerCaseSearchRequest { caseNumberFull = caseNumber };    

            //instantiating a client variable for connection to specific pacer URL
            var client = new RestSharp.RestClient("https://qa-pcl.uscourts.gov");

            //instantiating a connection resquest updating the path of URL and method    
            var request = new RestSharp.RestRequest("/pcl-public-api/rest/cases/find/", Method.Post);

            //updating header content-type
            request.AddHeader("Content-Type", "application/json");

            //updating header accept
            request.AddHeader("Accept", "application/json");

            //updating header X-NEXT-GEN-CSO
            request.AddHeader("X-NEXT-GEN-CSO", nextGenCSO);

            //serializing search critiria into json format
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(caseNum);

            //adding all parameters to connection request
            request.AddParameter("application/json", body, RestSharp.ParameterType.RequestBody);

            //setting the connection security protocol
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

            //excuting connection request and saving response into variable
            var response = client.Execute(request);

            //saving the contents of response into variable
            var responseContent = response.Content;

            //checking Pacer response variable to ensure data was received 
            if (responseContent == null)
                {
                    Console.WriteLine("Pacer API Case Search Result Error");                
                    System.Environment.Exit(0);
                }

            //desiralizing response and saving it as class object in variable
            var oResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PacerCaseSearchResults>(responseContent);

            //checking variable to ensure deserialization was completed properly
            if (oResponse == null) 
                {
                    Console.WriteLine("Pacer Case Search Json Deserialization Error");
                    System.Environment.Exit(0);
                }
            //returning Pacer response
            return oResponse;
        }

    //Method that conducts advance case searches based on full case number, court id, and jurisdiction
    public PacerCaseSearchResults PacerAdvCaseSearch(string nextGenCSO, string courtId, string caseNumber)
        {
            //assignment of advance case information to variables in PacerAdvCaseSearchRequest class
            var searchParameters = new PacerAdvCaseSearchRequest
                {
                    jurisdictionType = "bk",
                    courtId = new List<string> { courtId },
                    caseNumberFull = caseNumber
                };
              
            //instantiating a client variable for connection to specific pacer URL
            var client = new RestSharp.RestClient("https://qa-pcl.uscourts.gov");

            //instantiating a connection resquest updating the path of URL and method
            var request = new RestSharp.RestRequest("/pcl-public-api/rest/cases/find/", Method.Post);

            //updating header content-type
            request.AddHeader("Content-Type", "application/json");

            //updating header accept
            request.AddHeader("Accept", "application/json");

            //updating heaser X-NEXT-GEN-CSO
            request.AddHeader("X-NEXT-GEN-CSO", nextGenCSO);

            //serializing search critiria into json format
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(searchParameters);

            //adding all parameters to connection request
            request.AddParameter("application/json", body, RestSharp.ParameterType.RequestBody);

            //setting the connection security protocol
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

            //excuting connection request and saving response into variable
            var response = client.Execute(request);

            //saving the contents of response into variable
            var responseContent = response.Content;

            //cheking Pacer response variable to ensure data was received
            if (responseContent == null)
                {
                    Console.WriteLine("Pacer API Case Search Result Error");
                    System.Environment.Exit(0);
                }

            //desiralizing response and saving it as class object in variable
            var oResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PacerCaseSearchResults>(responseContent);

            //checking variable to ensure deserialization was completed properly
            if (oResponse == null)
                {
                    Console.WriteLine("Pacer Case Search Json Deserialization Error");
                    System.Environment.Exit(0);
                }

            //returning Pacer response
            return oResponse;
        }

    //Method that conducts logout procedures from Pacer
    public string PacerLogOut(string nextGenCSO)
        {
            //assignment of advance case information to variables in PacerLogOutRequest class
            var nextGenCSOLogOut = new PacerLogOutRequest { nextGenCSO = nextGenCSO };

            //instantiating a client variable for connection to specific pacer URL
            var client = new RestSharp.RestClient("https://qa-login.uscourts.gov");

            //instantiating a connection resquest updating the path of URL and method
            var request = new RestSharp.RestRequest("/services/cso-logout", Method.Post);

            //updating header content-type
            request.AddHeader("Content-Type", "application/json");

            //updating header accept
            request.AddHeader("Accept", "application/json");

            //serializing logout information into json format
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(nextGenCSOLogOut);

            //adding all parameters to connection request
            request.AddParameter("application/json", body, RestSharp.ParameterType.RequestBody);

            //setting the connection security protocol
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

            //excuting connection request and saving response into variable
            var response = client.Execute(request);

            //saving the contents of response into variable
            var responseContent = response.Content;

            //checking response from Pacer to ensure data was received
            if (responseContent == null )
                return "4";

            //desiralizing response and saving it as class object in variable
            var oResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PacerLogOutResults>(responseContent);

            //checking variable to ensure deserialization was completed properly
            if (oResponse == null ) 
                return "5";

            //returning Pacer logout results
            return (oResponse.loginResult == "0" && oResponse.errorDescription == null) ? oResponse.loginResult: "6";
        }

    //checking for possible errors in authentication/logout
    public void PacerErrorCheck(string PacerTest)
        {
            if (PacerTest == "1" || PacerTest == "2" || PacerTest == "3" || PacerTest == "4" || PacerTest == "5" || PacerTest == "6")
                {
                    //statement that prints errors in authentication/logout
                    switch (PacerTest)
                        {
                            case "1":
                                Console.WriteLine("Credential text file is empty");
                                System.Environment.Exit(0);
                                break;

                            case "2":
                                Console.WriteLine("No reponse from pacer");
                                System.Environment.Exit(0);
                                break;

                            case "3":
                                Console.WriteLine("Unable to desirialize authentication response from pacer");
                                System.Environment.Exit(0);
                                break;

                            case "4":
                                Console.WriteLine("No reponse from pacer with logout results");
                                System.Environment.Exit(0);
                                break;

                            case "5":
                                Console.WriteLine("Unable to desirialize logout response from pacer");
                                System.Environment.Exit(0);
                                break;

                            case "6":
                                Console.WriteLine("Unable to logout from pacer");
                                System.Environment.Exit(0);
                                break;
                        }
                }
        }

}
