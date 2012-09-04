namespace SolutionCop.VisualStudio

open System
open System.Xml

[<RequireQualifiedAccess>]
module Configuration = 

    type Data = {
        Name : String;
        Platform : String;
        OutputPath : String;
    }

    let read node = 
        { Name = ""; Platform = ""; OutputPath = ""; }
