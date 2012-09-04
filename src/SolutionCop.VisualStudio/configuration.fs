namespace SolutionCop.VisualStudio

open System

[<RequireQualifiedAccess>]
module Configuration = 

    type Data = {
        Name : String;
        Platform : String;
        OutputPath : String;
    }

