using Nop.Core.Data;
using Nop.Plugin.Soft2Print.Data.Entities;
using System.Linq;

namespace Nop.Plugin.Soft2Print.Data.Repositories
{
    public interface IProjectAttributeRepository
    {
        void Create(Entities.S2P_ProjectAttributes record);

        /// <summary>
        /// Use this to get attributes from a project
        /// </summary>
        Entities.S2P_ProjectAttributes Get(int projectID);

        /// <summary>
        /// Use this to copy product attribute data from one project to another
        /// </summary>
        void Copy(int fromProject, int toProjec);
    }

    public class ProjectAttributeRepository : IProjectAttributeRepository
    {
        private readonly IRepository<Entities.S2P_ProjectAttributes> _projectAttributeRepository;

        public ProjectAttributeRepository(IRepository<Entities.S2P_ProjectAttributes> projectAttributeRepository)
        {
            this._projectAttributeRepository = projectAttributeRepository;
        }

        /// <summary>
        /// Logs the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Create(Entities.S2P_ProjectAttributes record)
        {
            this._projectAttributeRepository.Insert(record);
        }

        public S2P_ProjectAttributes Get(int projectID)
        {
            var record = this._projectAttributeRepository.Table.FirstOrDefault(i => i.ProjectID.Equals(projectID));
            return record;
        }

        public void Copy(int fromProject, int toProjec)
        {

            var fromRecord = this._projectAttributeRepository.Table.FirstOrDefault(i => i.ProjectID.Equals(fromProject));
            if (fromRecord != null)
            {
                this._projectAttributeRepository.Insert(new S2P_ProjectAttributes()
                {
                    ProjectID = toProjec,
                    Attributes = fromRecord.Attributes,
                    ProductID = fromRecord.ProductID
                });
            }


        }

    }
}
