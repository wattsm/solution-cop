namespace SolutionCop.FxCop

open System

[<RequireQualifiedAccess>]
module Targets =

    ///Records describing basic target data for an FxCop project
    type Data = {
        Binaries : String list;
        ReferencePaths : String list;
    }

    ///Extracts target binaries from the given solution
    let private binaries solution filters = 
        []

    ///Extracts target reference paths from the given solution
    let private references solution filters = 
        []

    ///Extracts target binaries and reference paths from the given solution
    let extract filters solution = 
        { 
            Binaries = (binaries solution filters); 
            ReferencePaths = (references solution filters); 
        }