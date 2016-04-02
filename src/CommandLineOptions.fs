module CommandLineOptions


// Only doing all files in a directory for now. Really should add single file.
type CommandLineOptions = {
  CssFile: string
  DestinationDir: string
  DisplayHelp: bool
  GenerateToc: bool
  SourceDir: string
  ResourceDir: string
  Verbose: bool
}

let defaultOptions = {
  CssFile = "ilaria.css"
  DestinationDir = "default"
  DisplayHelp = false
  GenerateToc = false
  SourceDir = "default"
  ResourceDir = ""
  Verbose = false
}

let helpText = "
usage: Ilaria [--help] [-sourceDir <path>] [-destinationDir <path>] [-toc]
              [-verbose] [-css <file>] [-resourceDir <path>]

-sourceDir , -s        Set the source directory for input Markdown fies

-destinationDir , -d   Set the destination directory for output HTML files

-verbose , -v          Print detailed information to terminal

-toc                   Generate a table of contents based on Markdown headers

-css                   Use the specified CSS file to format HTML output. The
                       file will be copied to the directory specified by
                       -destinationDir

--help , -?            Display this help text for Ilaria
"
