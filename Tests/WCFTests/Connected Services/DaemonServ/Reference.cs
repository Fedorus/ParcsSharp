﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WCFTests.DaemonServ {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ChannelType", Namespace="http://schemas.datacontract.org/2004/07/Parcs")]
    public enum ChannelType : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Any = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        TCP = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        NamedPipe = 2,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ControlSpace", Namespace="http://schemas.datacontract.org/2004/07/Parcs")]
    [System.SerializableAttribute()]
    public partial class ControlSpace : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private WCFTests.DaemonServ.Channel[] ChannelsOnCurrentDaemonField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private WCFTests.DaemonServ.PointCreationManager CreatorField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] DaemonAdressesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid IDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public WCFTests.DaemonServ.Channel[] ChannelsOnCurrentDaemon {
            get {
                return this.ChannelsOnCurrentDaemonField;
            }
            set {
                if ((object.ReferenceEquals(this.ChannelsOnCurrentDaemonField, value) != true)) {
                    this.ChannelsOnCurrentDaemonField = value;
                    this.RaisePropertyChanged("ChannelsOnCurrentDaemon");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public WCFTests.DaemonServ.PointCreationManager Creator {
            get {
                return this.CreatorField;
            }
            set {
                if ((object.ReferenceEquals(this.CreatorField, value) != true)) {
                    this.CreatorField = value;
                    this.RaisePropertyChanged("Creator");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string[] DaemonAdresses {
            get {
                return this.DaemonAdressesField;
            }
            set {
                if ((object.ReferenceEquals(this.DaemonAdressesField, value) != true)) {
                    this.DaemonAdressesField = value;
                    this.RaisePropertyChanged("DaemonAdresses");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid ID {
            get {
                return this.IDField;
            }
            set {
                if ((this.IDField.Equals(value) != true)) {
                    this.IDField = value;
                    this.RaisePropertyChanged("ID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PointCreationManager", Namespace="http://schemas.datacontract.org/2004/07/Parcs")]
    [System.SerializableAttribute()]
    public partial class PointCreationManager : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Random randField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Random rand {
            get {
                return this.randField;
            }
            set {
                if ((object.ReferenceEquals(this.randField, value) != true)) {
                    this.randField = value;
                    this.RaisePropertyChanged("rand");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Channel", Namespace="http://schemas.datacontract.org/2004/07/Parcs")]
    [System.SerializableAttribute()]
    public partial class Channel : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string IPField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid PointIDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int PortField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private WCFTests.DaemonServ.ChannelType TypeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string IP {
            get {
                return this.IPField;
            }
            set {
                if ((object.ReferenceEquals(this.IPField, value) != true)) {
                    this.IPField = value;
                    this.RaisePropertyChanged("IP");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid PointID {
            get {
                return this.PointIDField;
            }
            set {
                if ((this.PointIDField.Equals(value) != true)) {
                    this.PointIDField = value;
                    this.RaisePropertyChanged("PointID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Port {
            get {
                return this.PortField;
            }
            set {
                if ((this.PortField.Equals(value) != true)) {
                    this.PortField = value;
                    this.RaisePropertyChanged("Port");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public WCFTests.DaemonServ.ChannelType Type {
            get {
                return this.TypeField;
            }
            set {
                if ((this.TypeField.Equals(value) != true)) {
                    this.TypeField = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FileTransferData", Namespace="http://schemas.datacontract.org/2004/07/Parcs.WCF")]
    [System.SerializableAttribute()]
    public partial class FileTransferData : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private WCFTests.DaemonServ.ControlSpace ControlSpaceField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] FileDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FileNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HashField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PathField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public WCFTests.DaemonServ.ControlSpace ControlSpace {
            get {
                return this.ControlSpaceField;
            }
            set {
                if ((object.ReferenceEquals(this.ControlSpaceField, value) != true)) {
                    this.ControlSpaceField = value;
                    this.RaisePropertyChanged("ControlSpace");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] FileData {
            get {
                return this.FileDataField;
            }
            set {
                if ((object.ReferenceEquals(this.FileDataField, value) != true)) {
                    this.FileDataField = value;
                    this.RaisePropertyChanged("FileData");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FileName {
            get {
                return this.FileNameField;
            }
            set {
                if ((object.ReferenceEquals(this.FileNameField, value) != true)) {
                    this.FileNameField = value;
                    this.RaisePropertyChanged("FileName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Hash {
            get {
                return this.HashField;
            }
            set {
                if ((object.ReferenceEquals(this.HashField, value) != true)) {
                    this.HashField = value;
                    this.RaisePropertyChanged("Hash");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Path {
            get {
                return this.PathField;
            }
            set {
                if ((object.ReferenceEquals(this.PathField, value) != true)) {
                    this.PathField = value;
                    this.RaisePropertyChanged("Path");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="DaemonServ.IDaemonService")]
    public interface IDaemonService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDaemonService/CreatePoint", ReplyAction="http://tempuri.org/IDaemonService/CreatePointResponse")]
        WCFTests.DaemonServ.Channel CreatePoint(string Name, WCFTests.DaemonServ.ChannelType channelType, WCFTests.DaemonServ.ControlSpace controlSpaceData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDaemonService/CreatePoint", ReplyAction="http://tempuri.org/IDaemonService/CreatePointResponse")]
        System.Threading.Tasks.Task<WCFTests.DaemonServ.Channel> CreatePointAsync(string Name, WCFTests.DaemonServ.ChannelType channelType, WCFTests.DaemonServ.ControlSpace controlSpaceData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDaemonService/DestroyControlSpace", ReplyAction="http://tempuri.org/IDaemonService/DestroyControlSpaceResponse")]
        void DestroyControlSpace(WCFTests.DaemonServ.ControlSpace data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDaemonService/DestroyControlSpace", ReplyAction="http://tempuri.org/IDaemonService/DestroyControlSpaceResponse")]
        System.Threading.Tasks.Task DestroyControlSpaceAsync(WCFTests.DaemonServ.ControlSpace data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDaemonService/SendFile", ReplyAction="http://tempuri.org/IDaemonService/SendFileResponse")]
        void SendFile(WCFTests.DaemonServ.FileTransferData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDaemonService/SendFile", ReplyAction="http://tempuri.org/IDaemonService/SendFileResponse")]
        System.Threading.Tasks.Task SendFileAsync(WCFTests.DaemonServ.FileTransferData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDaemonService/TestWork", ReplyAction="http://tempuri.org/IDaemonService/TestWorkResponse")]
        bool TestWork();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDaemonService/TestWork", ReplyAction="http://tempuri.org/IDaemonService/TestWorkResponse")]
        System.Threading.Tasks.Task<bool> TestWorkAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDaemonServiceChannel : WCFTests.DaemonServ.IDaemonService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DaemonServiceClient : System.ServiceModel.ClientBase<WCFTests.DaemonServ.IDaemonService>, WCFTests.DaemonServ.IDaemonService {
        
        public DaemonServiceClient() {
        }
        
        public DaemonServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DaemonServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DaemonServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DaemonServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public WCFTests.DaemonServ.Channel CreatePoint(string Name, WCFTests.DaemonServ.ChannelType channelType, WCFTests.DaemonServ.ControlSpace controlSpaceData) {
            return base.Channel.CreatePoint(Name, channelType, controlSpaceData);
        }
        
        public System.Threading.Tasks.Task<WCFTests.DaemonServ.Channel> CreatePointAsync(string Name, WCFTests.DaemonServ.ChannelType channelType, WCFTests.DaemonServ.ControlSpace controlSpaceData) {
            return base.Channel.CreatePointAsync(Name, channelType, controlSpaceData);
        }
        
        public void DestroyControlSpace(WCFTests.DaemonServ.ControlSpace data) {
            base.Channel.DestroyControlSpace(data);
        }
        
        public System.Threading.Tasks.Task DestroyControlSpaceAsync(WCFTests.DaemonServ.ControlSpace data) {
            return base.Channel.DestroyControlSpaceAsync(data);
        }
        
        public void SendFile(WCFTests.DaemonServ.FileTransferData data) {
            base.Channel.SendFile(data);
        }
        
        public System.Threading.Tasks.Task SendFileAsync(WCFTests.DaemonServ.FileTransferData data) {
            return base.Channel.SendFileAsync(data);
        }
        
        public bool TestWork() {
            return base.Channel.TestWork();
        }
        
        public System.Threading.Tasks.Task<bool> TestWorkAsync() {
            return base.Channel.TestWorkAsync();
        }
    }
}