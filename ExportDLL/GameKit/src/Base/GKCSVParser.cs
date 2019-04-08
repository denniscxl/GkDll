using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
Example:

var csv = GKCSVParser.OpenFile( "test.csv", true ); 
 
while( csv.NextRow() ) {
	//get cell by index
	for( int i=0; i<csv.CellCount; i++ ) {
		Tk.Log( csv[i] + "," );
	}
	
	//get cell by column name
	Tk.Log( csv["FirstName"] );
	
	Tk.Log("=====");
}
 
*/

namespace GKBase
{
    //! base on RFC 4180 Standard  http://tools.ietf.org/html/rfc4180
    public class GKCSVParser
    {

        public class NonSerializeAttribute : System.Attribute { }
        public bool verbose = true;
        public int CellCount { get { return _row.Count; } }
        public Cell this[int i] { get { return GetCell(i); } }
        public Cell this[string name] { get { return GetCell(name); } }

        string _filename = "";
        string _sourceCsv;
        int _readIndex;
        //use for error msg
        int _lineNo = 1;
        int _charNo = 1;
        int _rowNo = 0;
        int _cellNo = 0;
        List<Cell> _row = new List<Cell>();
        List<Cell> _colName_row = new List<Cell>();
        Dictionary<string, int> _columnIndexByName = new Dictionary<string, int>(64);

        public Cell GetCell(int i)
        {
            if (i < 0 || i >= _row.Count)
            {
                Debug.LogError("GetCell faile. i:" + i + "| row.Count:" + _row.Count);
                Debug.LogError("CSV and data structure does not match.");
                return null;
            }
            return _row[i];
        }

        public Cell GetCell(string name)
        {
            var c = GetCell(ColIndexByName(name));
            if (c == null)
            {
                Debug.LogError(LineStr + "Cannot find column '" + name + "'");
            }
            return c;
        }

        public void ResetReadIndex()
        {
            _readIndex = 3;
        }

        public bool GetValueByName(string name, out bool val, bool defaultForEmptyCell)
        {
            var c = GetCell(name); val = defaultForEmptyCell;
            return c == null ? false : c.GetValue(out val, defaultForEmptyCell);
        }

        public bool GetValueByName(string name, out float val, float defaultForEmptyCell)
        {
            var c = GetCell(name); val = defaultForEmptyCell;
            return c == null ? false : c.GetValue(out val, defaultForEmptyCell);
        }

        public bool GetValueByName(string name, out double val, double defaultForEmptyCell)
        {
            var c = GetCell(name); val = defaultForEmptyCell;
            return c == null ? false : c.GetValue(out val, defaultForEmptyCell);
        }

        public bool GetValueByName(string name, out int val, int defaultForEmptyCell)
        {
            var c = GetCell(name); val = defaultForEmptyCell;
            return c == null ? false : c.GetValue(out val, defaultForEmptyCell);
        }

        public bool GetValueByName(string name, out long val, long defaultForEmptyCell)
        {
            var c = GetCell(name); val = defaultForEmptyCell;
            return c == null ? false : c.GetValue(out val, defaultForEmptyCell);
        }

        public string ColNameByIndex(int i)
        {
            if (i < 0 || i >= _colName_row.Count) return string.Empty;
            return _colName_row[i].asString;
        }

        public int ColNameCount { get { return _colName_row.Count; } }

        public int ColIndexByName(string name)
        {
            int index;
            if (!_columnIndexByName.TryGetValue(name, out index))
            {
                return -1;
            }
            return index;
        }

        public List<Cell> GetRow { get { return _row; } }

        public bool RowToObject<T>(ref T obj)
        {
            var type = typeof(T);
            var fields = type.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                var f = fields[i];

                var attrs = f.GetCustomAttributes(typeof(NonSerializeAttribute), true);
                if (attrs.Length > 0)
                {
                    continue;
                }

                var cellIndex = ColIndexByName(f.Name);
                _cellNo = cellIndex;

                var cell = GetCell(cellIndex);
                if (cell == null)
                {
                    return false;
                }

                var val = cell.GetValueByType(f.FieldType);
                if (val == null)
                {
                    Debug.LogError(LineStr + "Error Get Value for Field: " + type.Name + "." + f.Name);
                    return false;
                }
                f.SetValue(obj, val);
            }
            return true;
        }

        public class Cell
        {
            GKCSVParser p_ = null;
            public Cell(GKCSVParser p)
            {
                p_ = p;
            }

            public string asString = string.Empty;
            public bool asBool { get { bool val = false; GetValue(out val); return val; } }
            public int asInt { get { int val = 0; GetValue(out val); return val; } }
            public long asLong { get { long val = 0; GetValue(out val); return val; } }
            public float asFloat { get { float val = 0; GetValue(out val); return val; } }
            public double asDouble { get { double val = 0; GetValue(out val); return val; } }

