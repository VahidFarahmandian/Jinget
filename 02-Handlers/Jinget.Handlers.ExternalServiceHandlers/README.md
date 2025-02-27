# Jinget

**We are currently in the way to make Jinget an open source project, during this journey we will publish different parts of Jinget**

## Jinget.Handlers.ExternalServiceHandlers
The purpose of this package is to facilitate communication and use of various types of web services and Web APIs including Rest APIs and SOAP web services.

### How to Use:

1.  Download the package from NuGet using Package Manager:
`Install-Package Jinget.Handlers.ExternalServiceHandlers`
You can also use other methods supported by NuGet. Check [Here](https://www.nuget.org/packages/Jinget.Handlers.ExternalServiceHandlers "Here") for more information.
 Then register th DI configuration like this:

```csharp
builder.Services.AddJingetExternalServiceHandler("jinget-client", true);
```

You can replace `jinget-client` with your desired client name. if not specified then by default `jinget-client` will be used as client name.

2. Create a class which defines the response model. 
```csharp
     public class SampleGetResponse
     {
         public int id { get; set; }
         public string name { get; set; }
         public string username { get; set; }
    }
```

3. Create an object of type `JingetServiceHandler<>` class and pass the response model as its generic type, or Create an object of type `JingetServiceHandler` class:
```csharp
var jingetServiceHandler = new JingetServiceHandler<SampleGetResponse>(serviceProvider, "https://jsonplaceholder.typicode.com");
```

4. Call your endpoint:
```csharp
var result = await jingetServiceHandler.GetAsync("users");
```
------------
### How to call SOAP web Services:

1. For SOAP services, it would be important to create the request envelope before sending the request. To create the request envelop, You need to create classes for different parts of the request envelop. For example suppose that we need to model the following request envelop:
```xml
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tem="http://tempuri.org/">
   <soapenv:Header/>
   <soapenv:Body>
      <tem:Add>
         <tem:intA>?</tem:intA>
         <tem:intB>?</tem:intB>
      </tem:Add>
   </soapenv:Body>
</soapenv:Envelope>
```
In this request envelop we have `Envelop` node which contains two inner nodes: `Header` and `Body`. Also for `Body`, this node contains a node node called `Add` and finally `Add` contains two nodes called `intA` and `intB`. Also each node belongs to some namespaces which are defined in `Envelop` node.
With all these in mind, we need to define classes for exact this hierarchy as follow:
```csharp
//This is the whole request envelop
public class SampleSOAPRequest : SOAPRequestBase<SampleSOAPRequest.Envelope, SampleSOAPRequest.SampleSOAPGet>
{
    //This method creates an envelop with the following structure
    public override (Envelope envelope, SampleSOAPGet request) CreateEnvelope()
    {
        var envelope = new Envelope
        {
            Header = new EnvelopeHeader(),
            Body = new EnvelopeBody()
        };
        return (envelope, envelope.Body.Add);
    }

    //request envelop contains the Envelop node with some namespaces
    [Serializable, XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/"), XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Envelope : SOAPEnvelopeBase
    {
	    //When calling this method, the final envelop is returned as string value
        public override string ToString()
        {
            XmlSerializerNamespaces ns = new();
            ns.Add("tem", "http://tempuri.org/");
            ns.Add("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");

            return XmlUtility.SerializeToXml(this, true, ns);
        }

        //Envelop node contains the Header node
        public EnvelopeHeader Header { get; set; }

        //Envelop node contains the Body node
        public EnvelopeBody Body { get; set; }
    }

    //The Header node has the following members. As you see it is empty. Also this node belongs to the http://schemas.xmlsoap.org/soap/envelope/ namespace
    [Serializable, XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class EnvelopeHeader
    {

    }

      //The Body node has the following members. Also this node belongs to the http://schemas.xmlsoap.org/soap/envelope/ namespace
    [Serializable, XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class EnvelopeBody
    {
	    //Body node contains a node called 'Add' which belongs to the http://schemas.xmlsoap.org/soap/envelope/ namespace
        [XmlElement(Namespace = "http://tempuri.org/")]
        public SampleSOAPGet Add { get; set; }
    }

     //The Add node has the following members.
    [Serializable]
    public class SampleSOAPGet
    {
        public int intA { get; set; }
        public int intB { get; set; }
    }
}
```

2. Now that we have our envelop, we can easily call the SOAP web service as following:
```csharp
var (envelope, request) = new SampleSOAPRequest().CreateEnvelope();
envelope.Body.Add = new SampleSOAPRequest.SampleSOAPGet { intA = 1, intB = 2 };

 var jingetServiceHandler = new JingetServiceHandler<ResponseType>("http://www.dneonline.com/calculator.asmx");

 var result = await jingetServiceHandler.PostAsync("", envelope.ToString(), true, new Dictionary<string, string>
 {
              {"Content-Type","text/xml" },
              {"SOAPAction","http://tempuri.org/Add" }
 });
```
In line number 2, we have our envelop and all we need to do, is to pass our parameters. In line number 6, the envelop is being send to the PostAsync method as string value. It is important to note the `SOAPAction` header.

------------
### How to use custom Service Handler

You can use your custom service handler instead of using `JingetServiceHandler`. To do this create your custom class and makes it to inherit from `ServiceHandler<>` or `ServiceHandler` class. 
Also create a custom class for your event management and pass it as generic argument to `ServiceHandler<>` class. 
You can also make use of `ServiceHandler`(non generic class) too. Different between these two class is that `ServiceHandler` does not provide any event for response deserialization.
it returns the raw response and deserialization is up to you.
For example suppose that we have a class called `CustomHandler` as below:
```csharp
    public class CustomServiceHandler : ServiceHandler<CustomEvents>
    {
        ...
    }
```

or

```csharp
    public class CustomServiceHandler(string baseUri, bool ignoreSslErrors = false) : JingetServiceHandler(baseUri, ignoreSslErrors)
    {
        ...
    }
```
------------
# How to install
In order to install Jinget please refer to [nuget.org](https://www.nuget.org/packages/Jinget.Handlers.ExternalServiceHandlers "nuget.org")

# Further Information
Sample codes are available via Unit Test projects which are provided beside the main source codes.

# Contact Me
👨‍💻 Twitter: https://twitter.com/_jinget

📧 Email: farahmandian2011@gmail.com

📣 Instagram: https://www.instagram.com/vahidfarahmandian
