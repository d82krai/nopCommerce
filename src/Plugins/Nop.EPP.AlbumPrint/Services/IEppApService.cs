using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nop.EPP.AlbumPrint.Domain;

namespace Nop.EPP.AlbumPrint.Services
{
    public interface IEppApService
    {
        Task Insert(EppAp record);
    }

}
