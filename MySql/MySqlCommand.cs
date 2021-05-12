using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace Server
{
    public enum MySqlCommandType
    {
        DELETE, INSERT, SELECT, UPDATE, COUNT
    }

    public class MySqlCommand
    {
        private MySqlCommandType _type;
        public MySqlCommandType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        protected string _command;
        public string Command
        {
            get { return _command; }
            set { _command = value; }
        }

        private bool firstPart = true;
        private Dictionary<byte, string> Fields;
        private Dictionary<byte, long> longValues;
        private Dictionary<byte, ulong> ulongValues;
        private Dictionary<byte, bool> boolValues;
        private Dictionary<byte, string> stringValues;
        private byte lastpair;

        public MySqlCommand(MySqlCommandType Type)
        {
            this.Type = Type;
            switch (Type)
            {
                case MySqlCommandType.SELECT:
                    {
                        _command = "SELECT * FROM <R>";
                        break;
                    }
                case MySqlCommandType.UPDATE:
                    {
                        _command = "UPDATE <R> SET ";
                        break;
                    }
                case MySqlCommandType.INSERT:
                    {
                        Fields = new Dictionary<byte, string>();
                        longValues = new Dictionary<byte, long>();
                        ulongValues = new Dictionary<byte, ulong>();
                        boolValues = new Dictionary<byte, bool>();
                        stringValues = new Dictionary<byte, string>();
                        lastpair = 0;
                        _command = "INSERT INTO <R> (<F>) VALUES (<V>)";
                        break;
                    }
                case MySqlCommandType.DELETE:
                    {
                        _command = "DELETE FROM <R> WHERE <C> = <V>";
                        break;
                    }
                case MySqlCommandType.COUNT:
                    {
                        _command = "SELECT count(<V>) FROM <R>";
                        break;
                    }
            }
        }

        private bool Comma()
        {
            if (firstPart)
            {
                firstPart = false;
                return false;
            }
            if (_command[_command.Length - 1] == ',' || _command[_command.Length - 2] == ',' || _command[_command.Length - 3] == ',')
                return false;
            return true;
        }

        #region Select
        public MySqlCommand Select(string table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }
        #endregion

        #region Count
        public MySqlCommand Count(string table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }
        #endregion

        #region Delete
        public MySqlCommand Delete(string table, string column, string value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", "'" + value + "'");
            return this;
        }
        public MySqlCommand Delete(string table, string column, long value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", value.ToString());
            return this;
        }
        public MySqlCommand Delete(string table, string column, ulong value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", value.ToString());
            return this;
        }
        public MySqlCommand Delete(string table, string column, bool value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", (value ? "1" : "0"));
            return this;
        }
        public MySqlCommand COUNT(string table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }
        #endregion

        #region Update
        public MySqlCommand Update(string table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }
        public MySqlCommand Set(string column, long value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                    _command = _command + ",`" + column + "` = " + value.ToString() + " ";
                else
                    _command = _command + "`" + column + "` = " + value.ToString() + " ";
            }
            return this;
        }
        public MySqlCommand Set(string column, ulong value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                bool comma = false;
                comma = (_command[_command.Length - 1] == ',' || _command[_command.Length - 2] == ',' || _command[_command.Length - 3] == ',') ? false : true;
                if (comma)
                    _command = _command + ",`" + column + "` = " + value.ToString() + " ";
                else
                    _command = _command + "`" + column + "` = " + value.ToString() + " ";
            }
            return this;
        }
        public MySqlCommand Set(string column, string value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                    _command = _command + ",`" + column + "` = '" + value + "' ";
                else
                    _command = _command + "`" + column + "` = '" + value + "' ";
            }
            return this;
        }
        public MySqlCommand Set(string column, bool value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                    _command = _command + ",`" + column + "` = " + (value ? "1" : "0") + " ";
                else
                    _command = _command + "`" + column + "` = " + (value ? "1" : "0") + " ";
            }
            return this;
        }
        #endregion

        #region Insert
        public MySqlCommand Insert(string table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }
        public MySqlCommand Insert(string field, long value)
        {
            Fields.Add(lastpair, field);
            longValues.Add(lastpair, value);
            lastpair++;
            return this;
        }
        public MySqlCommand Insert(string field, ulong value)
        {
            Fields.Add(lastpair, field);
            ulongValues.Add(lastpair, value);
            lastpair++;
            return this;
        }
        public MySqlCommand Insert(string field, bool value)
        {
            Fields.Add(lastpair, field);
            boolValues.Add(lastpair, value);
            lastpair++;
            return this;
        }
        public MySqlCommand Insert(string field, string value)
        {
            Fields.Add(lastpair, field);
            stringValues.Add(lastpair, value);
            lastpair++;
            return this;
        }
        public void UpdateInsert()
        {
            if (Type == MySqlCommandType.INSERT)
            {
                string fields = "";
                string values = "";
                byte x;
                for (x = 0; x < lastpair; x++)
                {
                    bool comma = (x + 1) == lastpair ? false : true;
                    #region Fields
                    if (comma)
                        fields += Fields[x] + ",";
                    else
                        fields += Fields[x];
                    #endregion
                    #region Values
                    if (longValues.ContainsKey(x))
                    {
                        if (comma)
                            values += longValues[x].ToString() + ",";
                        else
                            values += longValues[x].ToString();
                    }
                    else if (ulongValues.ContainsKey(x))
                    {
                        if (comma)
                            values += ulongValues[x].ToString()[x] + ",";
                        else
                            values += ulongValues[x].ToString();
                    }
                    else if (boolValues.ContainsKey(x))
                    {
                        if (comma)
                            values += (boolValues[x] ? "1" : "0") + ",";
                        else
                            values += (boolValues[x] ? "1" : "0");
                    }
                    else if (stringValues.ContainsKey(x))
                    {
                        if (comma)
                            values += "'" + stringValues[x] + "'" + ",";
                        else
                            values += "'" + stringValues[x] + "'";
                    }
                    #endregion
                }
                _command = _command.Replace("<F>", fields);
                _command = _command.Replace("<V>", values);
            }
        }
        #endregion

        #region Where
        public MySqlCommand Where(string column, long value)
        {
            _command = _command + "WHERE `" + column + "` = " + value;
            return this;
        }
        public MySqlCommand Where(string column, long value, bool greater)
        {
            if (greater)
                _command = _command + "WHERE `" + column + "` > " + value;
            else
                _command = _command + "WHERE `" + column + "` < " + value;
            return this;
        }
        public MySqlCommand Where(string column, ulong value)
        {
            _command = _command + "WHERE `" + column + "` = " + value;
            return this;
        }
        public MySqlCommand Where(string column, string value)
        {
            _command = _command + "WHERE `" + column + "` = '" + value + "'";
            return this;
        }
        public MySqlCommand Where(string column, bool value)
        {
            _command = _command + "WHERE `" + column + "` = " + (value ? "1" : "0");
            return this;
        }
        #endregion

        #region And
        public MySqlCommand And(string column, long value)
        {
            _command = _command + " AND `" + column + "` = " + value;
            return this;
        }
        public MySqlCommand And(string column, ulong value)
        {
            _command = _command + " AND `" + column + "` = " + value;
            return this;
        }
        public MySqlCommand And(string column, string value)
        {
            _command = _command + " AND `" + column + "` = '" + value + "'";
            return this;
        }
        public MySqlCommand And(string column, bool value)
        {
            _command = _command + " AND `" + column + "` = " + (value ? "1" : "0");
            return this;
        }
        #endregion

        #region Order
        public MySqlCommand Order(string column)
        {
            _command = _command + "ORDER BY " + column + "";
            return this;
        }
        #endregion

        public void Execute()
        {
            if (Type == MySqlCommandType.INSERT)
            {
                string fields = "";
                string values = "";
                byte x;
                for (x = 0; x < lastpair; x++)
                {
                    bool comma = (x + 1) == lastpair ? false : true;
                    #region Fields
                    if (comma)
                        fields += Fields[x] + ",";
                    else
                        fields += Fields[x];
                    #endregion
                    #region Values
                    if (longValues.ContainsKey(x))
                    {
                        if (comma)
                            values += longValues[x].ToString() + ",";
                        else
                            values += longValues[x].ToString();
                    }
                    else if (ulongValues.ContainsKey(x))
                    {
                        if (comma)
                            values += ulongValues[x].ToString()[x] + ",";
                        else
                            values += ulongValues[x].ToString();
                    }
                    else if (boolValues.ContainsKey(x))
                    {
                        if (comma)
                            values += (boolValues[x] ? "1" : "0") + ",";
                        else
                            values += (boolValues[x] ? "1" : "0");
                    }
                    else if (stringValues.ContainsKey(x))
                    {
                        if (comma)
                            values += "'" + stringValues[x] + "'" + ",";
                        else
                            values += "'" + stringValues[x] + "'";
                    }
                    #endregion
                }
                _command = _command.Replace("<F>", fields);
                _command = _command.Replace("<V>", values);
            }
            using (MySqlConnection conn = Database.MySqlConnection)
            {
                conn.Open();
                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(Command, conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
