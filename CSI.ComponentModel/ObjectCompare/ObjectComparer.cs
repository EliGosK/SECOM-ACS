namespace CSI.ObjectCompare
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using System.Text;

    public class ObjectComparer
    {
        private ObjectCompareResultCollection _differences;
        private List<string> _elementsToIgnore;
        private int _maxDifferences;
        private List<object> _parents;
        private ObjectCompareOptions compareOptions;

        public ObjectComparer() : this(1)
        {
        }

        public ObjectComparer(int maxDifference)
        {
            this._differences = new ObjectCompareResultCollection();
            this.compareOptions = ObjectCompareOptions.Default;
            this._parents = new List<object>();
            this._elementsToIgnore = new List<string>();
            this._maxDifferences = 1;
            this.MaxDifferences = maxDifference;
        }

        private string AddBreadCrumb(string existing, string name, string extra, int index)
        {
            return this.AddBreadCrumb(existing, name, extra, (index >= 0) ? index.ToString() : null);
        }

        private string AddBreadCrumb(string existing, string name, string extra, string index)
        {
            bool flag = !string.IsNullOrEmpty(index);
            bool flag2 = name.Length > 0;
            StringBuilder builder = new StringBuilder();
            builder.Append(existing);
            if (flag2)
            {
                builder.AppendFormat(".", new object[0]);
                builder.Append(name);
            }
            builder.Append(extra);
            if (flag)
            {
                int result = -1;
                if (int.TryParse(index, out result))
                {
                    builder.AppendFormat("[{0}]", index);
                }
                else
                {
                    builder.AppendFormat("[\"{0}\"]", index);
                }
            }
            return builder.ToString();
        }

        public bool Compare(object object1, object object2)
        {
            return this.Compare(object1, object2, ObjectCompareOptions.Default);
        }

        public bool Compare(object object1, object object2, ObjectCompareOptions compareOptions)
        {
            string breadCrumb = string.Empty;
            this.compareOptions = compareOptions;
            this.Differences.Clear();
            this.Compare(object1, object2, breadCrumb);
            return (this.Differences.Count == 0);
        }

        private void Compare(object object1, object object2, string breadCrumb)
        {
            if ((object1 != null) || (object2 != null))
            {
                if (object1 == null)
                {
                    this.Differences.Add(object1, object2, 0, breadCrumb, string.Format("object1{0} == null && object2{0} != null ((null),{1})", breadCrumb, this.cStr(object2)));
                }
                else if (object2 == null)
                {
                    this.Differences.Add(object1, object2, 0, breadCrumb, string.Format("object1{0} != null && object2{0} == null ({1},(null))", breadCrumb, this.cStr(object1)));
                }
                else
                {
                    Type t = object1.GetType();
                    Type type = object2.GetType();
                    if (t != type)
                    {
                        this.Differences.Add(object1, object2, 0, breadCrumb, string.Format("Different Types:  object1{0}.GetType() != object2{0}.GetType()", breadCrumb));
                    }
                    else if (TypeHelper.IsDataset(t))
                    {
                        this.CompareDataset(object1, object2, breadCrumb);
                    }
                    else if (TypeHelper.IsDataTable(t))
                    {
                        this.CompareDataTable(object1, object2, breadCrumb);
                    }
                    else if (TypeHelper.IsDataRow(t))
                    {
                        this.CompareDataRow(object1, object2, breadCrumb);
                    }
                    else if (TypeHelper.IsIList(t))
                    {
                        this.CompareIList(object1, object2, breadCrumb);
                    }
                    else if (TypeHelper.IsIDictionary(t))
                    {
                        this.CompareIDictionary(object1, object2, breadCrumb);
                    }
                    else if (TypeHelper.IsEnum(t))
                    {
                        this.CompareEnum(object1, object2, breadCrumb);
                    }
                    else if (TypeHelper.IsSimpleType(t))
                    {
                        this.CompareSimpleType(object1, object2, breadCrumb);
                    }
                    else if (TypeHelper.IsClass(t))
                    {
                        this.CompareClass(object1, object2, breadCrumb);
                    }
                    else if (TypeHelper.IsTimespan(t))
                    {
                        this.CompareTimespan(object1, object2, breadCrumb);
                    }
                    else
                    {
                        if (!TypeHelper.IsStruct(t))
                        {
                            throw new NotImplementedException("Cannot compare object of type " + t.Name);
                        }
                        this.CompareStruct(object1, object2, breadCrumb);
                    }
                }
            }
        }

        private void CompareClass(object object1, object object2, string breadCrumb)
        {
            try
            {
                this._parents.Add(object1);
                this._parents.Add(object2);
                Type type = object1.GetType();
                if (!this.ElementsToIgnore.Contains(type.Name))
                {
                    if (this.CompareProperties)
                    {
                        this.PerformCompareProperties(type, object1, object2, breadCrumb);
                    }
                    if (this.CompareFields)
                    {
                        this.PerformCompareFields(type, object1, object2, breadCrumb);
                    }
                }
            }
            finally
            {
                this._parents.Remove(object1);
                this._parents.Remove(object2);
            }
        }

        private void CompareDataRow(object object1, object object2, string breadCrumb)
        {
            DataRow row = object1 as DataRow;
            DataRow row2 = object2 as DataRow;
            if (row == null)
            {
                throw new ArgumentNullException("object1");
            }
            if (row2 == null)
            {
                throw new ArgumentNullException("object2");
            }
            for (int i = 0; i < row.Table.Columns.Count; i++)
            {
                if ((!this.ElementsToIgnore.Contains(row.Table.Columns[i].ColumnName) && (this.CompareReadOnly || !row.Table.Columns[i].ReadOnly)) && (!row.IsNull(i) || !row2.IsNull(i)))
                {
                    string str = this.AddBreadCrumb(breadCrumb, string.Empty, string.Empty, row.Table.Columns[i].ColumnName);
                    if (row.IsNull(i))
                    {
                        this.Differences.Add(object1, object2, 0, str, string.Format("object1{0} == null && object2{0} != null ((null),{1})", str, this.cStr(object2)));
                        return;
                    }
                    if (row2.IsNull(i))
                    {
                        this.Differences.Add(object1, object2, 0, str, string.Format("object1{0} != null && object2{0} == null ({1},(null))", str, this.cStr(object1)));
                        return;
                    }
                    this.Compare(row[i], row2[i], str);
                    if (this.Differences.Count >= this.MaxDifferences)
                    {
                        return;
                    }
                }
            }
        }

        private void CompareDataset(object object1, object object2, string breadCrumb)
        {
            DataSet set = object1 as DataSet;
            DataSet set2 = object2 as DataSet;
            if (set == null)
            {
                throw new ArgumentNullException("object1");
            }
            if (set2 == null)
            {
                throw new ArgumentNullException("object2");
            }
            if (set.Tables.Count != set2.Tables.Count)
            {
                this.Differences.Add(object1, object2, 0, breadCrumb, string.Format("object1{0}.Tables.Count != object2{0}.Tables.Count ({1},{2})", breadCrumb, set.Tables.Count, set2.Tables.Count));
                if (this.Differences.Count >= this.MaxDifferences)
                {
                    return;
                }
            }
            for (int i = 0; i < set.Tables.Count; i++)
            {
                string str = this.AddBreadCrumb(breadCrumb, "Tables", string.Empty, set.Tables[i].TableName);
                this.CompareDataTable(set.Tables[i], set2.Tables[i], str);
                if (this.Differences.Count >= this.MaxDifferences)
                {
                    return;
                }
            }
        }

        private void CompareDataTable(object object1, object object2, string breadCrumb)
        {
            DataTable table = object1 as DataTable;
            DataTable table2 = object2 as DataTable;
            if (table == null)
            {
                throw new ArgumentNullException("object1");
            }
            if (table2 == null)
            {
                throw new ArgumentNullException("object2");
            }
            if (!this.ElementsToIgnore.Contains(table.TableName))
            {
                if (table.Rows.Count != table2.Rows.Count)
                {
                    this.Differences.Add(object1, object2, 0, breadCrumb, string.Format("object1{0}.Rows.Count != object2{0}.Rows.Count ({1},{2})", breadCrumb, table.Rows.Count, table2.Rows.Count));
                    if (this.Differences.Count >= this.MaxDifferences)
                    {
                        return;
                    }
                }
                if (table.Columns.Count != table2.Columns.Count)
                {
                    this.Differences.Add(object1, object2, 0, breadCrumb, string.Format("object1{0}.Columns.Count != object2{0}.Columns.Count ({1},{2})", breadCrumb, table.Columns.Count, table2.Columns.Count));
                    if (this.Differences.Count >= this.MaxDifferences)
                    {
                        return;
                    }
                }
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string str = this.AddBreadCrumb(breadCrumb, "Rows", string.Empty, i);
                    this.CompareDataRow(table.Rows[i], table2.Rows[i], str);
                    if (this.Differences.Count >= this.MaxDifferences)
                    {
                        return;
                    }
                }
            }
        }

        private void CompareEnum(object object1, object object2, string breadCrumb)
        {
            if (object1.ToString() != object2.ToString())
            {
                string str = this.AddBreadCrumb(breadCrumb, object1.GetType().Name, string.Empty, -1);
                this.Differences.Add(object1, object2, 0, breadCrumb, string.Format("object1{0} != object2{0} ({1},{2})", str, object1, object2));
            }
        }

        private void CompareIDictionary(object object1, object object2, string breadCrumb)
        {
            IDictionary dictionary = object1 as IDictionary;
            IDictionary dictionary2 = object2 as IDictionary;
            if (dictionary == null)
            {
                throw new ArgumentNullException("object1");
            }
            if (dictionary2 == null)
            {
                throw new ArgumentNullException("object2");
            }
            try
            {
                this._parents.Add(object1);
                this._parents.Add(object2);
                if (dictionary.Count != dictionary2.Count)
                {
                    this.Differences.Add(object1, object2, 0, breadCrumb, string.Format("object1{0}.Count != object2{0}.Count ({1},{2})", breadCrumb, dictionary.Count, dictionary2.Count));
                    if (this.Differences.Count >= this.MaxDifferences)
                    {
                        return;
                    }
                }
                IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
                IDictionaryEnumerator enumerator2 = dictionary2.GetEnumerator();
                while (enumerator.MoveNext() && enumerator2.MoveNext())
                {
                    string str = this.AddBreadCrumb(breadCrumb, "Key", string.Empty, -1);
                    this.Compare(enumerator.Key, enumerator2.Key, str);
                    if (this.Differences.Count >= this.MaxDifferences)
                    {
                        return;
                    }
                    str = this.AddBreadCrumb(breadCrumb, "Value", string.Empty, -1);
                    this.Compare(enumerator.Value, enumerator2.Value, str);
                    if (this.Differences.Count >= this.MaxDifferences)
                    {
                        return;
                    }
                }
            }
            finally
            {
                this._parents.Remove(object1);
                this._parents.Remove(object2);
            }
        }

        private void CompareIList(object object1, object object2, string breadCrumb)
        {
            IList list = object1 as IList;
            IList list2 = object2 as IList;
            if (list == null)
            {
                throw new ArgumentNullException("object1");
            }
            if (list2 == null)
            {
                throw new ArgumentNullException("object2");
            }
            try
            {
                this._parents.Add(object1);
                this._parents.Add(object2);
                if (list.Count != list2.Count)
                {
                    this.Differences.Add(object1, object2, 0, breadCrumb, string.Format("object1{0}.Count != object2{0}.Count ({1},{2})", breadCrumb, list.Count, list2.Count));
                    if (this.Differences.Count >= this.MaxDifferences)
                    {
                        return;
                    }
                }
                IEnumerator enumerator = list.GetEnumerator();
                IEnumerator enumerator2 = list2.GetEnumerator();
                for (int i = 0; enumerator.MoveNext() && enumerator2.MoveNext(); i++)
                {
                    string str = this.AddBreadCrumb(breadCrumb, string.Empty, string.Empty, i);
                    this.Compare(enumerator.Current, enumerator2.Current, str);
                    if (this.Differences.Count >= this.MaxDifferences)
                    {
                        return;
                    }
                }
            }
            finally
            {
                this._parents.Remove(object1);
                this._parents.Remove(object2);
            }
        }

        private void CompareIndexer(PropertyInfo info, object object1, object object2, string breadCrumb)
        {
            string str;
            int num = (int) info.ReflectedType.GetProperty("Count").GetGetMethod().Invoke(object1, new object[0]);
            int num2 = (int) info.ReflectedType.GetProperty("Count").GetGetMethod().Invoke(object2, new object[0]);
            if (num != num2)
            {
                str = this.AddBreadCrumb(breadCrumb, info.Name, string.Empty, -1);
                this.Differences.Add(object1, object2, 0, str, string.Format("object1{0}.Count != object2{0}.Count ({1},{2})", str, num, num2));
                if (this.Differences.Count >= this.MaxDifferences)
                {
                    return;
                }
            }
            for (int i = 0; i < num; i++)
            {
                str = this.AddBreadCrumb(breadCrumb, info.Name, string.Empty, i);
                object obj2 = info.GetValue(object1, new object[] { i });
                object obj3 = info.GetValue(object2, new object[] { i });
                this.Compare(obj2, obj3, str);
                if (this.Differences.Count >= this.MaxDifferences)
                {
                    return;
                }
            }
        }

        private void CompareSimpleType(object object1, object object2, string breadCrumb)
        {
            if (object2 == null)
            {
                throw new ArgumentNullException("object2");
            }
            IComparable comparable = object1 as IComparable;
            if (comparable == null)
            {
                throw new ArgumentNullException("object1");
            }
            int result = comparable.CompareTo(object2);
            if (result != 0)
            {
                this.Differences.Add(object1, object2, result, breadCrumb, string.Format("object1{0} != object2{0} ({1},{2})", breadCrumb, object1, object2));
            }
        }

        private void CompareStruct(object object1, object object2, string breadCrumb)
        {
            foreach (FieldInfo info in object1.GetType().GetFields())
            {
                if (TypeHelper.ValidStructSubType(info.FieldType))
                {
                    string str = this.AddBreadCrumb(breadCrumb, info.Name, string.Empty, -1);
                    this.Compare(info.GetValue(object1), info.GetValue(object2), str);
                    if (this.Differences.Count >= this.MaxDifferences)
                    {
                        break;
                    }
                }
            }
        }

        private void CompareTimespan(object object1, object object2, string breadCrumb)
        {
            TimeSpan span = (TimeSpan) object1;
            TimeSpan span2 = (TimeSpan) object2;
            if (span.Ticks != span2.Ticks)
            {
                this.Differences.Add(object1, object2, 0, breadCrumb, string.Format("object1{0}.Ticks != object2{0}.Ticks", breadCrumb));
            }
        }

        private string cStr(object obj)
        {
            try
            {
                if (obj == null)
                {
                    return "(null)";
                }
                if (obj == DBNull.Value)
                {
                    return "System.DBNull.Value";
                }
                return obj.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        private bool IsValidIndexer(PropertyInfo info, object object1, object object2, string breadCrumb)
        {
            ParameterInfo[] indexParameters = info.GetIndexParameters();
            if (indexParameters.Length == 0)
            {
                return false;
            }
            if (indexParameters.Length > 1)
            {
                throw new Exception("Cannot compare objects with more than one indexer for object " + breadCrumb);
            }
            if (indexParameters[0].ParameterType != typeof(int))
            {
                throw new Exception("Cannot compare objects with a non integer indexer for object " + breadCrumb);
            }
            if (info.ReflectedType.GetProperty("Count") == null)
            {
                throw new Exception("Indexer must have a corresponding Count property for object " + breadCrumb);
            }
            if (info.ReflectedType.GetProperty("Count").PropertyType != typeof(int))
            {
                throw new Exception("Indexer must have a corresponding Count property that is an integer for object " + breadCrumb);
            }
            return true;
        }

        private void PerformCompareFields(Type t1, object object1, object object2, string breadCrumb)
        {
            FieldInfo[] fields;
            if (this.ComparePrivateFields)
            {
                fields = t1.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            }
            else
            {
                fields = t1.GetFields();
            }
            foreach (FieldInfo info in fields)
            {
                if ((this.CompareChildren || !TypeHelper.IsChildType(info.FieldType)) && !this.ElementsToIgnore.Contains(info.Name))
                {
                    object item = info.GetValue(object1);
                    object obj3 = info.GetValue(object2);
                    bool flag = (item != null) && ((item == object1) || this._parents.Contains(item));
                    bool flag2 = (obj3 != null) && ((obj3 == object2) || this._parents.Contains(obj3));
                    if (!TypeHelper.IsClass(info.FieldType) || (!flag && !flag2))
                    {
                        string str = this.AddBreadCrumb(breadCrumb, info.Name, string.Empty, -1);
                        this.Compare(item, obj3, str);
                        if (this.Differences.Count >= this.MaxDifferences)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void PerformCompareProperties(Type t1, object object1, object object2, string breadCrumb)
        {
            PropertyInfo[] properties;
            if (this.ComparePrivateProperties)
            {
                properties = t1.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            }
            else
            {
                properties = t1.GetProperties();
            }
            foreach (PropertyInfo info in properties)
            {
                object obj2;
                object obj3;
                bool flag;
                if ((info.CanRead && (this.CompareChildren || !TypeHelper.IsChildType(info.PropertyType))) && (!this.ElementsToIgnore.Contains(info.Name) && (this.CompareReadOnly || info.CanWrite)))
                {
                    if (!this.IsValidIndexer(info, object1, object2, breadCrumb))
                    {
                        obj2 = info.GetValue(object1, null);
                        obj3 = info.GetValue(object2, null);
                        goto Label_00B1;
                    }
                    this.CompareIndexer(info, object1, object2, breadCrumb);
                }
                continue;
            Label_00B1:
                flag = (obj2 != null) && ((obj2 == object1) || this._parents.Contains(obj2));
                bool flag2 = (obj3 != null) && ((obj3 == object2) || this._parents.Contains(obj3));
                if ((!TypeHelper.IsClass(info.PropertyType) || !flag) || !flag2)
                {
                    string str = this.AddBreadCrumb(breadCrumb, info.Name, string.Empty, -1);
                    this.Compare(obj2, obj3, str);
                    if (this.Differences.Count >= this.MaxDifferences)
                    {
                        break;
                    }
                }
            }
        }

        private bool CompareChildren
        {
            get
            {
                return ((this.compareOptions & ObjectCompareOptions.Children) > ObjectCompareOptions.PrivateProperties);
            }
        }

        private bool CompareFields
        {
            get
            {
                return ((this.compareOptions & ObjectCompareOptions.Fields) > ObjectCompareOptions.PrivateProperties);
            }
        }

        private bool ComparePrivateFields
        {
            get
            {
                return ((this.compareOptions & ObjectCompareOptions.PrivateFields) > ObjectCompareOptions.PrivateProperties);
            }
        }

        private bool ComparePrivateProperties
        {
            get
            {
                return (0 > 0);
            }
        }

        private bool CompareProperties
        {
            get
            {
                return ((this.compareOptions & ObjectCompareOptions.Properties) > ObjectCompareOptions.PrivateProperties);
            }
        }

        private bool CompareReadOnly
        {
            get
            {
                return ((this.compareOptions & ObjectCompareOptions.ReadOnlyProperties) > ObjectCompareOptions.PrivateProperties);
            }
        }

        public ObjectCompareResultCollection Differences
        {
            get
            {
                return this._differences;
            }
            set
            {
                this._differences = value;
            }
        }

        public string DifferencesString
        {
            get
            {
                StringBuilder builder = new StringBuilder(0x1000);
                builder.Append("\r\nBegin Differences:\r\n");
                foreach (string str in this.Differences)
                {
                    builder.AppendFormat("{0}\r\n", str);
                }
                builder.AppendFormat("End Differences (Maximum of {0} differences shown).", this.MaxDifferences);
                return builder.ToString();
            }
        }

        public List<string> ElementsToIgnore
        {
            get
            {
                return this._elementsToIgnore;
            }
            set
            {
                this._elementsToIgnore = value;
            }
        }

        public int MaxDifferences
        {
            get
            {
                return this._maxDifferences;
            }
            set
            {
                this._maxDifferences = value;
            }
        }
    }
}

