﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NovAtelLogReader.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("COM2")]
        public string SerialPort {
            get {
                return ((string)(this["SerialPort"]));
            }
            set {
                this["SerialPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Users\\tersarkisov\\Desktop\\range.txt")]
        public string PathForReading {
            get {
                return ((string)(this["PathForReading"]));
            }
            set {
                this["PathForReading"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("amqp://uniframe:uniframe@172.20.5.28:5672/")]
        public string RabbitConnectionString {
            get {
                return ((string)(this["RabbitConnectionString"]));
            }
            set {
                this["RabbitConnectionString"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("datapoint-raw-range")]
        public string QueueNameRange {
            get {
                return ((string)(this["QueueNameRange"]));
            }
            set {
                this["QueueNameRange"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public uint PublishRate {
            get {
                return ((uint)(this["PublishRate"]));
            }
            set {
                this["PublishRate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("115200")]
        public int SerialPortSpeed {
            get {
                return ((int)(this["SerialPortSpeed"]));
            }
            set {
                this["SerialPortSpeed"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("datapoint-raw-satvis")]
        public string QueueNameSatvis {
            get {
                return ((string)(this["QueueNameSatvis"]));
            }
            set {
                this["QueueNameSatvis"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("datapoint-raw-psrpos")]
        public string QueueNamePsrpos {
            get {
                return ((string)(this["QueueNamePsrpos"]));
            }
            set {
                this["QueueNamePsrpos"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("datapoint-raw-satxyz2")]
        public string QueueNameSatxyz2 {
            get {
                return ((string)(this["QueueNameSatxyz2"]));
            }
            set {
                this["QueueNameSatxyz2"] = value;
            }
        }
    }
}