            //--------
            public bool GetValue(out int val, int defaultForEmptyCell)
            {
                if (string.IsNullOrEmpty(asString)) { val = defaultForEmptyCell; return true; }
                return GetValue(out val);
            }

            public bool GetValue(out long val, long defaultForEmptyCell)
            {
                if (string.IsNullOrEmpty(asString)) { val = defaultForEmptyCell; return true; }
                return GetValue(out val);
            }

            public bool GetValue(out float val, float defaultForEmptyCell)
            {
                if (string.IsNullOrEmpty(asString)) { val = defaultForEmptyCell; return true; }
                return GetValue(out val);
            }

            public bool GetValue(out double val, double defaultForEmptyCell)
            {
                if (string.IsNullOrEmpty(asString)) { val = defaultForEmptyCell; return true; }
                return GetValue(out val);
            }

            public bool GetValue(out bool val, bool defaultForEmptyCell)
            {
                if (string.IsNullOrEmpty(asString)) { val = defaultForEmptyCell; return true; }
                return GetValue(out val);
            }

            public bool GetValue(out int val)
            {
                val = 0;
                if (string.IsNullOrEmpty(asString)) return true;
                if (!int.TryParse(asString, out val))
                {
                    Debug.LogError(p_.LineStr + "Error Get int Value from [" + asString + "]");
                    return false;
                }
                return true;
            }

            public bool GetValue(out long val)
            {
                val = 0;
                if (string.IsNullOrEmpty(asString)) return true;
                if (!long.TryParse(asString, out val))
                {
                    Debug.LogError(p_.LineStr + "Error Get long Value from [" + asString + "]");
                    return false;
                }
                return true;
            }

            public bool GetValue(out float val)
            {
                val = 0;
                if (string.IsNullOrEmpty(asString)) return true;
                if (!float.TryParse(asString, out val))
                {
                    Debug.LogError(p_.LineStr + "Error Get float Value from [" + asString + "]");
                    return false;
                }
                return true;
            }

            public bool GetValue(out double val)
            {
                val = 0;
                if (string.IsNullOrEmpty(asString)) return true;
                if (!double.TryParse(asString, out val))
                {
                    Debug.LogError(p_.LineStr + "Error Get double Value from [" + asString + "]");
                    return false;
                }
                return true;
            }

            public bool GetValue(out bool val)
            {
                val = false;
                if (string.IsNullOrEmpty(asString)) return true;
                if (!bool.TryParse(asString, out val))
                {
                    Debug.LogError(p_.LineStr + "Error Get bool Value from [" + asString + "]");
                    return false;
                }
                return true;
            }

            public bool GetValue(out Color val)
            {
                float[] fCustonColor = { 0, 0, 0 };
                string[] strList = null;
                string tag = asString.Trim();

                val = Color.white;
                if (string.IsNullOrEmpty(asString)) return true;

                // Custom.
                if (asString[0].Equals('('))
                {
                    tag = "Custom";
                    strList = asString.Substring(1, asString.Length - 2).Split(',');
                    if (strList.Length >= 3)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            float.TryParse(strList[i], out fCustonColor[i]);
                        }
                    }
                }

