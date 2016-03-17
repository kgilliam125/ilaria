// A small command line utility to transform markdown to html
module ilaria

open System.IO
open FSharp.Markdown

// Just going to do single file or single directory for now
type CommandLineOptions = {
  sourceDir: string;
  destinationDir: string
  verbose: bool
}

let defaultOptions = {
  sourceDir = "."
  destinationDir = "."
  verbose = false
}

let rec parseCommandArgsRec args optionsSoFar =
  match args with
  | [] -> optionsSoFar

  | "-verbose"::xs | "-v"::xs ->
    let newOptionsSoFar = {optionsSoFar with verbose = true}
    parseCommandArgsRec xs newOptionsSoFar

  | "-sourceDir"::xs | "-s"::xs ->
    match xs with
    | s::xss ->
      let newOptionsSoFar = {optionsSoFar with sourceDir = s}
      parseCommandArgsRec xss newOptionsSoFar
    | _ ->
      eprintfn "Must have a directory"
      parseCommandArgsRec xs optionsSoFar

  | "-destinationDir"::xs | "-d"::xs ->
      match xs with
      | s::xss ->
        let newOptionsSoFar = {optionsSoFar with destinationDir = s}
        parseCommandArgsRec xss newOptionsSoFar
      | _ ->
        eprintfn "Must have a directory"
        parseCommandArgsRec xs optionsSoFar

    | x::xs ->
        eprintfn "Unrecognized options"
        parseCommandArgsRec xs optionsSoFar

let parseCommandArgs args =
  parseCommandArgsRec args defaultOptions

[<EntryPoint>]
let main argv =
    let argRecord = parseCommandArgs (Seq.toList argv)
    let source = argRecord.sourceDir
    let dest = argRecord.destinationDir
    let verbose = argRecord.verbose
    let filesToConvert = Directory.GetFiles(source, "*.md")

    for f in filesToConvert do
      let newFileName = dest + Path.GetFileNameWithoutExtension f + ".html"
      let markdownText = File.ReadAllText f
      let transformedHtml = Markdown.TransformHtml markdownText

      if verbose then
        printfn "creating file %s from %s" newFileName f

      File.WriteAllText(newFileName, transformedHtml)

    0 // return an integer exit code
