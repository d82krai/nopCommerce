using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nop.Data;
using Nop.EPP.AlbumPrint.Domain;

namespace Nop.EPP.AlbumPrint.Services
{
    public class EppApService : IEppApService
    {
        private readonly IRepository<EppAp> _eppApRepository;
        public EppApService(IRepository<EppAp> eppEpRepository)
        {
            _eppApRepository = eppEpRepository;
        }

        /// <summary>
        /// Logs the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        public virtual async Task Insert(EppAp record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));
            await _eppApRepository.InsertAsync(record);
        }
    }
}
