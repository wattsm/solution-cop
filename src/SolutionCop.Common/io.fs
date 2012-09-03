namespace SolutionCop.Common

open System
open System.IO

module IO = 

    let exists = 
        File.Exists

    let extension path = 
        
        let info = FileInfo (path)
        info.Extension

    let folder path = 

        let info = FileInfo (path)
        info.DirectoryName

    let combine root leaf = 

        //This method allows relative and absolute paths to be combined and resolved to an absolute paths
        //e.g. by default c:\root\branch combined with ..\leaf results in c:\root\branch\..\leaf whereas this method
        //yields c:\root\leaf

        let combined = Path.Combine (root, leaf)
        let local = (Uri (combined)).LocalPath

        Path.GetFullPath local

    let fileContents path = 
        
        if not (File.Exists path) then
            raise (FileNotFoundException ())

        use reader = File.OpenText path
        reader.ReadToEnd ()

    let fileName path = 

        let info = FileInfo (path)
        info.Name