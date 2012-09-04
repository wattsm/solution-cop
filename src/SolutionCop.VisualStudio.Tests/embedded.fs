namespace SolutionCop.VisualStudio.Tests

open System
open System.Reflection
open System.Xml

module Embedded =

    let xml name = 

        let assembly = Assembly.GetExecutingAssembly ()

        use stream = assembly.GetManifestResourceStream name

        let xml = XmlDocument ()
        xml.Load stream

        xml