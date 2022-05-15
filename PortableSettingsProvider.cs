using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

internal abstract class PortableSettingsProvider : SettingsProvider
{
    public static void MakePortable<T>(ApplicationSettingsBase settings) where T : PortableSettingsProvider
    {
        var pp = settings.Providers.OfType<T>().First();

        foreach (SettingsProperty p in settings.Properties)
            p.Provider = pp;
        settings.Reload();
    }

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

    public abstract string GetAppSettingsFilename();

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
                Trace.WriteLine("Unable to load settings", "DEBUG");
                Trace.WriteLine(ex.Message, "DEBUG");

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

        if (propVal.SerializedValue != null && propVal.SerializedValue.ToString().StartsWith("<?"))
        {
            var cdata = SettingsXML.CreateCDataSection(propVal.SerializedValue.ToString());
            SettingNode.RemoveAll();
            SettingNode.AppendChild(cdata);
        }
        else
            SettingNode.InnerText = propVal.SerializedValue == null ? null : propVal.SerializedValue.ToString();
    }
}
