using System;
using Greet.DataStructureV4.Interfaces;


namespace Greet.DataStructureV4.Entities
{
    public class DependentItem : IDependentItem
    {

        #region attributes
        private Greet.DataStructureV4.Interfaces.Enumerators.ItemType type;
        private int id;
        private String name;
        #endregion

        #region constructors

        public DependentItem(Vehicle veh)
        {
            this.id = veh.Id;
            this.type = Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Vehicle;
            this.name = veh.Name;
        }

        public DependentItem(Pathway pw)
        {
            this.id = pw.Id;
            this.type = Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Pathway;
            this.name = pw.Name;
        }

        public DependentItem(Mix mix)
        {
            this.id = mix.Id;
            this.type = Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Pathway_Mix;
            this.name = mix.Name;
        }

        public DependentItem(Monitor m)
        {
            this.id = m.ResultArrayindex;
            this.type = Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Monitor;
            this.name = m.ToString();
        }

        public DependentItem(AProcess p)
        {
            this.id = p.Id;
            this.type = Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Process;
            this.name = p.Name;
        }

        public DependentItem(TechnologyData td)
        {
            this.id = td.Id;
            this.type = Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Technology;
            this.name = td.Name;
        }

        public DependentItem(AMode m)
        {
            this.id = m.Id;
            this.type = Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Mode;
            this.name = m.Name;
        }

        public DependentItem(ResourceData r)
        {
            this.id = r.Id;
            this.type = Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Resource;
            this.name = r.Name;
        }

        public DependentItem(Parameter p)
        {
            this.id = -1;
            Int32.TryParse(p.Id, out this.id);
            this.type = Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Parameter;
            this.name = p.Name;
        }



        #endregion

        #region accessors
        public int Id
        {
            get { return this.id; }
        }

        public String Name
        {
            get { return this.name; }
        }

        public Greet.DataStructureV4.Interfaces.Enumerators.ItemType Type
        {
            get { return this.type; }
        }

        public string TypeName
        {
            get
            {
                if (this.type == Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Vehicle)
                    return "Vehicle";
                else if (this.type == Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Pathway)
                    return "Pathway";
                else if (this.type == Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Pathway_Mix)
                    return "Pathway Mix";
                else if (this.type == Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Monitor)
                    return "Monitor";
                else if (this.type == Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Process)
                    return "Process";
                else if (this.type == Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Technology)
                    return "Technology";
                else if (this.type == Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Vehicle_Monitor)
                    return "Vehicle Monitor";
                else if (this.type == Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Mode)
                    return "Mode";
                else
                    return "Data";
            }
        }

        public bool Equals(IDependentItem other)
        {
            if (this.Type == other.Type && this.TypeName == other.TypeName && this.Id == other.Id)
                return true;
            else
                return false;
        }


        #endregion

    }
}
