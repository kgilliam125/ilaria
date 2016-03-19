// A small command line utility to transform markdown to html
module Ilaria

open System.IO
open FSharp.Markdown

// Just going to do single file or single directory for now
type CommandLineOptions = {
  SourceDir: string;
  DestinationDir: string
  Verbose: bool
  GenerateToc: bool
  DisplayHelp: bool
  CssFile: string
}

let defaultOptions = {
  SourceDir = "."
  DestinationDir = "."
  Verbose = false
  GenerateToc = false
  DisplayHelp = false
  CssFile = "ilaria.css"
}

let helpText = "
usage: Ilaria [--help] [-sourceDir <path>] [-destinationDir <path>] [-toc]
              [-verbose] [-css <file>]

-sourceDir , -s        Set the source directory for input Markdown fies

-destinationDir , -d   Set the destination directory for output HTML files

-verbose , -v          Print detailed information to terminal

-toc                   Generate a table of contents based on Markdown headers

-css                   Use the specified CSS file to format HTML output. The
                       file will be copied to the directory specified by
                       -destinationDir

--help , -?            Display this help text for Ilaria
"

let rec parseCommandArgsRec args optionsSoFar =
  match args with
  | [] -> optionsSoFar

  | "-?"::xs | "--help"::xs ->
    let newOptionsSoFar = {optionsSoFar with DisplayHelp = true}
    parseCommandArgsRec xs newOptionsSoFar

  | "-toc"::xs ->
    let newOptionsSoFar = {optionsSoFar with GenerateToc = true}
    parseCommandArgsRec xs newOptionsSoFar

  | "-verbose"::xs | "-v"::xs ->
    let newOptionsSoFar = {optionsSoFar with Verbose = true}
    parseCommandArgsRec xs newOptionsSoFar

  | "-css"::xs ->
    match xs with
      | s::xss ->
        let newOptionsSoFar = {optionsSoFar with CssFile = s}
        parseCommandArgsRec xss newOptionsSoFar
      | _ ->
        eprintfn "Must specify a file"
        parseCommandArgsRec xs optionsSoFar

  | "-sourceDir"::xs | "-s"::xs ->
    match xs with
    | s::xss ->
      let newOptionsSoFar = {optionsSoFar with SourceDir = s}
      parseCommandArgsRec xss newOptionsSoFar
    | _ ->
      eprintfn "Must have a directory"
      parseCommandArgsRec xs optionsSoFar

  | "-destinationDir"::xs | "-d"::xs ->
      match xs with
      | s::xss ->
        let newOptionsSoFar = {optionsSoFar with DestinationDir = s}
        parseCommandArgsRec xss newOptionsSoFar
      | _ ->
        eprintfn "Must have a directory"
        parseCommandArgsRec xs optionsSoFar

    | x::xs ->
        eprintfn "Unrecognized options"
        parseCommandArgsRec xs optionsSoFar

let parseCommandArgs args =
  parseCommandArgsRec args defaultOptions

let css_start = "
  <link rel=\"stylesheet\" href=\"{0}\">
    <style>
        .markdown-body {{
            box-sizing: border-box;
            min-width: 200px;
            max-width: 980px;
            margin: 0 auto;
            padding: 45px;
        }}
      </style>
  <article class=\"markdown-body\">"

let css_end = "</article>"

let createCssWrapper cssFile =
    if File.Exists(cssFile) then
      Some ( System.String.Format(css_start, cssFile) , css_end)
    else
      None


[<EntryPoint>]
let main argv =
    let argRecord = parseCommandArgs (Seq.toList argv)
    let source = argRecord.SourceDir
    let dest = argRecord.DestinationDir
    let verbose = argRecord.Verbose
    let cssFile = argRecord.CssFile
    let showHelp = argRecord.DisplayHelp
    let filesToConvert = Directory.GetFiles(source, "*.md")
    let cssWrapper = createCssWrapper cssFile

    if showHelp then
      printfn "%s" helpText

    else
      match cssWrapper with
        | Some (a, b) ->
          if verbose then
            printfn "%s used for css" (Path.GetFileName cssFile)
          File.Copy(cssFile, dest + Path.GetFileName cssFile, true)
        | None ->
          if verbose then
            eprintfn "No CSS will be used either because none was specified or
the specified file did not exist."
          else ()

      for f in filesToConvert do
        let newFileName = dest + Path.GetFileNameWithoutExtension f + ".html"
        let markdownText = File.ReadAllText f
        let transformedHtml = Markdown.TransformHtml markdownText
        let createFormattedHtml css =
          match css with
            | None ->
              transformedHtml
            | Some (a, b) ->
              a + transformedHtml + b

        if verbose then
          printfn "creating file %s from %s" newFileName f

        File.WriteAllText(newFileName, createFormattedHtml cssWrapper)

    0 // return an integer exit code
