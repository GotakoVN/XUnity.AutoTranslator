using System.IO;
using System.IO.Compression;
using System.Text;


namespace XZipper
{
   class Program
   {
      static void Main( string[] args )
      {
         if( File.Exists( args[ 1 ] ) )
         {
            File.Delete( args[ 1 ] );
         }
         using( var zip = ZipFile.Open( args[1], ZipArchiveMode.Create) )
         {
            foreach(string file in Directory.GetFiles( args[0] ))
            {
               zip.CreateEntryFromFile( file, Path.GetFileName( file ), CompressionLevel.Optimal );
            }
            // zip.SaveTo( args[ 1 ], new ZipWriterOptions( CompressionType.Deflate ) { ArchiveEncoding = new ArchiveEncoding( Encoding.UTF8, Encoding.UTF8 ), CompressionType = CompressionType.Deflate, DeflateCompressionLevel = CompressionLevel.BestCompression, UseZip64 = false } );
         }
      }
   }
}
