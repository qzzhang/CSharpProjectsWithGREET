using System;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.DataStructureV4
{
    [Serializable]
    internal class DVMatrix
    {
        #region attributes
        Matrix user;
        Matrix defo;
        BoolMatrix useDefault;

        #endregion

        #region constructors
        public DVMatrix(int _row, int _col)
        {
            if (_row == 0)
                throw new Exception("A matrix cannot be defined with 0 rows");
            else if (_col == 0)
                throw new Exception("A matrix cannot be defined with 0 columns");
            user = new Matrix(_row, _col);
            defo = new Matrix(_row, _col);
            useDefault = new BoolMatrix(_row, _col);
        }

        public DVMatrix(XmlNode node)
        {
            if (node.Attributes["value"] != null)
                this.defo = new Matrix(node.Attributes.GetNamedItem("value").Value);
            else
                return;
            if (node.Attributes["user_value"] != null)
                this.user = new Matrix(node.Attributes.GetNamedItem("user_value").Value);
            else
                this.user = new Matrix(this.defo.RowsCount, this.defo.ColsCount);
            if (node.Attributes["usedefault"] != null)
                this.useDefault = new BoolMatrix(node.Attributes.GetNamedItem("usedefault").Value);
            else
                this.useDefault = new BoolMatrix(this.defo.RowsCount, this.defo.ColsCount);
        }
        #endregion

        #region accessors
        public double this[int row, int col]
        {
            get
            {
                if(ChoiceMatrix[row, col])
                    return DeafultValuesMatrix[row, col];
                else
                    return UserValuesMatrix[row, col];
            }
            set
            {
                ChoiceMatrix[row, col] = false;
                UserValuesMatrix[row, col] = value;
            }
        }

        public int Count 
        {
            get 
            {
                if (UserValuesMatrix.Count == DeafultValuesMatrix.Count)
                    return UserValuesMatrix.Count;
                else
                    return -1;
            }
        }

        /// <summary>
        /// This method returns a DVMatrix made of a single row
        /// which corresponds to the row passed as a parameter.
        /// </summary>
        /// <param name="row">The desired Row</param>
        /// <returns>DVMatrix containing default and user values for the desired row</returns>
        internal DVMatrix GetRow(int row)
        {
            Matrix userRow = UserValuesMatrix.GetRow(row);
            Matrix defoRow = DeafultValuesMatrix.GetRow(row);
            BoolMatrix useDefaultRow = ChoiceMatrix.GetRow(row);

            if (userRow.ColsCount == DeafultValuesMatrix.ColsCount 
                && userRow.ColsCount == useDefaultRow.ColsCount)
            {
                DVMatrix mat = new DVMatrix(1, UserValuesMatrix.ColsCount);
                for (int i = 0; i < UserValuesMatrix.ColsCount; i++)
                {
                    mat.UserValuesMatrix[0, i] = userRow[i];
                    mat.DeafultValuesMatrix[0, i] = defoRow[i];
                    mat.ChoiceMatrix[0, i] = useDefaultRow[i];
                }
                return mat;
            }
            return null;
        }

        /// <summary>
        /// This method returns a DVMatrix made of a single column
        /// which corresponds to the column passed as a parameter.
        /// 
        /// David Dieffenthaler
        /// </summary>
        /// <param name="col">The desired Column</param>
        /// <returns>DVMatrix containing default and user values for the desired column</returns>
        internal DVMatrix GetColumn(int col)
        {
            Matrix userColumn = UserValuesMatrix.GetColumn(col);
            Matrix defoColumn = DeafultValuesMatrix.GetColumn(col);
            BoolMatrix useDefaultColumn = ChoiceMatrix.GetColumn(col);

            if (userColumn.RowsCount == DeafultValuesMatrix.RowsCount
                && userColumn.RowsCount == useDefaultColumn.RowsCount)
            {
                DVMatrix mat = new DVMatrix(UserValuesMatrix.RowsCount, 1);
                for (int i = 0; i < UserValuesMatrix.ColsCount; i++)
                {
                    mat.UserValuesMatrix[i, 0] = userColumn[i];
                    mat.DeafultValuesMatrix[i, 0] = defoColumn[i];
                    mat.ChoiceMatrix[i, 0] = useDefaultColumn[i];
                }
                return mat;
            }
            return null;
        }

        /// <summary>
        /// Returns a single matrix representing the values to be used for this DVMatrix object.
        /// The elements to be used are determined by the boolean of the useDefault matrix which 
        /// defines weather we want to use the default values or the user values.
        /// 
        /// David Dieffenthaler
        /// </summary>
        /// <returns>The elements to be used according to the useDefault elements</returns>
        internal Matrix GetValueToBeUsed()
        {
            Matrix valToUse = new Matrix(RowsCount, ColsCount);
            for (int i = 0; i < RowsCount; i++)
            {
                for (int j = 0; j < ColsCount; j++)
                {
                    if (ChoiceMatrix[i, j])
                        valToUse[i, j] = DeafultValuesMatrix[i, j];
                    else
                        valToUse[i, j] = UserValuesMatrix[i, j];
                }
            }
            return valToUse;
        }


        public int ColsCount 
        {
            get 
            {
                if (UserValuesMatrix.ColsCount == DeafultValuesMatrix.ColsCount)
                    return UserValuesMatrix.ColsCount;
                return -1;
            }
            set 
            {
                UserValuesMatrix.ColsCount = value;
                DeafultValuesMatrix.ColsCount = value;
                ChoiceMatrix.ColsCount = value;
            }
        }

        public int RowsCount 
        {
            get
            {
                if (UserValuesMatrix.RowsCount == DeafultValuesMatrix.RowsCount)
                    return UserValuesMatrix.RowsCount;
                return -1;
            }
            set 
            {
                UserValuesMatrix.RowsCount = value;
                DeafultValuesMatrix.RowsCount = value;
                ChoiceMatrix.RowsCount = value;
            }
        }

        public Matrix UserValuesMatrix
        {
            get { return user; }
            set { user = value; }
        }
        public Matrix DeafultValuesMatrix
        {
            get { return defo; }
            set { defo = value; }
        }

        public BoolMatrix ChoiceMatrix
        {
            get { return useDefault; }
            set { useDefault = value; }
        }
        
        #endregion

        #region methods
        public XmlNode toXmlNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateNode("transfer_matrix",
                doc.CreateAttr("value", this.DeafultValuesMatrix),
                doc.CreateAttr("user_value", this.UserValuesMatrix),
                doc.CreateAttr("usedefault", this.ChoiceMatrix)
                );
            return node;
        }
        #endregion

        #region exceptions
        class DimException : Exception
        {
            public DimException() :
                base("Matrix dimestions do not match") { }
        }
        class NodeCoumnsException : Exception
        {
            public NodeCoumnsException() :
                base("The transfer_matrix XmlNode attribute has data is corrupt. The number of eliments is not the same in each column") { }
        }
        class NotAVectorException : Exception
        {
            public NotAVectorException() :
                base("The object is not a vector and accessor ogj[i] cannot be used. Need to provide both indeces: obj[i,j]") { }
        }
        class ListException : Exception
        {
            public ListException() :
                base("The list is of the wrong size. The number of eliments is not the same in each column") { }
        }
        #endregion

        public double Sum 
        {
            get 
            {
                double somme = 0;
                for (int i = 0; i < this.ChoiceMatrix.RowsCount; i++)
                {
                    for (int j = 0; j < this.ChoiceMatrix.ColsCount;j++)
                    {
                        if (this.ChoiceMatrix[i, j])
                            somme += DeafultValuesMatrix[i, j];
                        else
                            somme += UserValuesMatrix[i, j];
                    }
                }
                return somme;
            }
        }

        internal bool Contains(int row, int col)
        {
            bool defoContains = DeafultValuesMatrix.Contains(row, col);
            bool userContains = UserValuesMatrix.Contains(row, col);
            return defoContains && userContains;
        }
    }
}
