using System.Xml;

namespace pjank.BossaAPI.Fixml
{
    /// <summary>
    /// Klasa ułatwiająca tworzenie/wysyłanie własnych komunikatów FIXML, 
    /// dla których nie zaimplementowano żadnej specjalnej, odrębnej klasy.
    /// Ewentualnie do wykorzystania przy testowaniu protokołu komunikacyjnego...
    /// </summary>
    public class CustomMsg : FixmlMsg
    {

        public CustomMsg(string name)
        {
            PrepareXmlMessage(name);
        }

        new public XmlElement AddElement(string name) { return base.AddElement(name); }
        new public XmlElement AddElement(XmlElement parent, string name) { return base.AddElement(parent, name); }

    }
}
