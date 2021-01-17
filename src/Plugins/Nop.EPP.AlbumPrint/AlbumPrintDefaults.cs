using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.EPP.AlbumPrint
{
    public class AlbumPrintDefaults
    {
        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string AlbumPrintPageRouteName => "Plugin.EPP.AlbumPrint.UploadPhotos";
        public static string DownloadPhotoRouteName => "Plugin.EPP.AlbumPrint.DownloadPhotos";
    }
}
