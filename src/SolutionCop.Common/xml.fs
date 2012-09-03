namespace SolutionCop.Common

open System
open System.Xml

module Xml = 

    let owner (xml : XmlNode) = 
        xml.OwnerDocument

    let node xpath (xml : XmlNode) = 
        xml.SelectSingleNode (xpath)

    let addAttr name value (xml : XmlNode) = 
        
        let attr = xml.OwnerDocument.CreateAttribute (name);
        attr.Value <- value

        xml.Attributes.Append attr
        |> ignore

        xml

    let createNode name (attrs : (string * string) list) (xml : XmlDocument) = 

        let node = 
            xml.CreateElement (name) :> XmlNode

        attrs
        |> List.fold (fun node (name, value) -> addAttr name value node) node    

    let innerText content (xml : XmlNode) = 
        xml.InnerText <- content
        xml

    let addContentNode name attrs content (xml : XmlNode) = 
        xml
        |> owner
        |> createNode name attrs 
        |> innerText content
        |> xml.AppendChild
        |> ignore

        xml        

    let addNode name attrs (xml : XmlNode) = 
        addContentNode name attrs null xml

    let asString (node : XmlNode) = 
        node.OuterXml


    
        
