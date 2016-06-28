using System.ComponentModel;
using System.Reflection;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This class is used to mantain a flattened out list of Mix for a datagridview
    /// </summary>
    public class MixListItem
    {
        #region attributes
        string materialName;
        string mixName;
        Mix mixTag;
        int materialId;
        #endregion

        #region Accessors

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string MaterialName
        {
            get { return materialName; }
            set { materialName = value; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string MixName
        {
            get { return mixName; }
            set { mixName = value; }
        }

        [Browsable(false), Obfuscation(Feature = "renaming", Exclude = true)]
        public Mix MixTag
        {
            get { return mixTag; }
            set { mixTag = value; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int MaterialId
        {
            get { return materialId; }
            set { materialId = value; }
        }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int MixId
        {
            get { return mixTag.Id; }
            set { mixTag.Id = value; }
        }
        #endregion
    }
}
