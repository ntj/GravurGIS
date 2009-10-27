﻿// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;

using NPack;
using NPack.Interfaces;
//using SharpMap.Utilities;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace GravurGIS.Rendering.Rendering2D
{
    /// <summary>
    /// A point in 2 dimensional Cartesian space.
    /// </summary>
    [Serializable]
    public struct Point2D : IVectorD, IHasEmpty, IComputable<Double, Point2D>
    {
        public static readonly Point2D Empty = new Point2D();
        public static readonly Point2D Zero = new Point2D(0, 0);
        public static readonly Point2D One = new Point2D(1, 1);

        public static Point2D operator +(Point2D lhs, Point2D rhs)
        {
            return new Point2D(lhs.Add(rhs));
        }

        public static Point2D operator -(Point2D lhs, Point2D rhs)
        {
            return new Point2D(lhs.Subtract(rhs));
        }

        private DoubleComponent _x, _y;
        private Boolean _hasValue;

        #region Constructors
        public Point2D(Double x, Double y)
        {
            _x = x;
            _y = y;
            _hasValue = true;
        }

        public Point2D(Double[] elements)
        {
            if (elements == null) throw new ArgumentNullException("elements");

            if (elements.Length != 2)
            {
                throw new ArgumentException("Elements array must have only 2 components.");
            }

            _x = elements[0];
            _y = elements[1];
            _hasValue = true;
        }

        public Point2D(IVectorD vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            if (vector.ComponentCount != 2)
            {
                throw new ArgumentException("Elements array must have only 2 components.");
            }

            _x = vector[0];
            _y = vector[1];
            _hasValue = true;
        }
        #endregion

        #region ToString
        public override String ToString()
        {
            return String.Format("[Point2D] ({0:N3}, {1:N3})", _x, _y);
        }
        #endregion

        #region GetHashCode
        public override Int32 GetHashCode()
        {
            return unchecked((Int32)_x ^ (Int32)_y);
        }
        #endregion

        #region Equality Testing

        public static Boolean operator ==(Point2D lhs, Point2D rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(Point2D lhs, Point2D rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static Boolean operator ==(Point2D lhs, IVectorD rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(Point2D lhs, IVectorD rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is Point2D)
            {
                return Equals((Point2D)obj);
            }

            if (obj is IVectorD)
            {
                return Equals(obj as IVectorD);
            }

            return false;
        }

        public Boolean Equals(Point2D point)
        {
            return _x.Equals(point._x) &&
                _y.Equals(point._y) &&
                IsEmpty == point.IsEmpty;
        }

        #region IEquatable<IViewVector> Members

        public Boolean Equals(IVectorD other)
        {
            if (other == null)
            {
                return false;
            }

            if ((this as IVectorD).ComponentCount != other.ComponentCount)
            {
                return false;
            }

            if (!_x.Equals(other[0]) || !_y.Equals(other[1]))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region IEquatable<IMatrixD> Members

        ///<summary>
        ///Indicates whether the current object is equal to another object of the same type.
        ///</summary>
        ///
        ///<returns>
        ///true if the current object is equal to the other parameter; otherwise, false.
        ///</returns>
        ///
        ///<param name="other">An object to compare with this object.</param>
        public Boolean Equals(IMatrixD other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.RowCount != 1 || other.ColumnCount != 2)
            {
                return false;
            }

            if (!other[0, 0].Equals(_x) || !other[0, 1].Equals(_y))
            {
                return false;
            }

            return true;
        }

        #endregion
        #endregion

        #region Properties
        public Double X
        {
            get { return (Double)_x; }
        }

        public Double Y
        {
            get { return (Double)_y; }
        }

        public Double this[Int32 element]
        {
            get
            {
                checkIndex(element);

                return element == 0 ? (Double)_x : (Double)_y;
            }
        }

        public Boolean IsEmpty
        {
            get { return !_hasValue; }
        }
        #endregion

        #region Clone
        public Point2D Clone()
        {
            return new Point2D((Double)_x, (Double)_y);
        }
        #endregion

        #region Negative
        public Point2D Negative()
        {
            return new Point2D((Double)_x.Negative(), (Double)_y.Negative());
        }
        #endregion

        #region IComputable<Double,Point2D> Members

        public Point2D Abs()
        {
            throw new NotImplementedException();
        }

        Point2D IComputable<Double, Point2D>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<Point2D> Members

        public Point2D Subtract(Point2D b)
        {
            return new Point2D(X - b.X, Y - b.Y);
        }

        #endregion

        #region IHasZero<Point2D> Members

        Point2D IHasZero<Point2D>.Zero
        {
            get { return Zero; }
        }

        #endregion

        #region IAddable<Point2D> Members

        public Point2D Add(Point2D b)
        {
            return new Point2D(X + b.X, Y + b.Y);
        }

        #endregion

        #region IDivisible<Point2D> Members

        public Point2D Divide(Point2D b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasOne<Point2D> Members

        Point2D IHasOne<Point2D>.One
        {
            get { return One; }
        }

        #endregion

        #region IMultipliable<Point2D> Members

        public Point2D Multiply(Point2D b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<Double,Point2D> Members

        public Point2D Divide(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<Point2D> Members

        public Boolean GreaterThan(Point2D value)
        {
            throw new NotImplementedException();
        }

        public Boolean GreaterThanOrEqualTo(Point2D value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThan(Point2D value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThanOrEqualTo(Point2D value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<Point2D> Members

        Point2D IExponential<Point2D>.Exp()
        {
            throw new NotImplementedException();
        }

        Point2D IExponential<Point2D>.Log()
        {
            throw new NotImplementedException();
        }

        Point2D IExponential<Point2D>.Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        public Point2D Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        public Point2D Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<Double> Members

        public IEnumerator<Double> GetEnumerator()
        {
            yield return (Double)_x;
            yield return (Double)_y;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IVectorD Members

        MatrixFormat IMatrixD.Format
        {
            get { return MatrixFormat.Unspecified; }
        }

        IVectorD IVectorD.Clone()
        {
            return Clone();
        }

        IVectorD IVectorD.Negative()
        {
            return Negative();
        }

        Int32 IVectorD.ComponentCount
        {
            get { return IsEmpty ? 0 : 2; }
        }

        /// <summary>
        /// Gets or sets the vector component array.
        /// </summary>
        DoubleComponent[] IVectorD.Components
        {
            get { return new DoubleComponent[] { _x, _y }; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                checkIndex(value.Length - 1);

                _x = value[0];
                _y = value[1];
            }
        }

        DoubleComponent IVectorD.this[Int32 index]
        {
            get
            {
                checkIndex(index);
                return this[index];
            }
            set
            {
                checkIndex(index);

                if (index == 0)
                {
                    _x = value;
                }
                else
                {
                    _y = value;
                }

                _hasValue = true;
            }
        }

        #endregion

        #region IAddable<IMatrixD> Members

        /// <summary>
        /// Returns the sum of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The sum.</returns>
        IMatrixD IAddable<IMatrixD>.Add(IMatrixD b)
        {
            //return MatrixProcessor.Add(this, b);
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<IMatrixD> Members

        /// <summary>
        /// Returns the difference of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The difference.</returns>
        IMatrixD ISubtractable<IMatrixD>.Subtract(IMatrixD b)
        {
            //return MatrixProcessor.Subtract(this, b);
            throw new NotImplementedException();
        }

        #endregion

        #region IHasZero<IMatrixD> Members

        /// <summary>
        /// Returns the additive identity.
        /// </summary>
        /// <value>e</value>
        IMatrixD IHasZero<IMatrixD>.Zero
        {
            get { return Zero; }
        }

        #endregion

        #region INegatable<IMatrixD> Members

        /// <summary>
        /// Returns the negative of the object. Must not modify the object itself.
        /// </summary>
        /// <returns>The negative.</returns>
        IMatrixD INegatable<IMatrixD>.Negative()
        {
            return Negative();
        }

        #endregion

        #region IMultipliable<IMatrixD> Members

        /// <summary>
        /// Returns the product of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The product.</returns>
        IMatrixD IMultipliable<IMatrixD>.Multiply(IMatrixD b)
        {
            //return MatrixProcessor.Multiply(this, b);
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<IMatrixD> Members

        /// <summary>
        /// Returns the quotient of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The quotient.</returns>
        IMatrixD IDivisible<IMatrixD>.Divide(IMatrixD b)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IHasOne<IMatrixD> Members

        /// <summary>
        /// Returns the multiplicative identity.
        /// </summary>
        /// <value>e</value>
        IMatrixD IHasOne<IMatrixD>.One
        {
            get { return One; }
        }

        #endregion

        #region IEnumerable<DoubleComponent> Members

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        IEnumerator<DoubleComponent> IEnumerable<DoubleComponent>.GetEnumerator()
        {
            yield return _x;
            yield return _y;
        }

        #endregion

        #region IMatrixD Members
        /// <summary>
        /// Makes an element-by-element copy of the matrix.
        /// </summary>
        /// <returns>An exact copy of the matrix.</returns>
        IMatrixD IMatrixD.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Gets the determinant for the matrix, if it exists.
        /// </summary>
        Double IMatrixD.Determinant
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the number of columns in the matrix.
        /// </summary>
        Int32 IMatrixD.ColumnCount
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets true if the matrix is singular (non-invertable).
        /// </summary>
        Boolean IMatrixD.IsSingular
        {
            get { return true; }
        }

        /// <summary>
        /// Gets true if the matrix is invertable (non-singluar).
        /// </summary>
        Boolean IMatrixD.IsInvertible
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the inverse of the matrix, if one exists.
        /// </summary>
        IMatrixD IMatrixD.Inverse
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets true if the matrix is square (<c>RowCount == ColumnCount != 0</c>).
        /// </summary>
        Boolean IMatrixD.IsSquare
        {
            get { return false; }
        }

        /// <summary>
        /// Gets true if the matrix is symmetrical.
        /// </summary>
        Boolean IMatrixD.IsSymmetrical
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the number of rows in the matrix.
        /// </summary>
        Int32 IMatrixD.RowCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets a submatrix.
        /// </summary>
        /// <param name="rowIndexes">The indexes of the rows to include.</param>
        /// <param name="j0">The starting column to include.</param>
        /// <param name="j1">The ending column to include.</param>
        /// <returns></returns>
        IMatrixD IMatrixD.GetMatrix(Int32[] rowIndexes, Int32 j0, Int32 j1)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets or sets an element in the matrix.
        /// </summary>
        /// <param name="row">The index of the row of the element.</param>
        /// <param name="column">The index of the column of the element.</param>
        /// <returns>The value of the element at the given index.</returns>
        DoubleComponent IMatrixD.this[Int32 row, Int32 column]
        {
            get
            {
                checkIndexes(row, column);

                return this[column];
            }
            set
            {
                checkIndexes(row, column);

                (this as IVectorD)[column] = value;
            }
        }

        /// <summary>
        /// Returns the transpose of the matrix.
        /// </summary>
        /// <returns>The matrix with the rows as columns and columns as rows.</returns>
        IMatrixD IMatrixD.Transpose()
        {
            //return new Matrix<DoubleComponent>((this as IMatrixD).Format,
            //        new DoubleComponent[][] { new DoubleComponent[] { _x }, new DoubleComponent[] { _y } });

            throw new NotImplementedException();
        }

        #endregion

        #region INegatable<IVectorD> Members

        IVectorD INegatable<IVectorD>.Negative()
        {
            return new Point2D(-X, -Y);
        }

        #endregion

        #region ISubtractable<IVectorD> Members

        IVectorD ISubtractable<IVectorD>.Subtract(IVectorD b)
        {
            if (b == null) throw new ArgumentNullException("b");

            if (b.ComponentCount != 2)
            {
                throw new ArgumentException("Vector must have only 2 components.");
            }

            return new Point2D(X - (Double)b[0], Y - (Double)b[1]);
        }

        #endregion

        #region IHasZero<IVectorD> Members

        IVectorD IHasZero<IVectorD>.Zero
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IAddable<IVectorD> Members

        IVectorD IAddable<IVectorD>.Add(IVectorD b)
        {
            if (b == null) throw new ArgumentNullException("b");

            if (b.ComponentCount != 2)
            {
                throw new ArgumentException("Vector must have only 2 components.");
            }

            return new Point2D(X + (Double)b[0], Y + (Double)b[1]);
        }

        #endregion

        #region IDivisible<IVectorD> Members

        IVectorD IDivisible<IVectorD>.Divide(IVectorD b)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IDivisible<Double, IVectorD> Members

        IVectorD IDivisible<Double, IVectorD>.Divide(Double b)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IHasOne<IVectorD> Members

        IVectorD IHasOne<IVectorD>.One
        {
            get { return new Point2D(1, 1); }
        }

        #endregion

        #region IMultipliable<IVectorD> Members

        IVectorD IMultipliable<IVectorD>.Multiply(IVectorD b)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IComparable<IMatrixD> Members

        Int32 IComparable<IMatrixD>.CompareTo(IMatrixD other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<IMatrixD> Members

        IMatrixD IComputable<IMatrixD>.Abs()
        {
            throw new NotImplementedException();
        }

        IMatrixD IComputable<IMatrixD>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<IMatrixD> Members

        Boolean IBooleanComparable<IMatrixD>.GreaterThan(IMatrixD value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrixD>.GreaterThanOrEqualTo(IMatrixD value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrixD>.LessThan(IMatrixD value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrixD>.LessThanOrEqualTo(IMatrixD value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<IMatrixD> Members

        IMatrixD IExponential<IMatrixD>.Exp()
        {
            throw new NotImplementedException();
        }

        IMatrixD IExponential<IMatrixD>.Log()
        {
            throw new NotImplementedException();
        }

        IMatrixD IExponential<IMatrixD>.Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        IMatrixD IExponential<IMatrixD>.Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        IMatrixD IExponential<IMatrixD>.Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<IVectorD> Members

        IVectorD IComputable<IVectorD>.Abs()
        {
            throw new NotImplementedException();
        }

        IVectorD IComputable<Double, IVectorD>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<IVectorD> Members

        Boolean IBooleanComparable<IVectorD>.GreaterThan(IVectorD value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IVectorD>.GreaterThanOrEqualTo(IVectorD value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IVectorD>.LessThan(IVectorD value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IVectorD>.LessThanOrEqualTo(IVectorD value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<IVectorD> Members

        IVectorD IExponential<IVectorD>.Exp()
        {
            throw new NotImplementedException();
        }

        IVectorD IExponential<IVectorD>.Log()
        {
            throw new NotImplementedException();
        }

        IVectorD IExponential<IVectorD>.Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        IVectorD IExponential<IVectorD>.Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        IVectorD IExponential<IVectorD>.Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable<IVectorD> Members

        Int32 IComparable<IVectorD>.CompareTo(IVectorD other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Helper Methods

        private static void checkIndex(Int32 index)
        {
            if (index != 0 && index != 1)
            {
                throw new ArgumentOutOfRangeExceptionEx("index", index, "The element index must be either 0 or 1 for a 2D point.");
            }
        }

        private static void checkIndexes(Int32 row, Int32 column)
        {
            if (row != 0)
            {
                throw new ArgumentOutOfRangeExceptionEx("row", row, "A Point2D has only 1 row.");
            }

            if (column < 0 || column > 1)
            {
                throw new ArgumentOutOfRangeExceptionEx("column", row, "A Point2D has only 2 columns.");
            }
        }
        #endregion

        #region IComputable<IVectorD> Members

        IVectorD IComputable<IVectorD>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultipliable<Double,IVectorD> Members

        IVectorD IMultipliable<Double, IVectorD>.Multiply(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<Point2D> Members
        Point2D IComputable<Point2D>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultipliable<Double,Point2D> Members

        public Point2D Multiply(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IAddable<double,IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Add(double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<double,IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Subtract(double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IAddable<double,Point2D> Members

        Point2D IAddable<double, Point2D>.Add(double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<double,Point2D> Members

        Point2D ISubtractable<double, Point2D>.Subtract(double b)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
