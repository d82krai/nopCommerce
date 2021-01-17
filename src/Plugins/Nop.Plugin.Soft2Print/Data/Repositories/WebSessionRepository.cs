using Nop.Core.Data;
using System;
using System.Linq;

namespace Nop.Plugin.Soft2Print.Data.Repositories
{
    public interface IWebSessionRepository
    {
        /// <summary>
        /// Use this to get the latest active s2p sessionID
        /// </summary>
        Guid? GetLastSessionID(int customerID);
        /// <summary>
        /// Use this to get the nop customer id from a s2p sessionID
        /// </summary>
        int GetCustomerID(Guid sessionID);

        /// <summary>
        /// Use this to mark a s2p sessionId as invalid
        /// </summary>
        void MarkAsInvalid(Guid sessionID);

        /// <summary>
        /// Use this to save a s2p sessionID
        /// </summary>
        /// <param name="record"></param>
        void Create(Guid sessionID, int customerID, bool isValid = true);
    }

    public class WebSessionRepository : IWebSessionRepository
    {
        private readonly IRepository<Entities.S2P_WebSession> _authenticationRepository;

        public WebSessionRepository(IRepository<Entities.S2P_WebSession> authenticationRepository)
        {
            this._authenticationRepository = authenticationRepository;
        }

        public void Create(Guid sessionID, int customerID, bool isValid = true)
        {
            this._authenticationRepository.Insert(new Entities.S2P_WebSession()
            {
                Id = sessionID,
                CustomerId = customerID,
                Valid = isValid
            });
        }

        public int GetCustomerID(Guid sessionID)
        {
            return this._authenticationRepository.Table.Single(i => i.Id.Equals(sessionID)).CustomerId;
        }

        public Guid? GetLastSessionID(int customerID)
        {
            var result = this._authenticationRepository.Table.Where(i => i.CustomerId.Equals(customerID) && i.Valid).OrderByDescending(i => i.Created).FirstOrDefault();
            if (result != null)
                return result.Id;
            else
                return null;
        }

        public void MarkAsInvalid(Guid id)
        {
            var session = this._authenticationRepository.GetById(id);
            session.Valid = false;
            this._authenticationRepository.Update(session);
        }
    }
}
