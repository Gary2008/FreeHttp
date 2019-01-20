﻿using FreeHttp.AutoTest.RunTimeStaticData.MyStaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FreeHttp.AutoTest.RunTimeStaticData
{
    /// <summary>
    /// ActuatorStaticData 集合
    /// </summary>
    [DataContract]
    [KnownType(typeof(MyStaticDataValue)), KnownType(typeof(MyStaticDataIndex)), KnownType(typeof(MyStaticDataList)), KnownType(typeof(MyStaticDataLong)), KnownType(typeof(MyStaticDataNowTime)), KnownType(typeof(MyStaticDataRandomStr)), KnownType(typeof(MyStaticDataSourceCsv)), KnownType(typeof(MyStaticDataStrIndex))]
    public class ActuatorStaticDataCollection : IDisposable, ICloneable
    {
        public class ChangeDataEventArgs : EventArgs
        {
            public bool IsAddOrDel { get; set; }
            public ChangeDataEventArgs(bool isAddOrDel)
            {
                IsAddOrDel = isAddOrDel;
            }
        }

        [DataMember]
        public bool IsAllCollectionKeyUnique { get; private set; }

        /// <summary>
        /// RunTimeParameter List
        /// </summary>
        [DataMember]
        private Dictionary<string, IRunTimeStaticData> runActuatorStaticDataKeyList;

        /// <summary>
        /// RunTimeStaticData List
        /// </summary>
        [DataMember]
        private Dictionary<string, IRunTimeStaticData> runActuatorStaticDataParameterList;

        /// <summary>
        /// RunTimeDataSouce List
        /// </summary>
        [DataMember]
        private Dictionary<string, IRunTimeDataSource> runActuatorStaticDataSouceList;


        private readonly object padlock = new object();

        public ActuatorStaticDataCollection()
        {
            runActuatorStaticDataKeyList = new Dictionary<string, IRunTimeStaticData>();
            runActuatorStaticDataParameterList = new Dictionary<string, IRunTimeStaticData>();
            runActuatorStaticDataSouceList = new Dictionary<string, IRunTimeDataSource>();
            IsAllCollectionKeyUnique = false;
        }

        public ActuatorStaticDataCollection(bool isAllCollectionKeyUnique):this()
        {
            IsAllCollectionKeyUnique = isAllCollectionKeyUnique;
        }

        public ActuatorStaticDataCollection(Dictionary<string, IRunTimeStaticData> yourActuatorParameterList, Dictionary<string, IRunTimeStaticData> yourActuatorStaticDataList, Dictionary<string, IRunTimeDataSource> yourActuatorStaticDataSouceList)
        {
            runActuatorStaticDataKeyList = yourActuatorParameterList;
            runActuatorStaticDataParameterList = yourActuatorStaticDataList;
            runActuatorStaticDataSouceList = yourActuatorStaticDataSouceList;
            IsAllCollectionKeyUnique = false;
        }

        //public event EventHandler OnChangeCollection;
        public delegate void ChangeCollectionEventHandler(object sender, ChangeDataEventArgs e);
        public event ChangeCollectionEventHandler OnChangeCollection;

        private object IsHasSameKey(string key, int ignoreListIndex)
        {
            if (runActuatorStaticDataKeyList.ContainsKey(key) && ignoreListIndex != 1)
            {
                return runActuatorStaticDataKeyList;
            }
            if (runActuatorStaticDataParameterList.ContainsKey(key) && ignoreListIndex != 2)
            {
                return runActuatorStaticDataParameterList;
            }
            if (runActuatorStaticDataSouceList.ContainsKey(key) && ignoreListIndex != 3)
            {
                return runActuatorStaticDataSouceList;
            }
            return null;
        }

        private void OnListChanged(bool isAddOrDel)
        {
            if(OnChangeCollection!=null)
            {
                this.OnChangeCollection(this, new ChangeDataEventArgs(isAddOrDel));
            }
        }

        public Dictionary<string, IRunTimeStaticData> RunActuatorStaticDataKeyList
        {
            get { return runActuatorStaticDataKeyList; }
        }

        public Dictionary<string, IRunTimeStaticData> RunActuatorStaticDataParameterList
        {
            get { return runActuatorStaticDataParameterList; }
        }

        public Dictionary<string, IRunTimeDataSource> RunActuatorStaticDataSouceList
        {
            get { return runActuatorStaticDataSouceList; }
        }

        /// <summary>
        /// Is the StaticDataCollection has th same key name 
        /// </summary>
        /// <param name="yourKey">your Key</param>
        /// <returns>is has </returns>
        public bool IsHaveSameKey(string yourKey)
        {
            return (IsHasSameKey(yourKey, 0) != null);
        }

        /// <summary>
        /// Add Data into runActuatorStaticDataKeyList (if DataParameter or DataSouce has same key retrun false , if DataKey has same key cover the vaule)
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="vaule">vaule</param>
        /// <returns>is success</returns>
        [MethodImplAttribute(MethodImplOptions.Synchronized)]
        public bool AddStaticDataKey(string key, IRunTimeStaticData vaule)
        {
            if (IsHasSameKey(key, IsAllCollectionKeyUnique?0:1) != null)
            {
                if (!RemoveStaticData(key, false))
                {
                    return false;
                }
            }
            runActuatorStaticDataKeyList.MyAdd(key, vaule);
            OnListChanged(true);
            return true;
        }

        /// <summary>
        /// Add Data into runActuatorStaticDataParameterList (if DataKey or DataSouce has same key retrun false , if DataParameter has same key cover the vaule) 
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="vaule">vaule</param>
        /// <returns>is success</returns>
        [MethodImplAttribute(MethodImplOptions.Synchronized)]
        public bool AddStaticDataParameter(string key, IRunTimeStaticData vaule)
        {
            if (IsHasSameKey(key, IsAllCollectionKeyUnique ? 0 : 2) != null)
            {
                if (!RemoveStaticData(key, false))
                {
                    return false;
                }
            }
            runActuatorStaticDataParameterList.MyAdd<IRunTimeStaticData>(key, vaule);
            OnListChanged(true);
            return true;
        }

        /// <summary>
        /// Add Data into runActuatorStaticDataSouceList (if DataKey or DataParameter has same key retrun false , if DataSouce has same key cover the vaule)
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="vaule">vaule</param>
        /// <returns>is success</returns>
        [MethodImplAttribute(MethodImplOptions.Synchronized)]
        public bool AddStaticDataSouce(string key, IRunTimeDataSource vaule)
        {
            if (IsHasSameKey(key, IsAllCollectionKeyUnique ? 0 : 3) != null)
            {
                if (!RemoveStaticData(key, false))
                {
                    return false;
                }
            }
            runActuatorStaticDataSouceList.MyAdd<IRunTimeDataSource>(key, vaule);
            OnListChanged(true);
            return true;
        }

        /// <summary>
        /// Remove Static Data in any list (if there not has any same key retrun false)
        /// </summary>
        /// <param name="key">key or Regex</param>
        /// <param name="isRegex">is use Regex</param>
        /// <returns>is success</returns>
        [MethodImplAttribute(MethodImplOptions.Synchronized)]
        public bool RemoveStaticData(string key, bool isRegex)
        {
            if (!isRegex)
            {
                var tempDataList = IsHasSameKey(key, 0);
                if (tempDataList == null)
                {
                    return false;
                }
                else if (tempDataList == runActuatorStaticDataKeyList)
                {
                    runActuatorStaticDataKeyList.Remove(key);
                }
                else if (tempDataList == runActuatorStaticDataParameterList)
                {
                    runActuatorStaticDataParameterList.Remove(key);
                }
                else if (tempDataList == runActuatorStaticDataSouceList)
                {
                    runActuatorStaticDataSouceList.Remove(key);
                }
                else
                {
                    //ErrorLog.PutInLog(string.Format("error to [RemoveStaticData] in ActuatorStaticDataCollection  the key is {0} ", key));
                    return false;
                }
            }
            else
            {
                try
                {
                    bool isFindAndRegexKey = false;
                    System.Text.RegularExpressions.Regex sr;
                    sr = new System.Text.RegularExpressions.Regex(key, System.Text.RegularExpressions.RegexOptions.None);
                    List<string> dataToDel = new List<string>();

                    foreach (var tempKey in runActuatorStaticDataKeyList.Keys)
                    {
                        if (sr.IsMatch(tempKey))
                        {
                            dataToDel.Add(tempKey);
                        }
                    }
                    foreach (string tempKey in dataToDel)
                    {
                        runActuatorStaticDataKeyList.Remove(tempKey);
                    }
                    if (dataToDel.Count > 0)
                    {
                        isFindAndRegexKey = true;
                        dataToDel.Clear();
                    }

                    foreach (var tempKey in runActuatorStaticDataParameterList.Keys)
                    {
                        if (sr.IsMatch(tempKey))
                        {
                            dataToDel.Add(tempKey);
                        }
                    }
                    foreach (string tempKey in dataToDel)
                    {
                        runActuatorStaticDataParameterList.Remove(tempKey);
                    }
                    if (dataToDel.Count > 0)
                    {
                        isFindAndRegexKey = true;
                        dataToDel.Clear();
                    }

                    foreach (var tempKey in runActuatorStaticDataSouceList.Keys)
                    {
                        if (sr.IsMatch(tempKey))
                        {
                            dataToDel.Add(tempKey);
                        }
                    }
                    foreach (string tempKey in dataToDel)
                    {
                        runActuatorStaticDataSouceList.Remove(tempKey);
                    }
                    if (dataToDel.Count > 0)
                    {
                        isFindAndRegexKey = true;
                        dataToDel.Clear();
                    }

                    if (!isFindAndRegexKey)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    //ErrorLog.PutInLog(ex);
                    return false;
                }

            }
            OnListChanged(true);
            return true;
        }

        /// <summary>
        /// set or change data in any list
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="configVaule">config Vaule</param>
        /// <returns>is success</returns>
        [MethodImplAttribute(MethodImplOptions.Synchronized)]
        public bool SetStaticData(string key, string configVaule)
        {
            var tempDataList = IsHasSameKey(key, 0);
            if (tempDataList == null)
            {
                return false;
            }
            else if (tempDataList == runActuatorStaticDataKeyList)
            {
                if (!runActuatorStaticDataKeyList[key].DataSet(configVaule))
                    return false;
            }
            else if (tempDataList == runActuatorStaticDataParameterList)
            {
                if (!runActuatorStaticDataParameterList[key].DataSet(configVaule))
                    return false;
            }
            else if (tempDataList == runActuatorStaticDataSouceList)
            {
                if (!runActuatorStaticDataSouceList[key].DataSet(configVaule))
                    return false;
            }
            else
            {
                //ErrorLog.PutInLog(string.Format("error to [RemoveStaticData] in ActuatorStaticDataCollection  the key is {0} ", key));
                return false;
            }
            OnListChanged(false);
            return true;
        }

        public IRunTimeStaticData GetStaticData(string key)
        {
            object tempStaticData = IsHasSameKey(key, 0);
            if(tempStaticData==null)
            {
                return null;
            }
            return tempStaticData as IRunTimeStaticData;
        }

        public object Clone()
        {
            return new ActuatorStaticDataCollection(runActuatorStaticDataKeyList.MyDeepClone(), runActuatorStaticDataParameterList.MyDeepClone(), runActuatorStaticDataSouceList.MyDeepClone());
        }

        public void Dispose()
        {
            runActuatorStaticDataKeyList.Clear();
            runActuatorStaticDataParameterList.Clear();
            runActuatorStaticDataSouceList.Clear();
        }

    }
}
