﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3074
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NOHunterGame.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("main.mp3")]
        public string BACKGROUND_MUSIC {
            get {
                return ((string)(this["BACKGROUND_MUSIC"]));
            }
            set {
                this["BACKGROUND_MUSIC"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("wrong_choice.wav")]
        public string BUTTON_WRONG_SOUND {
            get {
                return ((string)(this["BUTTON_WRONG_SOUND"]));
            }
            set {
                this["BUTTON_WRONG_SOUND"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("right_choice.wav")]
        public string BUTTON_RIGHT_SOUND {
            get {
                return ((string)(this["BUTTON_RIGHT_SOUND"]));
            }
            set {
                this["BUTTON_RIGHT_SOUND"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string BUTTON_MOUSE_ENTER_SOUND {
            get {
                return ((string)(this["BUTTON_MOUSE_ENTER_SOUND"]));
            }
            set {
                this["BUTTON_MOUSE_ENTER_SOUND"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string BUTTON_MOUSE_LEAVE_SOUND {
            get {
                return ((string)(this["BUTTON_MOUSE_LEAVE_SOUND"]));
            }
            set {
                this["BUTTON_MOUSE_LEAVE_SOUND"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("appear.wav")]
        public string BUTTON_APPEAR_SOUND {
            get {
                return ((string)(this["BUTTON_APPEAR_SOUND"]));
            }
            set {
                this["BUTTON_APPEAR_SOUND"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("go.wav")]
        public string START_SOUND {
            get {
                return ((string)(this["START_SOUND"]));
            }
            set {
                this["START_SOUND"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string FINISHED_SOUND {
            get {
                return ((string)(this["FINISHED_SOUND"]));
            }
            set {
                this["FINISHED_SOUND"] = value;
            }
        }
    }
}
