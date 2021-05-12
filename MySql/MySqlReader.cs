using System;
using MySql.Data.MySqlClient;
using System.Data.OleDb;
using System.Data;

namespace Server
{
    public class MySqlReader
    {
        private DataSet _dataset;
        private DataRow _datarow;
        private int _row;
        private const string Table = "table";

        public MySqlReader(MySqlCommand command)
        {
            if (command.Type == MySqlCommandType.SELECT)
            {
                TryFill(command);
            }
        }
    
        private string _lasterror = null;

        private void TryFill(MySqlCommand command)
        {
            MySqlConnection connection = Database.MySqlConnection;
            MySqlDataAdapter DataAdapter;
            if (connection.State == ConnectionState.Open)
            {
                while (_dataset == null && (_lasterror == null || _lasterror.Contains("connection")))
                {
                    if (_lasterror != null && _lasterror.Contains("connection"))
                        connection = Database.MySqlConnection;
                    DataAdapter = new MySqlDataAdapter(command.Command, connection);
                    _dataset = new DataSet();
                    try
                    {
                        DataAdapter.Fill(_dataset, Table);
                    }
                    catch (MySqlException e)
                    {
                        _lasterror = e.ToString().ToLower();
                        _dataset = null;
                        continue;
                    }
                    catch (Exception e)
                    {
                        Program.WriteMessage(e.ToString());
                        break;
                    }
                    _row = 0;
                }
            }
            using (MySqlConnection conn = Database.MySqlConnection)
            {
                conn.Open();
                DataAdapter = new MySqlDataAdapter(command.Command, connection);
                _dataset = new DataSet();
                try
                {
                    DataAdapter.Fill(_dataset, Table);
                }
                catch (MySqlException e)
                {
                    _lasterror = e.ToString().ToLower();
                    _dataset = null;
                }
                catch (Exception e)
                {
                    Program.WriteMessage(e.ToString());
                }
                _row = 0;
            }
        }

        public bool Read()
        {
            if (_dataset.Tables[Table].Rows.Count > _row)
            {
                _datarow = _dataset.Tables[Table].Rows[_row];
                _row++;
                return true;
            }
            _row++;
            return false;
        }

        public sbyte ReadSByte(string columnName)
        {
            sbyte result = 0;
            try
            {
                sbyte.TryParse(_datarow[columnName].ToString(), out result);
            }
            catch { }
            return result;
        }
        public byte ReadByte(string columnName)
        {
            byte result = 0;
            try
            {
                byte.TryParse(_datarow[columnName].ToString(), out result);
            }
            catch { }
            return result;
        }
        public short ReadInt16(string columnName)
        {
            short result = 0;
            try
            {
                short.TryParse(_datarow[columnName].ToString(), out result);
            }
            catch { }
            return result;
        }
        public ushort ReadUInt16(string columnName)
        {
            ushort result = 0;
            try
            {
                ushort.TryParse(_datarow[columnName].ToString(), out result);
            }
            catch { }
            return result;
        }
        public int ReadInt32(string columnName)
        {
            int result = 0;
            try
            {
                int.TryParse(_datarow[columnName].ToString(), out result);
            }
            catch { }
            return result;
        }
        public uint ReadUInt32(string columnName)
        {
            uint result = 0;
            try
            {
                uint.TryParse(_datarow[columnName].ToString(), out result);
            }
            catch { }
            return result;
        }
        public long ReadInt64(string columnName)
        {
            long result = 0;
            try
            {
                long.TryParse(_datarow[columnName].ToString(), out result);
            }
            catch { }
            return result;
        }
        public ulong ReadUInt64(string columnName)
        {
            ulong result = 0;
            try
            {
                ulong.TryParse(_datarow[columnName].ToString(), out result);
            }
            catch { }
            return result;
        }
        public string ReadString(string columnName)
        {
            string result = "";
            try
            {
                result = _datarow[columnName].ToString();
            }
            catch { }
            return result;
        }
        public bool ReadBoolean(string columnName)
        {
            bool result = false;
            try
            {
                bool.TryParse(_datarow[columnName].ToString(), out result);
            }
            catch
            {
                byte value = 0;
                try
                {
                    byte.TryParse(_datarow[columnName].ToString(), out value);
                }
                catch { }
                result = value == 0 ? false : true;
            }
            return result;
        }
    }
}
