using iRacingSDK.Support;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Xml;

public class PortableSettingsProvider : SettingsProvider
{
    const string SETTINGSROOT = "Settings";
    XmlDocument _settingsXML = null;

    public override void Initialize(string name, NameValueCollection col)
    {
        base.Initialize(ApplicationName, col);
    }

    public override string ApplicationName
    {
        get { return Application.ProductName; }
        set { }
    }

    public override string Name
    {
        get { return "PortableSettingsProvider"; }
    }

    public virtual string GetAppSettingsPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }

    public virtual string GetAppSettingsFilename()
    {
        return "iracing-replay-director.settings";
    }

    public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propvals)
    {
        foreach (SettingsPropertyValue propval in propvals)
            SetValue(propval);

        SettingsXML.Save(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
    }

    public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
    {
        var values = new SettingsPropertyValueCollection();

        foreach (SettingsProperty setting in props)
        {
            var value = new SettingsPropertyValue(setting);
            value.IsDirty = false;
            value.SerializedValue = GetValue(setting);
            values.Add(value);
        }
        return values;
    }

    XmlDocument SettingsXML
    {
        get
        {
            if (_settingsXML != null)
                return _settingsXML;
            
            _settingsXML = new XmlDocument();

            try
            {
                _settingsXML.Load(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
            }
            catch (Exception ex)
            {
                TraceDebug.WriteLine("Unable to load settings");
                TraceDebug.WriteLine(ex.Message);

                var dec = _settingsXML.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
                _settingsXML.AppendChild(dec);

                var nodeRoot = _settingsXML.CreateNode(XmlNodeType.Element, SETTINGSROOT, "");
                _settingsXML.AppendChild(nodeRoot);
            }

            return _settingsXML;
        }
    }

    string GetValue(SettingsProperty setting)
    {
        string ret = "";

        try
        {
            ret = SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + setting.Name).InnerText;
        }
        catch (Exception)
        {
            if (setting.DefaultValue != null)
                ret = setting.DefaultValue.ToString();
        }

        return ret;
    }

    void SetValue(SettingsPropertyValue propVal)
    {
        XmlElement SettingNode = null;

        try
        {
            SettingNode = (XmlElement)SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + propVal.Name);
        }
        catch 
        {
        }

        if (SettingNode == null)
        {
            SettingNode = SettingsXML.CreateElement(propVal.Name);
            SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(SettingNode);
        }

        SettingNode.InnerText = propVal.SerializedValue == null ? null : propVal.SerializedValue.ToString();
    }
}
