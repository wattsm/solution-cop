namespace SolutionCop.Common

open System
open System.IO

module IO = 

    ///Reads the contents of a text file
    let contentsOf path = 
        use reader = File.OpenText (path)
        reader.ReadToEnd ()

    ///Reads all of the given stream as s string
    let readAll (stream : Stream) = 
        use reader = new StreamReader (stream)
        reader.ReadToEnd ()

    ///Gets the full directory path of a given file
    let directoryOf path =         
        let info = FileInfo (path)
        info.DirectoryName

    ///Gets the name of the file at the given path
    let nameOf path = 
        let info = FileInfo (path)
        info.Name

    ///Combines two paths, first being the root
    let combine root leaf = 

        //This method resolves relative path fragments like "..\", e.g. combining c:\Folder\Subfolder-1 with ..\Subfolder-2 will 
        //result in c:\Folder\Subfolder-2.

        let path = Path.Combine (root, leaf)
        let uri = Uri (path)

        uri.LocalPath
        |> Path.GetFullPath

    ///Writes the given string to disk at the path specified. Any existing file will be deleted.
    let toDisk path (contents : string) = 
        
        if (File.Exists path) then
            File.Delete path

        use writer = File.CreateText path
        writer.Write contents