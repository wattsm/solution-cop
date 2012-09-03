namespace SolutionCop.VisualStudio.Tests

open System
open System.Reflection
open System.IO

[<AutoOpen>]
module Utils = 

    let getFileContents (name : string) = 
        
        let assembly = Assembly.GetExecutingAssembly ()

        use stream = assembly.GetManifestResourceStream name
        use reader = new StreamReader (stream)

        reader.ReadToEnd ()
