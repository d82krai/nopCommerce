using System;
using System.Collections.Generic;
using System.Text;
using Nop.EPP.AlbumPrint.Domain;

namespace Nop.EPP.AlbumPrint.Services
{
    public interface IProductViewTrackerService
    {
        /// <summary>
        /// Logs the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        void Log(ProductViewTrackerRecord record);
    }

}
