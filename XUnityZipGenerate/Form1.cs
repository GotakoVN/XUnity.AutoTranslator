using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XUnityZipGenerate
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

      private void button1_Click( object sender, EventArgs e )
      {
          DialogResult result = folderBrowserDialog1.ShowDialog( this );
         if ( result == DialogResult.OK )
         {
            textFolderZip.Text = folderBrowserDialog1.SelectedPath;
         }
      }

      private void button2_Click( object sender, EventArgs e )
      {
         if (string.IsNullOrEmpty(textGameName.Text) || textGameName.Text.Trim().Length == 0)
         {
            MessageBox.Show( "Phải nhập tên game. Xin xem log của XUnity để biết tên game" );
            return;
         }

         if( string.IsNullOrEmpty( textFolderZip.Text ) || textFolderZip.Text.Trim().Length == 0 )
         {
            MessageBox.Show( "Phải đưa tên thư mục chứa file cần zip" );
            return;
         }

         if (!Directory.Exists(textFolderZip.Text.Trim()) )
         {
            MessageBox.Show( "Thư mục chứa file cần nén không tồn tại." );
            return;
         }
         else if ( Directory.GetFiles( textFolderZip.Text.Trim() ).Length == 0 )
         {
            MessageBox.Show( "Không có file cần zip !!!" );
            return;
         }
         string workDir = textFolderZip.Text.Trim();
         string[] fileList = Directory.GetFiles(workDir);
         string parentDir = Path.GetDirectoryName( workDir);
         string zipName = workDir.Replace(parentDir, "").Substring(1);
         string zipPassword = GenerateZipPassword(textGameName.Text.Trim());
         textGeneratedPassword.Text = zipPassword;
         try
         {
            using( ZipFile zipFile = new ZipFile() )
            {
               zipFile.b = zipPassword;
               zipFile.Y = EncryptionAlgorithm.WinZipAes256;
               foreach( string file in fileList )
               {
                  zipFile.AddFile( file, "" );
               }
               zipFile.Save( Path.Combine( workDir, zipName + ".zip" ) );
            }
         }
         catch(Exception ex)
         {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
         }
      }

      private string GenerateZipPassword(string gameName)
      {
         string postfix = "Fkue98%A";
         string basePW = "g4Sd6*he(ES3uhf3sDh";
         string inject = "yT8XEnpxfHgZMdSV";
         int dow = (int)DateTime.Now.DayOfWeek;
         if( dow > 3 ) dow = dow + 1;
         string gNameBase64 = System.Convert.ToBase64String( System.Text.Encoding.UTF8.GetBytes( gameName ) );
         return basePW.Insert( 3 + dow, gNameBase64 + "##" + inject ) + postfix;
      }
   }
}
