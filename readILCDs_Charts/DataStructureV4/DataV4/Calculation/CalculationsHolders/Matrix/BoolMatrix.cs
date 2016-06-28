using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.DataStructureV4
{
    [Serializable]
    internal class BoolMatrix
    {
        #region attributes
        int row, col;
        bool[,] dat;
        bool isvalid;
        #endregion
        #region constructors
        /// <summary>
        /// Default constructor, creates a Matix of the size _row and _col
        /// All elements are zeros
        /// </summary>
        /// <param name="_row">Number of desired rows</param>
        /// <param name="_col">Number of desired columns</param>
        public BoolMatrix(int _row, int _col)
        {
            row = _row;
            col = _col;
            dat = new bool[row, col];
            this.True();
        }

        /// <summary>
        /// Creates a squared matrix of size _sz
        /// </summary>
        /// <param name="_sz">Number of rows and columns</param>
        public BoolMatrix(int _sz) : this(_sz, _sz) { }

        /// <summary>
        /// Creates a Matrix with the elements copied from an array of elements
        /// </summary>
        /// <param name="_dat">Array of elements to be copied in the new Matrix object</param>
        public BoolMatrix(bool[,] _dat)
        {
            if (_dat == null)
            {
                this.isvalid = false;
                return;
            }
            this.dat = _dat;
            this.row = dat.GetLength(0);
            this.col = dat.GetLength(1);
        }

        /// <summary>
        /// Creates a Matrix with the elements copied from a List of elements
        /// </summary>
        /// <param name="_l">List of elements to be copied in the new Matrix object</param>
        public BoolMatrix(List<List<bool>> _l)
        {
            if (_l.Count() == 0)
                throw new ListException();
            this.col = _l.Count;
            this.row = _l[0].Count;
            for (int i = 0; i <= this.col; i++)
            {
                if (_l[i].Count != this.row)
                    throw new ListException();
                for (int j = 0; j <= this.row; j++)
                {
                    this.dat[i, j] = _l[i][j];
                }
            }
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="A">The Matrix from where elements are going to be copied</param>
        public BoolMatrix(BoolMatrix A) : this(A.row, A.col)
        {
            for (int i = 0; i < this.row; i++)
                for (int j = 0; j < this.col; j++)
                    this[i, j] = A[i, j];

        }

        /// <summary>
        /// Creates a Matrix class from a string, using the following model 
        /// 
        /// The following value string describes "{{1 2 3},{4 5 6}}"
        /// a matrix of size 2x3, 2 rows and 3 columns
        /// </summary>
        /// <param name="value">The string representing the matris</param>
        public BoolMatrix(string value)
        {
            //the following value string describes "{{1 2 3},{4 5 6}}"
            // a matrix of size 2x3, 2 rows and 3 columns
            value = value.TrimEnd('}');
            value = value.TrimStart('{');
            if (String.IsNullOrEmpty(value) == false)
            {
                string[] rows = value.Split(','); //rows a split using a comma
                this.row = rows.Length;
                string first_row = rows[0];
                first_row = first_row.TrimEnd('}'); first_row = first_row.TrimStart('{');
                string[] first_row_split = first_row.Split(' ');
                this.col = first_row_split.Length;
                this.dat = new bool[row, col];
                for (int i = 0; i < this.row; i++)
                {
                    string row_s = rows[i];
                    row_s = row_s.TrimEnd('}'); row_s = row_s.TrimStart('{');
                    string[] splitRow = row_s.Split(' ');
                    for (int j = 0; j < this.col; j++)
                        this.dat[i, j] = Convert.ToBoolean(splitRow[j]);
                }
            }
        }

        /// <summary>
        /// Creates a Matrix object from an XML node
        /// </summary>
        /// <param name="node">XMLNode object containing a string representation of the Matrix</param>
        public BoolMatrix(XmlNode node) : this(node.Attributes.GetNamedItem("value").Value) {}

        #endregion
        #region methods

        /// <summary>
        /// Outputs a string representing the elements of the matrix
        /// The resulting string has this format: "{{1 2 3},{4 5 6}}"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //the resulting string has this format: "{{1 2 3},{4 5 6}}"
            string s = "{";
            for (int i = 0; i < this.row; i++)
            {
                s += "{";
                for (int j = 0; j < this.col; j++)
                {
                    s += this[i, j].ToString();
                    s += " ";
                }
                s = s.TrimEnd(' ');
                s += "},";
            }
            s = s.TrimEnd(',');
            s += "}";
            return s;
        }

        /// <summary>
        /// This method generates an XmlNode that corresponds to the current matrix object.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public XmlNode toXmlNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateNode("transfer_matrix", doc.CreateAttr("value", this.ToString()));
            return node;
        }

        /// <summary>
        /// Returns the jth column of the matrix
        /// </summary>
        /// <param name="col">The desired column number to be returned</param>
        /// <returns>A single column matrix representing the desired column of the current object</returns>
        public BoolMatrix GetColumn(int col)
        {
            BoolMatrix res = new BoolMatrix(this.row, 1);
            for (int i = 0; i < this.row; i++)
                res[i, 0] = this[i, col];
            return res;
        }

        /// <summary>
        /// Returns the ith row of the matrix
        /// </summary>
        /// <param name="row">The desired row number to be returned</param>
        /// <returns>A single row matrix representing the desired row of the current oject</returns>
        public BoolMatrix GetRow(int row)
        {
            BoolMatrix res = new BoolMatrix(1, this.col);
            for (int j = 0; j < this.col; j++)
                res[0, j] = this[row, j];
            return res;
        }

        public void True()
        {
            for (int i = 0; i < this.row; i++)
                for (int j = 0; j < this.col; j++)
                    this[i, j] = true;
        }
        
        #endregion
        #region accessors
        public bool this[int row, int col]
        {
            get { return dat[row, col]; }
            set { dat[row, col] = value; }
        }
        public bool this[int i]
        {
            get
            {
                if (this.col == 1)
                    return dat[i, 0];
                else if (this.row == 1)
                    return dat[0, i];
                else
                    throw new NotAVectorException();
            }
            set
            {
                if (this.col == 1)
                    dat[i, 0] = value;
                else if (this.row == 1)
                    dat[0, i] = value;
                else
                    throw new NotAVectorException();
            }
        }
        public bool[,] toArray
        {
            get { return dat; }
        }
        public int Count
        {
            get { return dat.Length; }
        }

        public bool Isvalid
        {
            get { return isvalid; }
            set { isvalid = value; }
        }

        /// <summary>
        /// Get the number of rows 
        /// </summary>
        public int RowsCount
        {
            get
            {
                return row;
            }
            set
            {
                int rows = dat.GetLength(0);
                if (rows < value)
                {//we need to grow
                    bool[,] newData = new bool[value, col];
                    int cols = dat.GetLength(1);
                    for (int i = 0; i < row; i++)
                        for (int j = 0; j < cols; j++)
                            newData[i, j] = dat[i, j];
                    this.dat = newData;
                }
                else
                {//we need to shrink
                    bool[,] newData = new bool[value, col];
                    int cols = dat.GetLength(1);
                    for (int i = 0; i < value; i++)
                        for (int j = 0; j < cols; j++)
                            newData[i, j] = dat[i, j];
                    this.dat = newData;
                }
            }
        }

        /// <summary>
        /// Get the number of columns
        /// </summary>
        public int ColsCount
        {
            get 
            {
                return col;
            }
            set
            {
                int cols = dat.GetLength(1);
                if (cols < value)
                {//we need to grow
                    bool[,] newData = new bool[row, value];
                    int rows = dat.GetLength(0);
                    for (int i = 0; i < row; i++)
                        for (int j = 0; j < cols; j++)
                            newData[i, j] = dat[i, j];
                    this.dat = newData;
                }
                else
                {//we need to shrink
                    bool[,] newData = new bool[row, value];
                    int rows = dat.GetLength(0);
                    for (int i = 0; i < row; i++)
                        for (int j = 0; j < value; j++)
                            newData[i, j] = dat[i, j];
                    this.dat = newData;
                }
            }
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
    }
}
