namespace SolutionCop.Common

open System
open System.Xml
open System.Xml.XPath

module XPath = 

    type XmlContext = {
        Navigator : XPathNavigator;
        Namespaces : XmlNamespaceManager;
    }
    
    let parse str = 

        let xml = 
            XmlDocument ()

        xml.LoadXml (str)

        xml


    let navigate (xml : XmlDocument) =     
        {
            Navigator = (xml.CreateNavigator ());
            Namespaces = (XmlNamespaceManager (xml.NameTable));
        }

    let prefix symbol ns (context : XmlContext) = 
        context.Namespaces.AddNamespace (symbol, ns)
        context

    let node xpath (context : XmlContext) = 
        { context with Navigator = (context.Navigator.SelectSingleNode (xpath, context.Namespaces)); }

    let maybeNode xpath (context : XmlContext) =         
        match (context.Navigator.SelectSingleNode (xpath, context.Namespaces)) with
        | null -> None
        | n -> Some { context with Navigator = n; }

    let nodes xpath (context : XmlContext) = 
        context.Navigator.Select (xpath, context.Namespaces)
        |> List.ofEnumerable<XPathNavigator>          
        |> List.map (fun navigator -> { context with Navigator = navigator; })

    let attr name (context : XmlContext) = 
        context.Navigator.GetAttribute (name, String.Empty)

    let valueOf xpath (context : XmlContext) = 
        match (maybeNode xpath context) with
        | Some n -> Some n.Navigator.InnerXml
        | _ -> None