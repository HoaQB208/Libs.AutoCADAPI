using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Windows;
using Libs.AutoCADAPI.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace Libs.AutoCADAPI.Objects
{
    public static class RibbonAPI
    {
        public static RibbonTab CreateRibbonTab(string appName)
        {
            RibbonTab ribbon = new RibbonTab()
            {
                Title = appName,
                Id = appName
            };
            ComponentManager.Ribbon.Tabs.Add(ribbon);
            ribbon.IsActive = true;
            return ribbon;
        }

        public static RibbonPanelSource CreateGroup(this RibbonTab ribbon, string title)
        {
            RibbonPanelSource ribbonPanelSource = new RibbonPanelSource() { Title = title };
            RibbonPanel ribbonPanel = new RibbonPanel() { Source = ribbonPanelSource };
            ribbon.Panels.Add(ribbonPanel);
            return ribbonPanelSource;
        }

        public static RibbonRowPanel CreateColumn(this RibbonPanelSource group)
        {
            RibbonRowPanel column = new RibbonRowPanel();
            group.Items.Add(column);
            return column;
        }

        public static void AddBigButton(this RibbonPanelSource group, string text, string cmd, byte[] imgBytes, string description = "", bool isEnabled = true)
        {
            BitmapImage img = ByteArrayToBitmapImage(imgBytes, 32, 32);
            RibbonButton bt = new RibbonButton()
            {
                Text = text,
                Orientation = System.Windows.Controls.Orientation.Vertical,
                ShowText = true,
                Size = RibbonItemSize.Large,
                Image = img,
                LargeImage = img,
                CommandParameter = cmd,
                Description = description == "" ? text : description,
                CommandHandler = new CmdHandler(),
                IsEnabled = isEnabled
            };
            group.Items.Add(bt);
        }

        public static void AddSmallButton(this RibbonRowPanel column, string text, string cmd, Bitmap bitmap, string description = "", bool isEnabled = true)
        {
            BitmapImage img = GetBitmap(bitmap, 16, 16);
            RibbonButton bt = new RibbonButton()
            {
                Text = text,
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                ShowText = true,
                Size = RibbonItemSize.Standard,
                Image = img,
                LargeImage = img,
                CommandParameter = cmd,
                Description = description == "" ? text : description,
                CommandHandler = new CmdHandler(),
                IsEnabled = isEnabled
            };
            column.Items.Add(bt);
            column.Items.Add(new RibbonRowBreak());
        }

        public static void AddSmallCheckBox(this RibbonRowPanel column, string text, string cmd, string description = "")
        {
            RibbonCheckBox bt = new RibbonCheckBox()
            {
                Text = text,
                ShowText = true,
                IsCheckable = true,
                IsChecked = false,
                Size = RibbonItemSize.Standard,
                CommandParameter = cmd,
                Description = description == "" ? text : description,
                CommandHandler = new CmdHandler()
            };
            column.Items.Add(bt);
            column.Items.Add(new RibbonRowBreak());
        }

        public static void AddSeparator(this RibbonPanelSource group)
        {
            group.Items.Add(new RibbonSeparator());
        }


        class CmdHandler : System.Windows.Input.ICommand
        {
            public event EventHandler CanExecuteChanged { add { } remove { } }
            public bool CanExecute(object parameter) { return true; }
            public void Execute(object parameter)
            {
                if (parameter is RibbonCommandItem btn)
                {
                    Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                    doc.SendStringToExecute((string)btn.CommandParameter + " ", true, false, true);
                }
            }
        }

        private static BitmapImage GetBitmap(Bitmap bitmap, int height, int width)
        {
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = new MemoryStream(stream.ToArray());
            bmp.DecodePixelHeight = height;
            bmp.DecodePixelWidth = width;
            bmp.EndInit();
            return bmp;
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] imageData, int height, int width)
        {
            if (imageData == null || imageData.Length == 0)
                return null;

            using (var ms = new MemoryStream(imageData))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.DecodePixelHeight = height;
                bitmap.DecodePixelWidth = width;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }

        public static FlowDocument CreateListCmdForHelp(RibbonTab ribbon)
        {
            FlowDocument doc = new FlowDocument();
            if (ribbon != null)
            {
                Paragraph paragraph = new Paragraph();
                // Group
                foreach (RibbonPanel panel in ribbon.Panels)
                {
                    // Group Name
                    Bold groupName = new Bold(new Run("\n* " + StringUtils.RemoveLines(panel.Source.Title)))
                    {
                        Foreground = System.Windows.Media.Brushes.Orange
                    };
                    paragraph.Inlines.Add(groupName);

                    foreach (RibbonItem item in panel.Source.Items)
                    {
                        List<RibbonCommandItem> commandItems = GetRibbonCommandItems(item);
                        foreach (RibbonCommandItem cmd in commandItems)
                        {
                            // Từng lệnh
                            if (cmd != null)
                            {
                                // CMD lệnh + Text hiển thị
                                paragraph.Inlines.Add("\n\n    ");
                                Run command = new Run(StringUtils.RemoveLines(cmd.CommandParameter.ToString())) { FontWeight = FontWeights.Bold };
                                paragraph.Inlines.Add(command);
                                paragraph.Inlines.Add($" : {StringUtils.RemoveLines(cmd.Text)}");
                                // Miêu tả chi tiết
                                paragraph.Inlines.Add($"\n        {cmd.Description.Replace("\n", "\n        ")}");
                            }
                        }
                    }
                    paragraph.Inlines.Add(new Run("\n\n"));
                }
                doc.Blocks.Add(paragraph);
            }
            return doc;
        }
        private static List<RibbonCommandItem> GetRibbonCommandItems(RibbonItem ribbonItem)
        {
            List<RibbonCommandItem> items = new List<RibbonCommandItem>();

            if (ribbonItem is RibbonRowPanel rowPanel)
            {
                foreach (RibbonItem item in rowPanel.Items)
                {
                    items.Add(item as RibbonCommandItem);
                }
            }
            else items.Add(ribbonItem as RibbonCommandItem);

            return items;
        }
    }
}
