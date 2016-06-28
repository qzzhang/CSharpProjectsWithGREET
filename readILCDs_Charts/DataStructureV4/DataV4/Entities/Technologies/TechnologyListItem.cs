using System.ComponentModel;
using System.Reflection;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This class is used to mantain a flattened out list of Technology for a datagridview
    /// </summary>
    public class TechnologyListItem
    {
        string materialName;

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string MaterialName
        {
            get { return materialName; }
            set { materialName = value; }
        }

        string techologyName;
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string TechologyName
        {
            get { return techologyName; }
            set { techologyName = value; }
        }

        TechnologyData technologyTag;
        [Browsable(false)]
        public TechnologyData TechnologyTag
        {
            get { return technologyTag; }
            set { technologyTag = value; }
        }

        int materialId;
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int MaterialId
        {
            get { return materialId; }
            set { materialId = value; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int TechnologyId
        {
            get { return technologyTag.Id; }
            set { technologyTag.Id = value; }
        }
    }
}
