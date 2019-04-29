using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml;

public class Taginfo : INotifyPropertyChanged
{
    #region const
    private const string captor_T = "T";
    private const string captor_RH = "RH";
    private const string captor_MAG_MOV = "MAG_MOV";
    private const string captor_ANG = "ANG";
    private const string hex_T = "6E-2A-";
    private const string hex_RH = "6F-2A-";
    private const string hex_MAG_MOV = "06-2A-";
    private const string hex_ANG = "A1-2A-";
    #endregion

    /** mac adress */
    private string mac = "";

    /** local name */
    private string name = "";

    /** RSSI / power value */
    private Int16 rssi = 0;

    /** formated data*/
    private string data = "";

    /** existence of data used to hide/show data on display */
    private bool dataExist = false;


    /** used to get updates on data in ObservableCollection */
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(name));
        }
    }

    #region accessors
    public string TagMac { get { return mac; } set { mac = value; OnPropertyChanged("TagMac"); } }
    public string TagName { get { return name; } set { name = value; OnPropertyChanged("TagName"); } }
    public Int16 TagRssi { get { return rssi; } set { rssi = value; OnPropertyChanged("TagRssi"); } }
    public List<string> TagDataRaw = new List<string>();
    public string TagData { get { return data; } set { data = value; OnPropertyChanged("TagData"); } }
    private string TagCaptorType { get; set; }
    public object TagDataVisibility { get { if (dataExist) { return Visibility.Visible; } else { return Visibility.Collapsed; } } }
    #endregion

    /** @brief : store captor type in TagCaptorType
      */
    private void getCaptorType()
    {
        TagCaptorType = String.Empty;

        for (int i = 1; i < TagDataRaw.Count - 1; ++i)
        {
            if (TagDataRaw[i].Length > 6)
            {
                string type = TagDataRaw[i].Substring(0, 6);
                if (type.Equals(hex_T))
                {
                    TagCaptorType += captor_T;
                }
                else if (type.Equals(hex_RH))
                {
                    TagCaptorType += captor_RH;
                }
                else if (type.Equals(hex_MAG_MOV))
                {
                    TagCaptorType += captor_MAG_MOV;
                }
                else if (type.Equals(hex_ANG))
                {
                    TagCaptorType += captor_ANG;
                }
            }
        }
    }

    /** @brief : format TagRawData and store it in TagData
     * set visibility of data value depending on data existance
     * call getCaptorType
     */
    public void getData()
    {
        getCaptorType();
        dataExist = !TagCaptorType.Equals(String.Empty);
        if (TagCaptorType.Contains(captor_T))
        {
            int lsb = Convert.ToInt32(TagDataRaw[1].Substring(6, 2), 16);
            int msb = Convert.ToInt32(TagDataRaw[1].Substring(9, 2), 16);
            int data_int = lsb + (msb << 8);
            string data_str = data_int.ToString().Substring(0, 2) + "." + data_int.ToString().Substring(2, 2);
            TagData += String.Format("T° : {0}°C", data_str);
        }
        if (TagCaptorType.Contains(captor_MAG_MOV))
        {
            int lsb = Convert.ToInt32(TagDataRaw[1].Substring(6, 2), 16);
            int msb = Convert.ToInt32(TagDataRaw[1].Substring(9, 2), 16);
            int data_int = lsb + (msb << 8);

            TagData += String.Format("MAG/MOV : {0}", data_int);
        }
        if (TagCaptorType.Contains(captor_RH))
        {
            int data_int = Convert.ToInt32(TagDataRaw[2].Substring(6, 2), 16);
            TagData += String.Format("  RH : {0}%", data_int);
        }
        if (TagCaptorType.Contains(captor_ANG))
        {
            int lsb = Convert.ToInt32(TagDataRaw[1].Substring(6, 2), 16);
            int msb = Convert.ToInt32(TagDataRaw[1].Substring(9, 2), 16);
            int data_int_x = lsb + (msb << 8);
            lsb = Convert.ToInt32(TagDataRaw[1].Substring(12, 2), 16);
            msb = Convert.ToInt32(TagDataRaw[1].Substring(15, 2), 16);
            int data_int_y = lsb + (msb << 8);
            lsb = Convert.ToInt32(TagDataRaw[1].Substring(18, 2), 16);
            msb = Convert.ToInt32(TagDataRaw[1].Substring(21, 2), 16);
            int data_int_z = lsb + (msb << 8);
            TagData = String.Format("X:{0} Y:{1} Z:{2}", data_int_x.ToString(), data_int_y.ToString(), data_int_z.ToString());
        }
    }

    /** @brief : update showned values
     * @param [in] taginfo : Taginfo with updated values
     */
    public void update(Taginfo taginfo)
    {
        this.TagName = taginfo.TagName;
        this.TagRssi = taginfo.TagRssi;
        this.TagData = taginfo.TagData;
    }
}
