using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Nop.Plugin.Soft2Print.Services
{
    public class ProductAttributeService
    {
        public class AttributeKeys
        {
            public const string Prefix = "Soft2Print Attribute - ";
            public const string ExtraPages = Prefix + "Extra pages";
            public const string TotalPages = Prefix + "Total pages";
            public const string MainPages = Prefix + "Main pages";
            public const string PrintImages = Prefix + "Prints";


            public const string ThemeIdentifier = Prefix + "Theme - PreSelected Theme";
            public const string ThemeStrcuture = Prefix + "Theme - Structure";
            public const string ThemeStrcutureOnSite = Prefix + "Theme - Structure - Show on product page";
        }


        private readonly Data.Repositories.IProjectAttributeRepository _projectAttributeRepository;

        public ProductAttributeService(Data.Repositories.IProjectAttributeRepository projectAttributeRepository)
        {
            this._projectAttributeRepository = projectAttributeRepository;
        }

        public void SaveNopAttributesToDB(int projectID, int productID, string productAttritures)
        {
            if (!string.IsNullOrEmpty(productAttritures))
            {
                this._projectAttributeRepository.Create(new Data.Entities.S2P_ProjectAttributes()
                {
                    ProjectID = projectID,
                    ProductID = productID,
                    Attributes = productAttritures
                });
            }
        }

        public Model.S2PProductAttributes GetS2PProductAttributes(string nopProductAttributeXML)
        {
            var xml = this.GetXElement(nopProductAttributeXML);

            var s2pElement = xml.Elements().SingleOrDefault(i => i.Name.LocalName.Equals(this.GetS2PElementName()));
            if (s2pElement != null)
                return this.FromXElement<Model.S2PProductAttributes>(s2pElement);
            else
                return null;
        }
        public string UpdateS2PProductAttributes(string nopProductAttributeXML, Model.S2PProductAttributes s2pProductAttributes)
        {
            var xml = this.GetXElement(nopProductAttributeXML);

            // Remove existing elements
            var s2pElement = xml.Elements().SingleOrDefault(i => i.Name.LocalName.Equals(this.GetS2PElementName()));
            if (s2pElement != null)
                s2pElement.Remove();

            // Add the new element
            xml.Add(this.ToXElement<Model.S2PProductAttributes>(s2pProductAttributes));

            return xml.ToString(SaveOptions.DisableFormatting);
        }


        private string GetS2PElementName()
        {
            return this.ToXElement<Model.S2PProductAttributes>(new Model.S2PProductAttributes()).Name.LocalName;
        }
        private XElement GetXElement(string nopProductAttributeXML)
        {
            XElement xml;

            if (!string.IsNullOrEmpty(nopProductAttributeXML))
                xml = XElement.Parse(nopProductAttributeXML);
            else
                xml = new XElement("Attributes");

            return xml;
        }



        private XElement ToXElement<T>(T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(streamWriter, obj);
                    return XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));
                }
            }
        }
        private T FromXElement<T>(XElement xElement)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(xElement.CreateReader());
        }


    }
}
