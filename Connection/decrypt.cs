using System;
using System.IO;
using Server.Game;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace Server
{
    class decrypt
    {
        public static void EncryptFile(string inputFile, string outputFile)
        {
            //FileStream File = new FileStream(Program.ConquerPath + @"Server.dat", FileMode.Open);
            // FileStream Output = new FileStream(Program.ConquerPath + @"Server1.dat", FileMode.Create);
            try
            {
                string password = @"myKey123"; // Your Key Here
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch
            {
                MessageBox.Show("Encryption failed!", "Error");
            }
        }
        private void DecryptFile(string inputFile, string outputFile)
        {

            {
                string password = @"myKey123"; // Your Key Here

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateDecryptor(key, key),
                    CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();

            }
        }
        public static void dec()
        {
            FileStream File = new FileStream(Program.ConquerPath + @"mag1.dat", FileMode.Open);
            FileStream Output = new FileStream(Program.ConquerPath + @"mag122222.dat", FileMode.Create);
            StreamWriter Writer = new StreamWriter(Output);
            BinaryReader Reader = new BinaryReader(File);

            bool finshed = false;
            while (!finshed)
            {
                try
                {
                    Writer.WriteLine("{0}", Math.Abs(Reader.ReadInt32()));
                }
                catch { finshed = true; }
            }
            Console.WriteLine("Complete!");
        }
    }
}
