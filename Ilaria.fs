// A small command line utility to transform markdown to html
module ilaria

open System.IO
open FSharp.Markdown

// Just going to do single file or single directory for now
type CommandLineOptions = {
  sourceDir: string;
  destinationDir: string
  verbose: bool
  generate_toc: bool
  display_help: bool
  css: string
}

let defaultOptions = {
  sourceDir = "."
  destinationDir = "."
  verbose = false
  generate_toc = false
  display_help = false
  css = ""
}

let helpText = "
usage: Ilaria [--help] [-sourceDir <path>] [-destinationDir <path>] [-toc]
              [-verbose]

-sourceDir , -s        Set the source directory for input Markdown fies

-destinationDir , -d   Set the destination directory for output HTML files

-verbose , -v          Print detailed information to terminal

-toc                   Generate a table of contents based on Markdown headers

-css                   Use the specified CSS file to format HTML output

--help , -?            Display this help text for Ilaria
"

let rec parseCommandArgsRec args optionsSoFar =
  match args with
  | [] -> optionsSoFar

  | "-?"::xs | "--help"::xs ->
    let newOptionsSoFar = {optionsSoFar with display_help = true}
    parseCommandArgsRec xs newOptionsSoFar

  | "-toc"::xs ->
    let newOptionsSoFar = {optionsSoFar with generate_toc = true}
    parseCommandArgsRec xs newOptionsSoFar

  | "-verbose"::xs | "-v"::xs ->
    let newOptionsSoFar = {optionsSoFar with verbose = true}
    parseCommandArgsRec xs newOptionsSoFar

  | "-css"::xs ->
    match xs with
      | s::xss ->
        let newOptionsSoFar = {optionsSoFar with css = s}
        parseCommandArgsRec xss newOptionsSoFar
      | _ ->
        eprintfn "Must specify a file"
        parseCommandArgsRec xs optionsSoFar

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

let css_start = "<link rel=\"stylesheet\" href=\"{0}\">
  <style>
      .markdown-body {
          box-sizing: border-box;
          min-width: 200px;
          max-width: 980px;
          margin: 0 auto;
          padding: 45px;
      }
  </style>
  <article class=\"markdown-body\">"
let css_end = "</article>"

let createCssHead css_file =
  System.String.Format(css_start, css_file)

[<EntryPoint>]
let main argv =
    let argRecord = parseCommandArgs (Seq.toList argv)
    let source = argRecord.sourceDir
    let dest = argRecord.destinationDir
    let verbose = argRecord.verbose
    let filesToConvert = Directory.GetFiles(source, "*.md")

    if argRecord.display_help then
      printfn "%s" helpText

    else

      for f in filesToConvert do
        let newFileName = dest + Path.GetFileNameWithoutExtension f + ".html"
        let markdownText = File.ReadAllText f
        let transformedHtml = Markdown.TransformHtml markdownText

        if verbose then
          printfn "creating file %s from %s" newFileName f

          File.WriteAllText(newFileName, transformedHtml)

    0 // return an integer exit code
