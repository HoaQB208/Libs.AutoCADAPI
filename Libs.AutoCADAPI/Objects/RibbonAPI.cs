using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

public class RibbonAPI
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

    public static RibbonPanelSource CreateGroup(RibbonTab ribbon, string title)
    {
        RibbonPanelSource ribbonPanelSource = new RibbonPanelSource() { Title = title };
        RibbonPanel ribbonPanel = new RibbonPanel() { Source = ribbonPanelSource };
        ribbon.Panels.Add(ribbonPanel);
        return ribbonPanelSource;
    }

    public static RibbonRowPanel CreateColumn(RibbonPanelSource group)
    {
        RibbonRowPanel column = new RibbonRowPanel();
        group.Items.Add(column);
        return column;
    }

    public static void AddBigButton(RibbonPanelSource group, string text, string cmd, Bitmap bitmap, string description = "", bool isEnabled = true)
    {
        BitmapImage img = GetBitmap(bitmap, 32, 32);
        RibbonButton bt = new RibbonButton()
        {
            Text = text,
            Orientation = Orientation.Vertical,
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

    public static void AddSmallButton(RibbonRowPanel column, string text, string cmd, Bitmap bitmap, string description = "", bool isEnabled = true)
    {
        BitmapImage img = GetBitmap(bitmap, 16, 16);
        RibbonButton bt = new RibbonButton()
        {
            Text = text,
            Orientation = Orientation.Horizontal,
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

    public static void AddSmallCheckBox(RibbonRowPanel column, string text, string cmd, string description = "")
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

    public static FlowDocument CreateListCmdForHelp(RibbonTab ribbon)
    {
        FlowDocument doc = new FlowDocument();
        Paragraph paragraph = new Paragraph();

        foreach (RibbonPanel panel in ribbon.Panels)
        {
            Bold groupName = new Bold(new Run("* " + panel.Source.Title))
            {
                Foreground = System.Windows.Media.Brushes.OrangeRed
            };
            paragraph.Inlines.Add(groupName);

            foreach (RibbonItem item in panel.Source.Items)
            {
                List<RibbonCommandItem> commandItems = GetRibbonCommandItems(item);
                foreach (RibbonCommandItem cmd in commandItems)
                {
                    if (cmd != null)
                    {
                        paragraph.Inlines.Add("\n    ");

                        Run command = new Run(cmd.CommandParameter.ToString())
                        {
                            FontWeight = FontWeights.Bold
                        };
                        paragraph.Inlines.Add(command);

                        paragraph.Inlines.Add($" : {cmd.Text}\n        {cmd.Description}");
                    }
                }
            }

            paragraph.Inlines.Add(new Run("\n\n"));
        }

        doc.Blocks.Add(paragraph);
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
