using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace Libs.AutoCADAPI.Utils
{
    public class EnumUtils
    {
        public static string GetDescription(Enum e)
        {
            System.Reflection.MemberInfo[] members = e.GetType().GetMember(e.ToString());
            object[] attributes = members[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
            DescriptionAttribute descriptionAttribute = attributes[0] as DescriptionAttribute;
            return descriptionAttribute.Description;
        }

        public static List<string> GetDescriptions(Type enumType)
        {
            List<string> descs = new List<string>();
            string[] names = Enum.GetNames(enumType);
            foreach (string name in names)
            {
                object[] attributes = enumType.GetField(name).GetCustomAttributes(typeof(DescriptionAttribute), true);
                DescriptionAttribute descriptionAttribute = attributes[0] as DescriptionAttribute;
                descs.Add(descriptionAttribute.Description);
            }
            return descs;
        }

        // Ví dụ lấy danh sách Enum dùng để Binding
        //public ObservableCollection<MyEnum> EnumValues { get; } = new ObservableCollection<MyEnum>(Enum.GetValues(typeof(MyEnum)).Cast<MyEnum>());





        public static IEnumerable<EnumWithDescription> GetEnumWithDescriptions<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<Enum>().Select(e => new EnumWithDescription { Value = e, Description = GetDescription(e) });
        }

        public class EnumWithDescription
        {
            public Enum Value { get; set; }
            public string Description { get; set; }
        }

        // Cách sử dụng

        ///ViewModel
        // public ObservableCollection<EnumWithDescription> EnumValues { get; set; } = new ObservableCollection<EnumWithDescription>(GetEnumWithDescriptions<MyEnum>());

        ///xalm
        //<ComboBox ItemsSource = "{Binding EnumValues}"
        //  DisplayMemberPath="Description"
        //  SelectedValuePath="Value"
        //  SelectedValue="{Binding SelectedEnum}" />

    }
}