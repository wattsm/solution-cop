namespace SolutionCop.Common

open System
open System.Xml
open System.Xml.XPath
open SolutionCop.Common.IO

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

    ///Parses an XML context from the given string
    let parse str = 
        
        let xml = XmlDocument ()
        xml.LoadXml (str)

        xml
        |> toContext

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

    ///Removes child nodes of the node(s) identified by the given xpath query based on name
    let removeChildren xpath name context = 

        let remove context' = 
            match (selectMany name context') with
            | [] -> () 
            | cs ->
                cs
                |> List.iter (fun c -> 
                        context'.Node.RemoveChild c.Node
                        |> ignore                         
                    )

        selectMany xpath context 
        |> List.iter remove

        context

    ///Removes a named attribute from the node(s) identified by the given xpath query
    let removeAttribute xpath name context = 
        selectMany xpath context
        |> List.iter (fun context' -> 
                context'.Node.Attributes.RemoveNamedItem name
                |> ignore
            )

        context

    ///Appends an attribute with the given name and value to the node(s) matching the given xpath query
    let appendAttribute xpath name value context = 
        
        let append context' = 
            
            let attr = context'.Node.OwnerDocument.CreateAttribute name
            attr.Value <- value

            context'.Node.Attributes.Append attr
            |> ignore

        selectMany xpath context
        |> List.iter append

        context

    ///Appends a child element with the given name and attributes to the node(s) matching the given xpath query
    let appendChild xpath name attrs context = 
        
        let append context' = 
            
            let child = context'.Node.OwnerDocument.CreateElement name

            attrs
            |> List.iter (fun (name, value) ->

                    let attr = context'.Node.OwnerDocument.CreateAttribute name
                    attr.Value <- value

                    child.Attributes.Append attr
                    |> ignore
                )

            context'.Node.AppendChild child
            |> ignore

        selectMany xpath context
        |> List.iter append

        context

    ///Appends a content node to the node(s) matching the given xpath query
    let appendContent xpath name value context = 
        
        let append context' = 

            let child = context'.Node.OwnerDocument.CreateElement name
            child.InnerText <- value

            context'.Node.AppendChild child
            |> ignore

        selectMany xpath context
        |> List.iter append

        context

    ///Saves the context to disk
    let save path context = 
        context.Node.OuterXml
        |> toDisk path
    
            