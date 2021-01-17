using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nop.EPP.AlbumPrint.Domain;

namespace Nop.EPP.AlbumPrint.Services
{
    public interface IProductViewTrackerService
    {
        /// <summary>
        /// Logs the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        Task Log(ProductViewTrackerRecord record);
    }

}