                switch (tag)
                {
                    case "Black": { val = Color.black; } break;
                    case "Blue": { val = Color.blue; } break;
                    case "Clear": { val = Color.clear; } break;
                    case "Cyan": { val = Color.cyan; } break;
                    case "Gray": { val = Color.gray; } break;
                    case "Green": { val = Color.green; } break;
                    case "Grey": { val = Color.grey; } break;
                    case "Red": { val = Color.red; } break;
                    case "White": { val = Color.white; } break;
                    case "Yellow": { val = Color.yellow; } break;
                    case "Custom": { val = new Color(fCustonColor[0], fCustonColor[1], fCustonColor[2]); } break;
                }
                return true;
            }

            public bool GetValue(out List<int> val)
            {
                //float[] fCustonColor = { 0, 0, 0 };
                //string[] strList = null;
                //string tag = asString.Trim();

                val = new List<int>();
                if (string.IsNullOrEmpty(asString)) return true;

                var arrays = asString.Split('|');
                int result = 0;
                foreach (var str in arrays)
                {
                    if (string.IsNullOrEmpty(str))
                        continue;

                    int.TryParse(str, out result);
                    val.Add(result);
                }
                return true;
            }

            public object GetValueByType(System.Type type)
            {
                if (type == typeof(string)) return asString;
                if (type == typeof(bool)) { bool val = false; return GetValue(out val) ? (object)val : null; }
                if (type == typeof(int)) { int val = 0; return GetValue(out val) ? (object)val : null; }
                if (type == typeof(long)) { long val = 0; return GetValue(out val) ? (object)val : null; }
                if (type == typeof(float)) { float val = 0; return GetValue(out val) ? (object)val : null; }
                if (type == typeof(double)) { double val = 0; return GetValue(out val) ? (object)val : null; }
                if (type == typeof(List<int>)) { List<int> val = null; return GetValue(out val) ? (object)val : null; }
                if (type == typeof(Color)) { Color val = Color.white; return GetValue(out val) ? (object)val : null; }

                if (type.IsEnum)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(asString)) return (object)0;
                        return System.Enum.Parse(type, asString, true);
                    }
                    catch
                    {
                        Debug.LogError(p_.LineStr + "Unknown value " + asString + " for enum " + type.Name);
                        return null;
                    }
                }

                Debug.LogError(p_.LineStr + "Unsupported cell type " + type.Name);
                return null;
            }

            public override string ToString()
            {
                return asString;
            }
        }

        static public GKCSVParser OpenFile(string filename, string columnStartMark, string filenameForDebug = "")
        {
            var p = new GKCSVParser(filename);
            p._filename = filenameForDebug;

            if (!string.IsNullOrEmpty(columnStartMark))
            {
                while (p.NextRow())
                {
                    var n = p.CellCount;
                    if (n <= 0)
                        continue;
                    if (p.GetCell(0).asString != columnStartMark)
                        continue;

                    for (int i = 1; i < n; i++)
                    {
                        var colName = p.GetCell(i).asString.Trim();
                        if (string.IsNullOrEmpty(colName))
                            continue;

                        if (!p._columnIndexByName.ContainsKey(colName))
                        {
                            p._columnIndexByName.Add(colName, i);
                        }
                        else
                        {
                            Debug.LogError(string.Format("Add columnIndexByName Key: {0}, Value: {1}", colName, i));
                        }
                    }

                    return p;
                }

                Debug.LogError("Cannot find columnStartMask " + filenameForDebug);
                return null;
            }

            return p;
        }

        GKCSVParser(string fileName)
        {
             _sourceCsv = System.IO.File.ReadAllText(fileName, GK.GetEncoding(fileName, out _readIndex));
            if (_sourceCsv.Contains("\r\n"))
                _sourceCsv = _sourceCsv.Replace("\r\n", "\n");
            _sourceCsv = _sourceCsv.TrimEnd('\n');
        }

        public bool IsRowStartWith(string s)
        {
            if (CellCount <= 0) return false;
            return GetCell(0).asString.StartsWith(s);
        }

        public bool NextRow()
        {
            char ch;
            if (!_ReadChar(out ch))
                return false; //End of file

            _row = new List<Cell>();
            _rowNo++;
            _cellNo = 0;

            bool inQuote = false;

            Cell cell = null;

            for (; ; )
            {
                if (cell == null)
                {
                    cell = new Cell(this); _cellNo++;
                    inQuote = (ch == '\"');
                    if (inQuote)
                    {
                        if (!_ReadChar(out ch))
                        {
                            Debug.LogError(LineStr + "Unexpected End Of File");
                            return false; //End of file
                        }
                        continue;
                    }
                }

                if (inQuote)
                {
                    if (ch == '\"')
                    {
                        if (!_ReadChar(out ch)) break;
                        if (ch == '\n') break; // end of row

                        switch (ch)
                        {
                            case ',':
                                { // new cell
                                    _row.Add(cell);
                                    cell = null;
                                }
                                break;

                            case '\"':
                                {
                                    cell.asString += ch;
                                    break;
                                }

                            default:
                                {
                                    Debug.LogError(LineStr + "Unexpected character " + ch + " (" + (int)ch + ") after \"");
                                    return false;
                                }
                        }
                    }
                    else
                    {
                        cell.asString += ch;
                    }
                }
                else
                {
                    if (ch == '\n' || ch == '\r')
                        break; // end of row				
                    switch (ch)
                    {
                        case ',':
                            { // new cell
                                _row.Add(cell);
                                cell = null;
                            }
                            break;

                        default:
                            {
                                cell.asString += ch;
                            }
                            break;
                    }
                }

                if (!_ReadChar(out ch))
                    break;
            }
            _row.Add(cell);

            return true;
        }

        public string RowAsString()
        {
            var o = "";
            foreach (var c in _row)
            {
                o += "[" + c.ToString() + "]\n";
            }
            return o;
        }

        public string LineStr
        {
            get { return _filename + ": CSV Line (" + _lineNo + "," + _charNo + ") Row=" + _rowNo + ", Col=" + _cellNo + " ColName=[" + ColNameByIndex(_cellNo - 1) + "]:\n"; }
        }

        bool _ReadChar(out char ch)
        {
            for (; ; )
            {
                if (_readIndex >= _sourceCsv.Length)
                {
                    ch = '\0';
                    return false;
                }
                ch = _sourceCsv[_readIndex];
                _readIndex++;

                //if( ch == '\r' ) {
                //	_charNo++;
                //	continue;
                //}

                if (ch == '\n' || ch == '\r')
                {
                    _lineNo++;
                    _charNo = 0;
                }
                _charNo++;
                return true;
            }
        }


    }
}