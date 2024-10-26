using Jinget.Blazor.Attributes.Input;
using Jinget.Blazor.Attributes;
using Jinget.Blazor.Models;
using MudBlazor;
using Jinget.Blazor.Attributes.DropDownList;
using System.Text;
using Jinget.Blazor.Enums;
using Jinget.Core.ExtensionMethods.Enums;

namespace Jinget.Blazor.Test
{
    public class SampleModel : DynamicFormBaseModel
    {
        public static string EngLongText()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= 200; i++)
            {
                sb.AppendLine("This is a very long text");
            }

            return sb.ToString();
        }

        public static string PersianLongText()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= 200; i++)
            {
                sb.AppendLine("این یک متن طولانی است.");
            }

            return sb.ToString();
        }

        [JingetTextBoxElement(DisplayName = "نام", HelperText = "نام خود را منطبق با اطلاعات کارت ملی وارد نمایید",
            Order = 1)]
        public string? Name { get; set; }

        [JingetTextBoxElement(DisplayName = "نام خانوادگی",
            HelperText = "نام خانوادگی خود را منطبق با اطلاعات کارت ملی وارد نمایید", Order = 2)]
        public string? LastName { get; set; }

        [JingetPasswordBoxElement(DisplayName = "رمز عبور", Order = 3)]
        public string? Password { get; init; }

        [JingetEmailBoxElement(DisplayName = "پست الکترونیکی", Order = 4)]
        public string? EMail { get; init; }

        [Attributes.Picker.JingetDatePickerElement(DisplayName = "تاریخ تولد",
            CalendarType = JingetCalendarType.Jalali,
            IsRtl = true,
            ShowPickerButton = true,
            Order = 5)]
        public string? DoB { get; init; }

        // [Attributes.Picker.JingetDateRangePickerElement(DisplayName = "بازه زمانی سفر", Culture = "fa-IR", Order = 6)]
        // public DateRange? TravelDate { get; init; }

        [JingetLabelElement(DisplayName = "امتیاز اکتسابی", CssClass = "", HasLabel = true,
            HelperText = "این امتیاز بصورت خودکار توسط سیستم محاسبه شده است")]
        public int Score { get; set; } = 1850;

        [JingetTextAreaElement(DisplayName = "اطلاعات بیشتر", Rows = 3)]
        public string? Description { get; set; }

        [JingetNumberBoxElement(DisplayName = "سن", Order = 7)]
        public int Age { get; set; }

        [JingetDropDownListElement(DisplayName = "وضعیت2", Id = "cmb2", DefaultText = "---Choose---", Order = 8,
            IsRtl = false)]
        public string Status2 { get; set; }

        [JingetDropDownListElement(DisplayName = "وضعیت", Id = "cmbSearch",
            BindingFunction = nameof(GetStatusAsync), PreBindingFunction = nameof(PreBinding),
            PostBindingFunction = nameof(PostBinding),
            IsSearchable = true, DefaultText = "---انتخاب کنید---", HasLabel = true, LabelCssClass = "overlayed-label",
            Order = 9)]
        public int? Status { get; set; }

        [JingetDropDownListElement(DisplayName = "نوع کاربر", Id = "cmbUser",
            BindingFunction = nameof(GetUserLevelAsync),
            DefaultText = "---انتخاب کنید---", HasLabel = true, LabelCssClass = "overlayed-label", Order = 9)]
        public UserLevel Level { get; set; }

        public async Task<List<JingetDropDownItemModel>> GetUserLevelAsync(object? preBindingResult)
        {
            return await new JingetDropDownListElement().BindAsync(async () =>
            {
                var items = new List<UserLevelModel>();
                foreach (UserLevel s in Enum.GetValues(typeof(UserLevel)))
                    items.Add(new UserLevelModel { Id = (byte)s, Title = s.GetDescription() });

                return await Task.FromResult(items);
            });
        }

        private class UserLevelModel : BaseTypeModel
        {
        }

        [JingetDropDownListElement(DisplayName = "جنسیت", Id = "cmbGender",
            BindingFunction = nameof(GetStatusAsync), IsSearchable = true, DefaultText = "---انتخاب کنید---",
            HasLabel = true, LabelCssClass = "overlayed-label", Order = 10)]
        public string? Gender { get; set; }

        public async Task<string> PreBinding() => await Task.FromResult("This is pre binding");

        public async Task<string> PostBinding(object? preBindingResult, object? data) =>
            await Task.FromResult("This is post binding");

        public async Task<List<JingetDropDownItemModel>> GetStatusAsync(object? preBindingResult)
            => await new JingetDropDownListElement().BindAsync(async () =>
            {
                var t = preBindingResult;
                return await Task.FromResult(new List<StatusModel>
                {
                    new() { Id = 1, Title = "فعال" },
                    new() { Id = 2, Title = "غیرفعال" },
                    new() { Id = 3, Title = "نامشخص" }
                });
            });

        [JingetDropDownListTreeElement(DisplayName = "Geo", Id = "cmbTreeGeo", IsRtl = false,
            BindingFunction = nameof(GetGeoAsync),
            IsRequired = true, RequiredError = "required",
            IsSearchable = true, HasLabel = true, LabelCssClass = "overlayed-label", Order = 1)]
        public Guid Geo { get; set; }

        public async Task<List<JingetDropDownTreeItemModel>> GetGeoAsync(object? preBindingResult)
            => await new JingetDropDownListTreeElement().BindAsync<GeoModel, Guid>(async () =>
            {
                return await Task.FromResult(new List<GeoModel>
                {
                    new()
                    {
                        Id = Guid.Parse("37d6a0c4-3d05-4224-a651-2e5b6349608c"), ParentId = Guid.Empty, Title = "Iran"
                    },
                    new() { Id = Guid.Parse("55d6a0c4-3d05-4224-a651-2e5b6349608c"), ParentId = null, Title = "USA" },
                    new()
                    {
                        Id = Guid.Parse("56d6a0c4-3d05-4224-a651-2e5b6349608c"),
                        ParentId = Guid.Parse("37d6a0c4-3d05-4224-a651-2e5b6349608c"), Title = "Tehran"
                    },
                });
            });

        [JingetDropDownListTreeElement(DisplayName = "Department", Id = "cmbTreeDepartment", IsRtl = false,
            IsSearchable = true, HasLabel = true, LabelCssClass = "overlayed-label", Order = 2)]
        public int Department { get; set; }

        public enum UserLevel : byte
        {
            Admin = 1,
            Ordinal,
            Unknown
        }

        public class StatusModel : BaseTypeModel
        {
        }

        public class StatusGuidModel : BaseTypeModel<string>
        {
        }

        public class GeoModel : BaseTypeTreeModel<Guid>
        {
        }

        public class DepartmentModel : BaseTypeTreeModel<int>
        {
        }
    }
}