namespace SolutionCop.Common

open System
open System.Xml
open System.Xml.XPath

module Xml = 

    ///Describes an error with an xpath query
    type XPathQueryException (xpath : string) = 
        inherit Exception (String.Format ("No node(s) could be found for the xpath query {0}.", xpath))

    ///A record linking an XML node and associated namespace manager
    type XmlContext = {
        Node : XmlNode;
        Namespaces : XmlNamespaceManager;
    }

    ///Converts an XML document to an XML context
    let toContext (xml : XmlDocument) = 
        {
            Node = xml.DocumentElement;
            Namespaces = XmlNamespaceManager (xml.NameTable)
        }

    ///Registers a namespace with the context's namespace manager
    let register prefix uri context = 
        if not (context.Namespaces.HasNamespace (prefix)) then
            context.Namespaces.AddNamespace (prefix, uri)

        context

    ///Clears any registered namespace prefixes
    let clearPrefixes context = 
        { context with Namespaces = XmlNamespaceManager (context.Node.OwnerDocument.NameTable) }

    ///Selects a single node
    let selectSingle xpath context = 
        match context.Node.SelectSingleNode (xpath, context.Namespaces) with
        | null -> raise (XPathQueryException (xpath))
        | node -> { context with Node = node; }

    ///Selects multiple notes
    let selectMany xpath context = 
        context.Node.SelectNodes (xpath, context.Namespaces)
        |> List.ofEnumerable
        |> List.map (fun node -> { Node = node; Namespaces = context.Namespaces; })

    ///Get the value of a named attribute
    let getAttribute (name : string) context = 
        match context.Node.Attributes.[name] with 
        | null -> String.Empty
        | attr -> attr.Value
            
    ///Gets the XML node of the current context
    let toNode context = 
        context.Node

    ///Gets the value of the current node
    let toValue context = 
        context.Node.InnerText

    ///Gets the value of the node identified by the xpath query
    let selectSingleValue xpath = 
        (selectSingle xpath) >> toValue