using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MC_SVGeneralDatCleaner
{
    internal class Control
    {
        private const string EXE = "\\Star Valor.exe";
        private const string GENERALDAT = "\\Star Valor_Data\\general.dat";
        private const string ASSEMBLY = "\\Star Valor_Data\\Managed\\Assembly-CSharp.dll";

        internal List<int> ShipList { get { return shipList; } }

        private static string path = "";
        private List<int> shipList = new();
        private object? generalData;

        internal bool SetPath(string path)
        {
            if (!File.Exists(path + EXE))
                return false;

            Control.path = path;
            return true;
        }

        internal bool DeserialiseGeneraDat()
        {
            FileStream fileStream = null;
            try
            {
                BinaryFormatter binaryFormatter = new();
                fileStream = File.Open(path + GENERALDAT, FileMode.Open);
                binaryFormatter.Binder = new GeneralDataBinder();
                generalData = binaryFormatter.Deserialize(fileStream);

                Assembly starValorAss = Assembly.LoadFrom(path + ASSEMBLY);

                Type? generaldataType = starValorAss.GetType("GeneralData");
                if (generaldataType == null)
                    return false;

                FieldInfo? generalDatShips = generaldataType.GetField("spaceships");
                if (generalDatShips == null)
                    return false;

                object? list = generalDatShips.GetValue(generalData);
                if (list == null)
                    return false;

                this.shipList = (List<int>)list;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
            return true;
        }

        internal bool SerialiseGeneralDat(string savePath)
        {
            FileStream fileStream = null;
            try
            {
                Assembly starValorAss = Assembly.LoadFrom(path + ASSEMBLY);

                Type? generaldataType = starValorAss.GetType("GeneralData");
                if (generaldataType == null)
                    return false;

                FieldInfo? generalDatShips = generaldataType.GetField("spaceships");
                if (generalDatShips == null)
                    return false;

                generalDatShips.SetValue(generalData, shipList);

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                fileStream = File.Create(savePath);
                binaryFormatter.Serialize(fileStream, generalData);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
            return true;
        }

        internal bool RemoveShip(int id)
        {
            if (shipList == null)
                return false;

            if (!shipList.Contains(id))
                return false;

            shipList.Remove(id);
            return true;
        }

        private class GeneralDataBinder : SerializationBinder
        {
            public override Type? BindToType(string assemblyName, string typeName)
            {
                if (typeName.Equals("GeneralData"))
                {
                    Assembly starValorAss = Assembly.LoadFrom(path + ASSEMBLY);

                    Type? generaldataType = starValorAss.GetType("GeneralData");
                    if (generaldataType == null)
                        return null;

                    return generaldataType;
                }
                else
                {
                    return typeof(List<int>);
                }
            }
        }
    }
}
