using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using WIA;

namespace Scanning
{
    public class Scanner
    {

        public const string Config = "scanner.cfg";
        private Device _scanDevice;
        private Item _scannerItem;
        private Random _rnd = new Random();

        private Dictionary<string, object> _defaultDeviceProp;

        public bool IsVirtual;

        public Scanner()
        {
            try
            {
                LoadConfig();
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка конфигурации, требуется ручная настройка сканера");
                Configuration();
            }
        }

        public void Configuration()
        {
            try
            {
                var commonDialog = new CommonDialogClass();
                _scanDevice = commonDialog.ShowSelectDevice(WiaDeviceType.ScannerDeviceType, true);

                if (_scanDevice == null)
                    return;

                var items = commonDialog.ShowSelectItems(_scanDevice);

                if (items.Count < 1)
                    return;

                _scannerItem = items[1];

                SaveProp(_scanDevice.Properties, ref _defaultDeviceProp);

                SaveConfig();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Интерфейс сканера не доступен");
            }
        }

        private void SaveProp(WIA.Properties props, ref Dictionary<string, object> dic)
        {
            if (dic == null) dic = new Dictionary<string, object>();

            foreach (Property property in props)
            {
                var propId = property.PropertyID.ToString();
                var propValue = property.get_Value();

                dic[propId] = propValue;
            }
        }

        public void SetDuplexMode(bool isDuplex)
        {
            // WIA property ID constants
            const string wiaDpsDocumentHandlingSelect = "3088";
            const string wiaDpsPages = "3096";

            // WIA_DPS_DOCUMENT_HANDLING_SELECT flags
            const int feeder = 0x001;
            const int duplex = 0x004;

            if (_scanDevice == null) return;

            if (isDuplex)
            {
                SetProp(_scanDevice.Properties, wiaDpsDocumentHandlingSelect, (duplex | feeder));
                SetProp(_scanDevice.Properties, wiaDpsPages, 1);
            }
            else
            {
                try
                {
                    SetProp(_scanDevice.Properties, wiaDpsDocumentHandlingSelect, _defaultDeviceProp[wiaDpsDocumentHandlingSelect]);
                    SetProp(_scanDevice.Properties, wiaDpsPages, _defaultDeviceProp[wiaDpsPages]);
                }
                catch (Exception e)
                {
                    MessageBox.Show(String.Format("Сбой восстановления режима сканирования:{0}{1}", Environment.NewLine, e.Message));
                }
            }
        }

        public MemoryStream MemScan()
        {
            if ((_scannerItem == null) && (!IsVirtual))
            {
                MessageBox.Show("Сканер не настроен, активировано виртуальное устройство!", "Info");
                //return null;
                IsVirtual = true;
            }

            var stream = new MemoryStream();

            if (IsVirtual)
            {
                if (_rnd.Next(3) == 0)
                {
                    return null;
                }

                var btm = GetVirtualScan();
                btm.Save(stream, ImageFormat.Jpeg);
                return stream;
            }

            try
            {
                var result = _scannerItem.Transfer(FormatID.wiaFormatJPEG);
                var wiaImage = (ImageFile)result;
                var imageBytes = (byte[])wiaImage.FileData.get_BinaryData();

                using (var ms = new MemoryStream(imageBytes))
                {
                    using (var bitmap = Bitmap.FromStream(ms))
                    {
                        bitmap.Save(stream, ImageFormat.Jpeg);
                    }
                }

            }
            catch (Exception)
            {
                return null;
            }

            return stream;
        }

        private Bitmap GetVirtualScan()
        {
            const int imgSize = 777;
            var defBtm = new Bitmap(imgSize, imgSize);
            var g = Graphics.FromImage(defBtm);

            var r = new Random();

            g.FillRectangle(new SolidBrush(Color.FromArgb(r.Next(0, 50), r.Next(0, 50), r.Next(0, 50))), 0, 0, imgSize, imgSize); // bg

            for (int i = 0; i < r.Next(1000, 3000); i++)
            {
                var den = r.Next(200, 255);
                var br = new SolidBrush(Color.FromArgb(den, den, den));

                den -= 100;

                var pr = new Pen(Color.FromArgb(den, den, den), 1);

                var size = r.Next(1, 8);
                var x = r.Next(0, imgSize - size);
                var y = r.Next(0, imgSize - size);
                g.FillEllipse(br, x, y, size, size);
                g.DrawEllipse(pr, x, y, size, size);
            }

            g.DrawString("Виртуальный сканер", new Font(FontFamily.GenericSerif, 25), Brushes.Aqua, new RectangleF(0, 0, imgSize, imgSize));

            g.Flush();

            return defBtm;
        }

        private void SaveConfig()
        {
            var settings = new List<string>();
            settings.Add("[device]");
            settings.Add(String.Format("DeviceID;{0}", _scanDevice.DeviceID));

            foreach (IProperty property in _scanDevice.Properties)
            {
                var propstring = string.Format("{1}{0}{2}{0}{3}", ";", property.Name, property.PropertyID, property.get_Value());
                settings.Add(propstring);
            }

            settings.Add("[item]");
            settings.Add(String.Format("ItemID;{0}", _scannerItem.ItemID));
            foreach (IProperty property in _scannerItem.Properties)
            {
                var propstring = string.Format("{1}{0}{2}{0}{3}", ";", property.Name, property.PropertyID, property.get_Value());
                settings.Add(propstring);
            }

            File.WriteAllLines(Config, settings.ToArray());
        }

        private enum loadMode { undef, device, item };

        private void LoadConfig()
        {
            var settings = File.ReadAllLines(Config);

            var mode = loadMode.undef;

            foreach (var setting in settings)
            {
                if (setting.StartsWith("[device]"))
                {
                    mode = loadMode.device;
                    continue;
                }

                if (setting.StartsWith("[item]"))
                {
                    mode = loadMode.item;
                    continue;
                }

                if (setting.StartsWith("DeviceID"))
                {
                    var deviceid = setting.Split(';')[1];
                    var devMngr = new DeviceManagerClass();

                    foreach (IDeviceInfo deviceInfo in devMngr.DeviceInfos)
                    {
                        if (deviceInfo.DeviceID == deviceid)
                        {
                            _scanDevice = deviceInfo.Connect();
                            break;
                        }
                    }

                    if (_scanDevice == null)
                    {
                        MessageBox.Show("Сканнер из конигурации не найден");
                        return;
                    }

                    _scannerItem = _scanDevice.Items[1];
                    continue;
                }

                if (setting.StartsWith("ItemID"))
                {
                    var itemid = setting.Split(';')[1];
                    continue;
                }

                var sett = setting.Split(';');
                switch (mode)
                {
                    case loadMode.device:
                        SetProp(_scanDevice.Properties, sett[1], sett[2]);
                        break;

                    case loadMode.item:
                        SetProp(_scannerItem.Properties, sett[1], sett[2]);
                        break;
                }
            }
            SaveProp(_scanDevice.Properties, ref _defaultDeviceProp);
        }

        private static void SetProp(IProperties prop, object property, object value)
        {
            try
            {
                prop[property].set_Value(value);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
