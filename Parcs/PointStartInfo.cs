using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Parcs
{
    [DataContract]
    public class PointStartInfo
    {
        /// <summary>
        /// Method.ReflectedType.FullName
        /// </summary>
        [DataMember]
        public string NamespaceAndClass { get; internal set; }
        /// <summary>
        /// Method.Name
        /// </summary>
        [DataMember]
        public string MethodName { get; internal set; }
        [DataMember]
        public string AssemblyName { get; set; }
        [DataMember]
        public bool IsStatic { get; set; }
        public PointStartInfo(Func<PointInfo, Task> func)
        {
            NamespaceAndClass = func.Method.ReflectedType.FullName;
            MethodName = func.Method.Name;
            AssemblyName = func.Method.Module.ToString();
            IsStatic = func.Method.IsStatic;
        }
        //
        public PointStartInfo(Action<PointInfo> func)
        {
            NamespaceAndClass = func.Method.ReflectedType.FullName;
            MethodName = func.Method.Name;
            AssemblyName = func.Method.Module.ToString();
            IsStatic = func.Method.IsStatic;
        }
        /*
        [DataMember]
        private Dictionary<Type, object> _dict { get; set; } = new Dictionary<Type, object>();
        [DataMember]
        public IReadOnlyDictionary<Type, object> Paramethers { get => _dict; }
        public void AddParamether<T>(T paramether)
        {
            _dict.Add(typeof(T), paramether);
        }*/
    }
}