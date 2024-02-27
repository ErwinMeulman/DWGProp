open System
open System.IO
open System.Text
 
// dwg encoding type
type DwgEncoding =
| AsUnicode
| AsAscii
 
// dwg property
type DwgProperty = {key : string ; value : string ; custom : bool}
 
// read a string from the dwg
let ReadString (binReader : BinaryReader, dwgEncoding : DwgEncoding) =
 
    let nBytes = if dwgEncoding = DwgEncoding.AsUnicode then 2 else 1   
 
    let mutable stringLen = int(binReader.ReadUInt16())
    let stringData = binReader.ReadBytes(stringLen * nBytes);
 
    // if null terminated...
    if (stringData.[(stringLen * nBytes) - (1 * nBytes)] = 0uy) then
        stringLen <- stringLen - 1
 
    let encoding = if dwgEncoding = DwgEncoding.AsUnicode then Encoding.Unicode else Encoding.UTF8   
    encoding.GetString(stringData, 0, stringLen * nBytes);
 
// Read the properties from the header of the dwg file
//
let ReadHeaderProps (binReader : BinaryReader, header : byte[], dwgEncoding : DwgEncoding) =
 
    // move to the properties section
    let offsetToSection = BitConverter.ToInt32(header, 0x20);
 
    // sometimes the offset is empty
    if offsetToSection > 0 then   
        let mutable toSkip = offsetToSection - header.Length;   
        while toSkip > 0 do       
            let ms = Math.Min(toSkip, 0x4000)
            let skip = binReader.ReadBytes(ms)
            toSkip <- toSkip - skip.Length;   
 
        // read standard properties   
        let stdProps = [for k in [|"TITLE"; "SUBJECT"; "AUTHOR"; "KEYWORDS"; "COMMENTS"; "LAST_AUTHOR"; "REVISION"; "RELATION"|]
                        -> {key = k ; value = ReadString(binReader, dwgEncoding) ; custom = false}]
 
        // skip unknown data
        let _ = binReader.ReadBytes(0x18);       
 
        // get the number of entries (2 bytes)
        let numEntries = int(binReader.ReadUInt16())
        // read the custom properties
        let cusProps = [for i in 0 .. numEntries-1 -> {key = ReadString(binReader, dwgEncoding) ; value = ReadString(binReader, dwgEncoding) ; custom = true}]
        // join the two lists
        stdProps @ cusProps
    else
        []      
   
 
let ReadDwgProps fn =
 
    let (|BeginsWith|) arg (s : string) =
        s.StartsWith arg       
 
    use binReader = new BinaryReader(File.OpenRead(fn)) 
 
    let header = binReader.ReadBytes(0x80);
    let encoding = System.Text.Encoding.ASCII;   
 
    // search in reverse order (most common at the beginning)
    let dwgVersion = encoding.GetString(header, 0, 6)
    let dwgProperties = match dwgVersion with
                          // unicode versions
                          | BeginsWith "AC1024" true
                          | BeginsWith "AC1021" true -> ReadHeaderProps(binReader, header, DwgEncoding.AsUnicode)
                          // ascii versions
                          | BeginsWith "AC1018" true -> ReadHeaderProps(binReader, header, DwgEncoding.AsAscii)
                          | BeginsWith "AC1015" true
                          | BeginsWith "AC1014" true
                          | BeginsWith "AC1012" true
                          | BeginsWith "AC1009" true
                          | BeginsWith "AC1006" true
                          | BeginsWith "AC1004" true
                          | BeginsWith "AC1003" true
                          | BeginsWith "AC1002" true
                          | BeginsWith "AC1001" true
                          | BeginsWith "AC2.22" true
                          | BeginsWith "AC2.21" true
                          | BeginsWith "AC2.10" true
                          | BeginsWith "AC1.50" true
                          | BeginsWith "AC1.40" true
                          | BeginsWith "AC1.2"  true
                          | BeginsWith "MC0.0"  true -> []
                          // try for releases to come
                          | _                        -> ReadHeaderProps(binReader, header, DwgEncoding.AsUnicode)
    // add the file version to the properties                   
    dwgProperties @ [{key = "DWGVERSION" ; value = dwgVersion ; custom = false}]
  
// for debugging only
let DumpDwgProperties fn =
    printf "Properties of file: %s\n" fn
    ReadDwgProps fn |> List.iter (fun (p :DwgProperty) -> printf "%s = %s\n" p.key p.value)
    printf "\n"
  
// put the folder to parse here
Directory.GetFiles(@"Folder\To\Scan\Here", "*.dwg")
|> Array.map DumpDwgProperties
|> ignore